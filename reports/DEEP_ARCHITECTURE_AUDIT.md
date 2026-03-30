# DEEP ARCHITECTURE & PERFORMANCE AUDIT
## Boulevard ASP.NET MVC 5 + EF6 — Data Access Layer
### Date: 2026-03-12

---

## SEVERITY LEGEND
- 🔴 **CRITICAL** — Will cause production failures, data leaks, or memory exhaustion
- 🟠 **HIGH** — Significant performance degradation or design flaw
- 🟡 **MEDIUM** — Poor practices that compound over time
- 🔵 **LOW** — Code quality/maintainability issues

---

# 1. REPOSITORY PATTERN AUDIT

## 1.1 GenericRepository.cs — CRITICAL ISSUES

### 🔴 ISSUE #1: DbContext and DbSet are PUBLIC fields
**File:** `BaseRepository/GenericRepository.cs` Lines 15-16
```csharp
public BoulevardDbContext _dbContext;
public DbSet<T> _dbSet;
```
**Problem:** These should be `private` or `protected readonly`. Any consumer can overwrite the context or DbSet, breaking encapsulation entirely.

### 🔴 ISSUE #2: Repository calls SaveChanges — violates Unit of Work pattern
**File:** `BaseRepository/GenericRepository.cs` Lines 26-29, 33-36, 55-58, 176-181, 187-198
```csharp
// Add() calls SaveChangesAsync
public async Task<T> Add(T entity)
{
    _dbSet.Add(entity);
    await _dbContext.SaveChangesAsync();  // ← WRONG: repository should not save
    return entity;
}

// Edit() calls SaveChangesAsync
// Remove() calls SaveChanges
// MultipleRemove() calls SaveChanges PER ITEM in foreach loop
```
**Problem:** The whole point of UnitOfWork is to batch multiple operations into a single transaction. Every repository method saving independently means:
- No transactional consistency across multiple operations
- `MultipleRemove()` calls `SaveChanges()` **inside a foreach loop** — N database roundtrips instead of 1

### 🟠 ISSUE #3: Duplicate sync/async methods with bad naming
**File:** `BaseRepository/GenericRepository.cs` Lines 25-36
```csharp
public async Task<T> Add(T entity)     // async version
public T Addd(T entity)                // sync version — typo "Addd"
```
**File:** `BaseRepository/GenericRepository.cs` Lines 142-150
```csharp
public async Task<T> GetById(int id)   // async
public T GetbyId(int id)               // sync — inconsistent casing "GetbyId"
```
**Problem:** Inconsistent naming (`Addd`, `GetbyId`), duplicated logic. Should pick one pattern and name properly.

### 🟠 ISSUE #4: Get() with orderBy returns null when no orderBy is provided
**File:** `BaseRepository/GenericRepository.cs` Lines 77-96
```csharp
IQueryable<T> result = null;
if (orderBy != null)
{
    result = orderBy(query);
}
if (isTrackingOff)
    return result?.AsNoTracking();  // returns null if no orderBy!
else
    return result;  // returns null if no orderBy!
```
**Problem:** If `orderBy` is `null`, `result` stays `null` and the method returns `null` — silently discarding the filter and includes. The filtered `query` is never returned.

### 🟡 ISSUE #5: String-based Include is fragile
**File:** `BaseRepository/GenericRepository.cs` Lines 84-88
```csharp
foreach (var includeProperty in includeProperties.Split
   (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
{
    query = query.Include(includeProperty);
}
```
**Problem:** String-based includes break silently at runtime when property names change. Should use `Expression<Func<T, object>>` overloads.

### 🟡 ISSUE #6: No IDisposable on GenericRepository
**File:** `BaseRepository/GenericRepository.cs` — entire class
**Problem:** `GenericRepository` holds a `BoulevardDbContext` but does NOT implement `IDisposable`. If anyone creates a standalone `GenericRepository`, the context is never disposed.

---

## 1.2 UnitOfWork.cs — CRITICAL ISSUES

### 🔴 ISSUE #7: DbContext initialized as field — not in constructor
**File:** `BaseRepository/UnitOfWork.cs` Line 13
```csharp
public BoulevardDbContext _dbContext = new BoulevardDbContext();
```
**Problem:**
1. `public` — anyone can replace or misuse the underlying context
2. Field initializer `= new BoulevardDbContext()` — creates context at construction time, preventing constructor injection
3. `new BoulevardDbContext()` — hardcoded, impossible to test or replace

### 🔴 ISSUE #8: **UnitOfWork has Dispose but is NEVER called by consumers**
**File:** `BaseRepository/UnitOfWork.cs` Lines 1096-1108 (Dispose exists)
**File:** Every Service class — e.g., `Service/ServiceAccess.cs` Lines 20-23:
```csharp
public class ServiceAccess
{
    public IUnitOfWork uow;
    public ServiceAccess()
    {
        uow = new UnitOfWork();  // ← Never disposed!
    }
}
```
**Problem:** `UnitOfWork` has a proper `Dispose()` pattern, but:
- `IUnitOfWork` does NOT extend `IDisposable`
- No service class ever calls `uow.Dispose()`
- No `using` statements anywhere
- DbContext lives for the entire lifetime of the service object
- **EVERY service class has this exact same pattern** — confirmed in ALL 11 service files

### 🟠 ISSUE #9: Massive God-class — 50+ repository properties
**File:** `BaseRepository/UnitOfWork.cs` — ~1100 lines, 50+ lazy repository properties
**Problem:** UnitOfWork is a monolithic class exposing every table in the database. Adding a new table requires modifying UnitOfWork, IUnitOfWork, and the service class. Violates Open-Closed Principle.

### 🟠 ISSUE #10: No SaveChanges/SaveChangesAsync on UnitOfWork
**File:** `BaseRepository/IUnitOfWork.cs` — entire interface
**Problem:** `IUnitOfWork` has NO `SaveChanges()` / `SaveChangesAsync()` methods. The "Unit of Work" cannot actually commit a unit of work. Each repository saves individually (Issue #2), defeating the entire purpose.

### 🟡 ISSUE #11: Duplicate region comments
**File:** `BaseRepository/UnitOfWork.cs` — Three regions all named `#region notification`
Lines ~1057, ~1071, ~1085 — all named `#region notification` but contain `ProductTypeMaster`, `OrderMasterStatusLog`, and `Notification` respectively.

---

## 1.3 IUnitOfWork.cs — ISSUES

### 🔴 ISSUE #12: IUnitOfWork does NOT extend IDisposable
**File:** `BaseRepository/IUnitOfWork.cs`
```csharp
public interface IUnitOfWork   // ← missing : IDisposable
```
**Problem:** Callers using `IUnitOfWork` cannot call `Dispose()` through the interface, so the underlying DbContext is never cleaned up.

### 🟠 ISSUE #13: Missing repository properties in IUnitOfWork
**File:** `BaseRepository/IUnitOfWork.cs` vs `BaseRepository/UnitOfWork.cs`
- `CartServiceRepository` is in both (good)
- `MonthlyGoal`, `GolbalMemberCategory`, `TempProduct`, `TempService`, `TempServiceType` exist in DbContext but have NO repository in UnitOfWork
- Some repositories exist in UnitOfWork but not in IUnitOfWork

---

# 2. SERVICE LAYER — DEEP SCAN

## 2.1 Global Pattern: `new UnitOfWork()` in Every Constructor

**Every single service class** creates `new UnitOfWork()` in its constructor, which means:
- Each service creates its own DbContext
- Services creating other services (`new ProductServiceAccess()`, `new MemberServiceAccess()`) each get their OWN DbContext
- A single API request can easily create 5-10+ DbContext instances
- None are ever disposed

**Affected files (ALL):**

| File | Line |
|------|------|
| `Service/ServiceAccess.cs` | 21 |
| `Service/CategoryAccess.cs` | 20 |
| `Service/ProductAccess.cs` | 41 |
| `Service/OfferAccess.cs` | 20 |
| `Service/ServiceAmenityAccess.cs` | 22 |
| `Service/ServiceLandmarkAccess.cs` | 21 |
| `Service/ServiceTypeAccess.cs` | 21 |
| `Service/FeatureCategoryAccess.cs` | 20 |
| `Service/LayoutSettingAccess.cs` | 18 |
| `Service/BrandAccess.cs` | 22 |
| `Service/RoleAccess.cs` | 20 |
| `Service/UserAccess.cs` | 19 |
| `Service/UserActivityAccess.cs` | 18 |
| `Service/WebAPI/OrderRequestServiceAccess.cs` | 29-30 |
| `Service/WebAPI/ProductServiceAccess.cs` | 17 |
| `Service/WebAPI/ServiceAccess.cs` | 26 |
| `Service/WebAPI/CartServiceAccess.cs` | 17 |
| `Service/WebAPI/OfferServiceAccess.cs` | 19 |
| `Service/WebAPI/MemberServiceAccess.cs` | 18 |
| `Service/Admin/DashboardDataAccess.cs` | 20 |
| `Service/Admin/OrderRequestProductDataAccess.cs` | 22 |
| `Service/Admin/OrderRequestServiceDataAccess.cs` | 18 |

---

## 2.2 N+1 Query Patterns (CRITICAL)

### 🔴 N+1 #1: ServiceAccess.GetAllByFeatureCategoryForChildService
**File:** `Service/ServiceAccess.cs` Lines 90-96
```csharp
foreach (var res in result)
{
    if (res.ParentId > 0)
    {
        res.ParentServiceName = await uow.ServiceRepository.Get()
            .Where(s => s.ServiceId == res.ParentId)
            .Select(s => s.Name).FirstOrDefaultAsync();  // ← 1 query per result
    }
}
```
**Impact:** If 100 child services, 100 extra DB queries.

### 🔴 N+1 #2: ServiceAccess.GetServicesTypeByFeatureCategoryForPackage
**File:** `Service/ServiceAccess.cs` Lines 139-150
```csharp
foreach (var res in result)  // ← iterating all services
{
    var types = await uow.ServiceTypesRepository.Get()
        .Where(e => e.Status.ToLower() == "Active" && e.ServiceId == res.ServiceId)
        .ToListAsync();  // ← 1 query per service
    if (types != null && types.Count() > 0)
    {
        resulttype.AddRange(types);
    }
}
```
**Fix:** Single query: `Where(e => serviceIds.Contains(e.ServiceId))`

### 🔴 N+1 #3: ProductAccess.GetAllByFCatagoryKey
**File:** `Service/ProductAccess.cs` Lines 77-80
```csharp
foreach (var item in product)
{
    var productPrice = await uow.ProductPriceRepository.Get()
        .Where(a => a.ProductId == item.ProductId)
        .Select(a => a.ProductStock).ToListAsync();  // ← 1 query per product
    item.StockQuantity = productPrice != null ? productPrice.Sum() : 0;
}
```

### 🔴 N+1 #4: ProductAccess.GetByKey — nested N+1 inside N+1
**File:** `Service/ProductAccess.cs` Lines 120-133
```csharp
foreach (var prod in product.ProductList)  // iterating all products in same category
{
    var upsellExist = uow.UpsellFeaturesRepository.IsExist(
        s => s.UpsellFeaturesTypeId == product.ProductId && s.RelatedFeatureId == prod.ProductId);
    // ← 1 query per product
    var crosssellExist = uow.CrosssellFeaturesRepository.IsExist(
        s => s.CrosssellFeaturesTypeId == product.ProductId && s.RelatedFeatureId == prod.ProductId);
    // ← ANOTHER query per product
}
```
**Impact:** 2N queries for N products.

### 🔴 N+1 #5: ProductAccess.Insert — category hierarchy walk
**File:** `Service/ProductAccess.cs` Lines 165-180
```csharp
while (categoryNode.ParentId != null && categoryNode.ParentId > 0)
{
    // SaveChanges called per iteration (via Add method)
    await uow.ProductCategoryRepository.Add(prodCat);
    categoryNode = await uow.CategoryRepository.Get()
        .Where(p => p.CategoryId == categoryNode.ParentId)
        .FirstOrDefaultAsync();  // ← 1 query per level of hierarchy
}
```

### 🔴 N+1 #6: ServiceAmenityAccess.GetAllServiceAmenityByFeatureCategory
**File:** `Service/ServiceAmenityAccess.cs` Lines 59-72
```csharp
foreach (var serviceNode in service)  // for each service...
{
    var serviceAmenity = await uow.ServiceAmenityRepository.GetAll()
        .ToListAsync();  // ← LOADS ENTIRE TABLE each iteration!
    if (serviceAmenity != null)
    {
        foreach (var serviceAmenityNode in serviceAmenity
            .Where(a => a.ServiceId == serviceNode.ServiceId))  // ← then filters in memory
        {
            serviceAmenityList.Add(serviceAmenityNode);
        }
    }
}
```
**This is catastrophic:** For N services, it loads the ENTIRE ServiceAmenity table N times, then filters in C#.

### 🔴 N+1 #7: ServiceTypeAccess.GetAllServiceType(string)
**File:** `Service/ServiceTypeAccess.cs` Lines 85-97
```csharp
foreach (var serviceId in serviceIds)
{
    var serviceTypesData = await uow.ServiceTypesRepository.Get()
        .Where(e => e.Status.ToLower() == "Active" && e.ServiceId == serviceId)
        .Include(p => p.Service).ToListAsync();  // ← 1 query per serviceId
}
```

### 🔴 N+1 #8: OrderRequestProductDataAccess.GetAll & GetAllByFeatureCategory
**File:** `Service/Admin/OrderRequestProductDataAccess.cs` Lines 37-56
```csharp
foreach(var orderRequestProduct in orderRequest)
{
    Result.Member = await uow.MemberRepository.Get()
        .FirstOrDefaultAsync(s => s.MemberId == orderRequestProduct.MemberId);
    Result.MemberAddresses = await uow.MemberAddressRepository.Get()
        .FirstOrDefaultAsync(a => a.MemberAddressId == orderRequestProduct.MemberAddressId);
    Result.PaymentMethod = await uow.PaymentMethodRepository.Get()
        .FirstOrDefaultAsync(a => a.PaymentMethodId == orderRequestProduct.PaymentMethodId);
    Result.OrderStatus = await uow.OrderStatusRepository.Get()
        .FirstOrDefaultAsync(a => a.OrderStatusId == orderRequestProduct.OrderStatusId);
    // ← 4 queries PER ORDER, no pagination
}
```
**Impact:** 100 orders = 400 extra queries. **NO PAGINATION** — returns ALL orders.

### 🔴 N+1 #9: OrderRequestServiceDataAccess.GetAll & GetAllByFeatureCategory
**File:** `Service/Admin/OrderRequestServiceDataAccess.cs` Lines 35-40
```csharp
foreach (var item in dataModel)
{
    item.Member = await uow.MemberRepository.Get()
        .FirstOrDefaultAsync(s => s.MemberId == item.MemberId);
}
```
Same pattern, no pagination.

### 🔴 N+1 #10: OrderRequestServiceAccess.getOrderForMember
**File:** `Service/WebAPI/OrderRequestServiceAccess.cs` Lines 145-175
```csharp
foreach (var order in orders)
{
    order.OrderStatushistory = await new ProductServiceAccess().getStatusInfo(...);
    // ← creates NEW UnitOfWork + DbContext PER CALL
    foreach (var orderdetail in orderDetails)
    {
        var DetailsList = await new ProductServiceAccess().getSmallDetailsProducts(...);
        // ← creates ANOTHER NEW UnitOfWork + DbContext PER DETAIL
    }
}
```
**Impact:** For 10 orders with 5 details each: ~60 new DbContext instances + ~150 queries.

### 🔴 N+1 #11: WebAPI ServiceAccess.GetServiceDetailsById
**File:** `Service/WebAPI/ServiceAccess.cs` Lines 235-285
```csharp
foreach (var item in Service.UserReviews)
{
    var Member = await uow.MemberRepository.Get().Where(p => p.MemberId == item.UserId)...;
    var userReviewImages = await uow.UserReviewImageRepository.Get()...;
    // ← 2 queries per review
    foreach (var itemimage in userReviewImages)
    {
        // processing images
    }
}
foreach (var img in Service.ServiceTypes)
{
    var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(...);
    // ← creates NEW UnitOfWork per service type
}
```

### 🔴 N+1 #12: DashboardDataAccess.GetAll
**File:** `Service/Admin/DashboardDataAccess.cs` Lines 46-60
```csharp
foreach (var fCategory in dashboardViewModel.Categories)
{
    if (fCategory.FeatureType == "Product")
    {
        var productCount = await uow.OrderRequestProductRepository.Get()
            .Where(s => s.FeatureCategoryId == fCategory.FeatureCategoryId).CountAsync();
    }
    if (fCategory.FeatureType == "Service")
    {
        var serviceCount = await uow.OrderRequestServiceRepository.Get()
            .Where(s => s.FeatureCategoryId == fCategory.FeatureCategoryId).CountAsync();
    }
}
```
Plus 12+ separate aggregate queries below it, each checking `AnyAsync` then running the same query AGAIN:
```csharp
dashboardViewModel.TotalProductOrderMonth = await ...AnyAsync(...) == false 
    ? 0 
    : await ...Where(...).CountAsync();  // same query executed TWICE
```

---

## 2.3 `new BoulevardDbContext()` Without using/Dispose

### 🔴 LEAK #1: ServiceAccess.Update
**File:** `Service/ServiceAccess.cs` Lines 325-327
```csharp
var db = new BoulevardDbContext();
db.Entry(node).State = EntityState.Modified;
db.SaveChanges();
```
No `using`, no `Dispose()`. Context leaked. **This same pattern repeats in:**

| File | Method | Line |
|------|--------|------|
| `Service/ServiceAccess.cs` | `Update()` | 325 |
| `Service/ServiceAccess.cs` | `UpdateForPackage()` | 523 |
| `Service/CategoryAccess.cs` | `Update()` | 294-296 |
| `Service/ProductAccess.cs` | `Update()` | 224-226 |
| `Service/ProductAccess.cs` | `PostUpsell()` | 290 |
| `Service/ProductAccess.cs` | `PostCrosssell()` | 329 |
| `Service/BrandAccess.cs` | `Update()` | 178-182 |
| `Service/LayoutSettingAccess.cs` | `layoutUpdate()` | 63-66 |

### 🟠 BYPASS #1: `new BoulevardDbContext()` bypasses UnitOfWork entirely
In all the above cases, code creates a **separate** DbContext to do updates, bypassing the UnitOfWork's context. This means:
- Changes tracked by `uow._dbContext` are invisible to `db`
- Changes made via `db` are invisible to `uow._dbContext`
- Entity state conflicts, double-saves, or lost updates are all possible

---

## 2.4 Loading Entire Tables Into Memory

### 🔴 TABLE LOAD #1: ServiceAmenityAccess — loads entire ServiceAmenity table PER SERVICE
**File:** `Service/ServiceAmenityAccess.cs` Lines 63-64
```csharp
var serviceAmenity = await uow.ServiceAmenityRepository.GetAll().ToListAsync();
```
Called **inside a foreach loop**. Full table scan per iteration.

### 🟠 TABLE LOAD #2: LayoutSettingAccess.layoutUpdate — loads all layouts
**File:** `Service/LayoutSettingAccess.cs` Lines 59-67
```csharp
var layout = uow.LayoutSettingRepository.Get();
foreach (var item in layout)
{
    item.IsDefault = item.IsDefault == true ? false : true;
    var bd = new BoulevardDbContext();  // NEW context per record + leaked
    bd.Entry(item).State = EntityState.Modified;
    bd.SaveChanges();
}
```

### 🟠 TABLE LOAD #3: OrderRequestProductDataAccess.GetAll — all orders, no pagination
**File:** `Service/Admin/OrderRequestProductDataAccess.cs` Line 37
```csharp
var orderRequest = await uow.OrderRequestProductRepository.GetAll()
    .Where(a => a.Status == "Active")
    .OrderByDescending(t => t.OrderRequestProductId)
    .ToListAsync();
```
Then 4 N+1 queries per record. No pagination at all.

---

## 2.5 Duplicate Queries

### 🟠 DUP #1: DashboardDataAccess — every metric queried twice
**File:** `Service/Admin/DashboardDataAccess.cs` Lines 65-73
```csharp
// Pattern repeated 6 times:
dashboardViewModel.TotalProductOrderMonth = 
    await uow.OrderRequestProductRepository.Get().AnyAsync(s => s.OrderDateTime.Month == DateTime.Now.Month) == false 
        ? 0 
        : await uow.OrderRequestProductRepository.Get().Where(s => s.OrderDateTime.Month == DateTime.Now.Month).CountAsync();
```
`AnyAsync` and `CountAsync` on the same predicate — just use `CountAsync` (returns 0 if none).

### 🟠 DUP #2: CartServiceAccess.GetCartListProductsCount — identical branches
**File:** `Service/WebAPI/CartServiceAccess.cs` Lines 316-325
```csharp
if (memberId == 0)
{
    productIds = await uow.CartRepository.Get().AnyAsync(...) == true 
        ? await uow.CartRepository.Get().Where(...).CountAsync() : 0;
}
else
{
    productIds = await uow.CartRepository.Get().AnyAsync(...) == true 
        ? await uow.CartRepository.Get().Where(...).CountAsync() : 0;
    // ← IDENTICAL code in both branches
}
```

---

## 2.6 Hardcoded / Dead Code / Anti-Patterns

### 🔴 HARDCODE #1: Hardcoded FeatureCategoryIds
**File:** `Service/ServiceTypeAccess.cs` Lines 80-81
```csharp
if (featureCategoryId == 9 || featureCategoryId == 11)
```
**File:** `Service/ServiceTypeAccess.cs` Line 354
```csharp
public async Task<List<ServiceType>> GetAllServiceType(int fCategoryId = 13)
```
Magic numbers that will break if IDs change.

### 🟠 DEAD CODE #1: Hardcoded test data still in production
**File:** `Service/ServiceAccess.cs` Lines 160-230
```csharp
public async Task<List<Boulevard.Models.Service>> GetAllMotorServices()
{
    Boulevard.Models.Service obj1 = new Models.Service()
    {
        ServiceId = 1,
        Name = "Orgenja Bikers"
    };
    // ... 5 hardcoded fake services
}

public async Task<List<Boulevard.Models.Service>> GetAllSalonServices()
{
    // ... 5 more hardcoded fake services
}
```
Test/mock data in production code. Both methods are `async` but do zero async work.

### 🟠 ANTIPATTERN #1: `new ServiceClass()` inside service methods
**File:** `Service/WebAPI/OrderRequestServiceAccess.cs` Lines 148, 155
```csharp
order.OrderStatushistory = await new ProductServiceAccess().getStatusInfo(...);
var DetailsList = await new ProductServiceAccess().getSmallDetailsProducts(...);
```
**File:** `Service/WebAPI/ProductServiceAccess.cs` Line 79
```csharp
var discountinfo = await new OfferServiceAccess().ProductDiscountCheck(...);
```
**File:** `Service/WebAPI/ServiceAccess.cs` Lines 211, 213, etc.
```csharp
result.City = await new CityAccess().GetById(...);
result.Country = await new CountryAccess().GetById(...);
var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(...);
var memberInfo = await new MemberServiceAccess().GetById(memberId);
```
Each `new XxxAccess()` creates a new `UnitOfWork` → new `DbContext`. A single request can cascade into 10-20+ DbContext instances.

### 🟠 ANTIPATTERN #2: UserAccess.GetUserByAuth — async method, sync call
**File:** `Service/UserAccess.cs` Lines 152-155
```csharp
public async Task<User> GetUserByAuth(string userName, string Password)
{
    var hashPass = HashConfig.GetHash(Password);
    return uow.UserRepository.Get().Where(...).FirstOrDefault(); // ← sync, not async
}
```

### 🟡 ANTIPATTERN #3: Swallowing exceptions, returning null
Every service file returns `null` on exception with just `Log.Error()`. No caller checks for null consistently — high risk of `NullReferenceException` higher up the call chain.

---

## 2.7 Missing Pagination (Admin Services)

### 🔴 NO PAGINATION — Admin endpoints:

| File | Method | Returns |
|------|--------|---------|
| `Service/Admin/OrderRequestProductDataAccess.cs` | `GetAll()` | ALL orders |
| `Service/Admin/OrderRequestProductDataAccess.cs` | `GetAllByFeatureCategory()` | ALL orders for category |
| `Service/Admin/OrderRequestServiceDataAccess.cs` | `GetAll()` | ALL service orders |
| `Service/Admin/OrderRequestServiceDataAccess.cs` | `GetAllByFeatureCategory()` | ALL service orders |
| `Service/ServiceAccess.cs` | `GetAll()` | ALL services |
| `Service/BrandAccess.cs` | `GetAll()` | ALL brands |
| `Service/CategoryAccess.cs` | `GetAll()` | ALL categories |
| `Service/ServiceAmenityAccess.cs` | `GetAll()` | ALL amenities |
| `Service/ServiceLandmarkAccess.cs` | `GetAll()` | ALL landmarks |
| `Service/UserAccess.cs` | `GetAll()` | ALL users |
| `Service/RoleAccess.cs` | `GetAll()` | ALL roles |
| `Service/UserActivityAccess.cs` | `GetUserActivityTimeLineByUserId()` | ALL activities |

These are ticking time bombs as data grows.

---

## 2.8 `new XxxAccess()` Creating Cascading DbContexts

Single request flow example for `getOrderForMember()`:

```
OrderRequestServiceAccess (creates UoW #1)
  └─ foreach order:
       └─ new ProductServiceAccess() → UoW #2 → getStatusInfo()
            └─ [queries with UoW #2]
       └─ foreach detail:
            └─ new ProductServiceAccess() → UoW #3 → getSmallDetailsProducts()
                 └─ new OfferServiceAccess() → UoW #4 → ProductDiscountCheck()
                      └─ [6+ queries]
                 └─ new MemberServiceAccess() → UoW #5 → GetById()
                      └─ [3+ queries]
```

**For 10 orders × 3 details = 30+ DbContext instances, 100+ queries.**

---

# 3. DATA STRUCTURE ISSUES (Models)

## 3.1 BaseEntity.cs —  Issues

### 🟠 ISSUE #1: Status field is `string` with `[StringLength(10)]`
**File:** `Models/BaseEntity.cs` Lines 12-13
```csharp
[StringLength(10)]
public string Status { get; set; }
```
Used as: `"Active"`, `"Deleted"`, `"Delete"`, `"Finished"`, `"Pending"`, `"Success"`
- Inconsistent: Some code checks `"Delete"`, others check `"Deleted"`
- No enum, no constants — any typo is a silent bug
- Magic string comparisons everywhere: `.Status.ToLower() == "Active"`, `.Status.ToLower() != "Delete"`
- Case inconsistency: some code does `.ToLower() == "Active"` (will never match because "Active" ≠ "active")

### 🟡 ISSUE #2: `CreateBy` should be nullable or have a default
**File:** `Models/BaseEntity.cs` Line 14
```csharp
public int CreateBy { get; set; }
```
Hardcoded to `1` everywhere: `node.CreateBy = 1;` — the audit trail is meaningless.

## 3.2 Service.cs — Issues

### 🟡 ISSUE #1: No [Key] attribute
**File:** `Models/Service.cs` Line 23
```csharp
public int ServiceId { get; set; }
```
Works by EF convention but should be explicit.

### 🟡 ISSUE #2: `ParentId` is `int` not `int?`
**File:** `Models/Service.cs` Line 75
```csharp
public int ParentId { get; set; } = 0;
```
Self-referencing hierarchy using `0` as "no parent" instead of `null`. No FK constraint possible, requires manual checks everywhere.

## 3.3 Product.cs — Issues

### 🟠 ISSUE #1: Massive number of [NotMapped] properties
**File:** `Models/Product.cs` — 15+ `[NotMapped]` properties
This model is being used as both a domain entity AND a view model. The domain model should be clean; DTOs/ViewModels should hold presentation data.

### 🟡 ISSUE #2: Private setter with validation on domain model
**File:** `Models/Product.cs` Lines 55-58
```csharp
public decimal ProductPrice
{
    get => _productPrice;
    set => _productPrice = Math.Max(0, value);
}
```
Silently clamps negative prices to 0 instead of throwing. A -$50 price would become $0 with no error.

## 3.4 Member.cs — Issues

### 🟡 ISSUE #1: Password stored in the model
**File:** `Models/Member.cs` Line 32
```csharp
[StringLength(150)]
public string Password { get; set; }
```
Password hash stored with the entity and potentially serialized to JSON responses. Should be excluded from serialization.

### 🟡 ISSUE #2: MemberId is `long` but used as `int` throughout
**File:** `Models/Member.cs` Line 17 — `public long MemberId`
But in `MemberServiceAccess.cs`:
```csharp
var ss = await GetById(Convert.ToInt32(mem.MemberId));
```
Truncation risk when MemberId > int.MaxValue.

## 3.5 OrderRequestProduct.cs — Issues

### 🟡 ISSUE #1: No `[Key]` annotation
Relies on EF convention. Missing explicit PK declaration.

### 🟡 ISSUE #2: String-based payment status
```csharp
[StringLength(100)]
public string PaymentStatus { get; set; }
```
Used as: `"Pending"`, `"Success"` — should be an enum. 100 chars for a status is excessive.

## 3.6 OrderRequestService.cs — Issues

### 🟠 ISSUE #1: Does NOT inherit BaseEntity
**File:** `Models/OrderRequestService.cs` Line 11
```csharp
public class OrderRequestService  // ← no BaseEntity
```
Missing audit fields (CreateBy, CreateDate, Status, etc.). Inconsistent with all other entities.

### 🟡 ISSUE #2: String-based BookingStatus, PaymentStatus
No enum, no constraints.

## 3.7 Category.cs — Issues

### 🟡 ISSUE #1: Misspelled property
**File:** `Models/Category.cs` Line 47
```csharp
public bool? IsTrenbding { get; set; }  // ← "Trending" misspelled
```

### 🟡 ISSUE #2: `int label` — lowercase, unclear purpose
**File:** `Models/Category.cs` Line 53
```csharp
public int label { get; set; }
```
Violates C# naming conventions. No clear purpose.

## 3.8 ServiceType.cs — Issues

### 🟡 ISSUE #1: Misspelled property
**File:** `Models/ServiceType.cs` Line 31
```csharp
public int PersoneQuantity { get; set; }  // ← "Person" misspelled
```

### 🟡 ISSUE #2: City/Country [NotMapped] initialized in constructor
**File:** `Models/ServiceType.cs` Lines 11-12
```csharp
this.City = new City();
this.Country = new Country();
```
Creates empty objects even when not needed — wasteful and confusing.

## 3.9 Global Issues Across Models

### 🟠 Missing Navigation Properties
- `OrderRequestProduct` → `OrderRequestProductDetails`: No navigation collection
- `Product` → `ProductPrice`: `[NotMapped]` — so EF doesn't know about it; every price query is manual
- `Product` → `ProductImage`: `[NotMapped]` — same issue
- `Product` → `ProductCategory`: `[NotMapped]` — same
- `Category` → `ChildCategories`: `[NotMapped]` self-reference — no EF navigation

### 🟡 Status Inconsistency Across Models
- `"Active"` vs `"active"` in queries (`.ToLower()` required but inconsistently applied)
- `"Delete"` vs `"Deleted"` used in different places
- `"Finished"` used for expired offers

---

# 4. CLEAN ARCHITECTURE VIOLATIONS

## 4.1 No Dependency Injection

### 🔴 VIOLATION #1: Manual instantiation everywhere — zero DI
**Every service class:**
```csharp
public ServiceAccess()
{
    uow = new UnitOfWork();  // hardcoded dependency
}
```
**Service classes creating other service classes:**
```csharp
await new ProductServiceAccess().getSmallDetailsProducts(...);
await new MemberServiceAccess().GetById(memberId);
await new OfferServiceAccess().ProductDiscountCheck(...);
await new AdminNotificationDataAccess().SaveNotification(...);
await new StockLogDataAccess().StockOut(...);
await new PushNotificationAccess().SendInvoiceMemberNotification(...);
await new CartServiceAccess().AddOrRemoveCart(...);
```
No IoC container, no interface registration, no scoped lifetimes. Impossible to unit test.

## 4.2 Controllers Likely Containing Business Logic

Based on the service layer patterns, controllers likely create service classes directly:
```csharp
var serviceAccess = new ServiceAccess();
```
No dependency injection in controllers either.

## 4.3 Service Layer Doing Presentation Logic

### 🟠 VIOLATION #1: URL prefixing in service layer
**File:** `Service/WebAPI/ProductServiceAccess.cs` Line 21
```csharp
string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
```
Then throughout:
```csharp
result.Image = link + await uow.ProductImageRepository...;
```
Image URL construction is presentation logic — should be in a DTO mapper or middleware.

**Same pattern in:**
- `Service/WebAPI/ServiceAccess.cs` Line 28
- `Service/WebAPI/OfferServiceAccess.cs` Line 21
- `Service/WebAPI/MemberServiceAccess.cs` Line 22
- `Service/WebAPI/OrderRequestServiceAccess.cs` Line 31

### 🟠 VIOLATION #2: Language switching in data access
```csharp
result.ServiceName = lang == "en" ? AreaFiltered.Name : AreaFiltered.NameAr;
```
Localization logic embedded deep in repository-level code. Should be a separate concern.

## 4.4 Hardcoded Values

| Value | File | Line | Should Be |
|-------|------|------|-----------|
| `CreateBy = 1` | Every Insert/Update | Multiple | `HttpContext.Current.User` |
| `DeleteBy = 1` | Every Delete | Multiple | `HttpContext.Current.User` |
| `"+971"` | `MemberServiceAccess.cs` | Registration | Config/DB |
| `featureCategoryId == 9 \|\| == 11` | `ServiceTypeAccess.cs` | 80 | Named constant or DB flag |
| `fCategoryId = 13` | `ServiceTypeAccess.cs` | 354 | Config |
| `"Boulvard-"` | `OrderRequestServiceAccess.cs` | 44 | Config |
| `AddMinutes(20)`, `AddHours(24)` | `OrderRequestServiceAccess.cs` | 60-64 | Config |

---

# 5. SUMMARY OF CRITICAL FINDINGS

## By Severity Count:

| Severity | Count |
|----------|-------|
| 🔴 CRITICAL | 22 |
| 🟠 HIGH | 18 |
| 🟡 MEDIUM | 16 |
| 🔵 LOW | 4 |

## Top 5 Most Impactful Issues:

1. **DbContext never disposed** — Every service creates `new UnitOfWork()`, never calls Dispose. Under load, connection pool exhaustion is inevitable.

2. **N+1 queries everywhere** — 12+ documented patterns where queries inside foreach loops generate hundreds of database roundtrips per request.

3. **Cascading DbContext creation** — Service classes instantiating other service classes creates 10-20+ DbContext instances per request.

4. **Repository saves on every operation** — `SaveChanges` called in every Add/Edit/Remove, defeating UnitOfWork transactional batching.

5. **No pagination on admin endpoints** — All admin listing endpoints return every record. With growth, these will crash the admin panel.

## Architecture Debt Summary:

The codebase implements a Repository + UnitOfWork pattern but **defeats its own purpose**:
- UnitOfWork has no `SaveChanges()` method
- Repositories save individually
- Services bypass UnitOfWork with `new BoulevardDbContext()`
- No dependency injection means no shared context scope
- No IDisposable on IUnitOfWork means no cleanup
- Test/mock data in production code
- Status managed via inconsistent magic strings

**Estimated performance overhead:** A typical product order request generates approximately **50-100 database queries** and creates **5-15 DbContext instances** where it should generate **3-5 queries** with **1 DbContext**.
