# Boulevard ASP.NET MVC 5 — Complete Caching Analysis Report

**Generated:** March 12, 2026  
**Scope:** Full codebase analysis of all service files, controllers, views, and configuration

---

## Executive Summary

The Boulevard codebase has **ZERO caching** implemented anywhere:
- ❌ No `MemoryCache` usage
- ❌ No `HttpRuntime.Cache` usage
- ❌ No `[OutputCache]` attributes
- ❌ No `outputCacheProfiles` in Web.config
- ❌ No `System.Runtime.Caching` references
- ❌ `compilation debug="true"` is set (disables view compilation optimization)
- ❌ No `optimizeCompilations` setting configured

Every single request hits the database directly, including data that rarely changes.

---

## FINDING 1: Triple LayoutSetting DB Calls Per Admin Page Load

**Severity:** 🔴 CRITICAL — Every single admin page makes 3 identical DB queries

### 1A: `_Layout.cshtml` (1st call)
- **File:** `Areas/Admin/Views/Shared/_Layout.cshtml` Lines 2-4
- **Code:**
  ```csharp
  Boulevard.Service.LayoutSettingAccess layoutSettingAccess = new Boulevard.Service.LayoutSettingAccess();
  var layout = layoutSettingAccess.GetDefaultLayout();
  ```
- **What:** Queries `LayoutSetting` table for the default theme (LogoHeader, MainHeader, Body, SideBar)
- **Change frequency:** Rarely — only when admin toggles theme
- **Impact:** 1 DB query per page

### 1B: `_Header.cshtml` (2nd call — DUPLICATE)
- **File:** `Areas/Admin/Views/Shared/_Header.cshtml` Lines 8-11
- **Code:**
  ```csharp
  Boulevard.Service.LayoutSettingAccess layoutSettingAccess = new Boulevard.Service.LayoutSettingAccess();
  var db = new BoulevardDbContext();   // ← UNUSED DbContext also created!
  var layout = layoutSettingAccess.GetDefaultLayout();
  ```
- **What:** Same exact query as `_Layout.cshtml`
- **Extra waste:** An unused `BoulevardDbContext` is also instantiated (Line 9)
- **Impact:** 2nd identical DB query + wasted DbContext

### 1C: `_Scripts.cshtml` (3rd call — TRIPLICATE)
- **File:** `Areas/Admin/Views/Shared/_Scripts.cshtml` Lines 5-7
- **Code:**
  ```csharp
  Boulevard.Service.LayoutSettingAccess layoutSettingAccess = new Boulevard.Service.LayoutSettingAccess();
  var layout = layoutSettingAccess.GetDefaultLayout();
  ```
- **What:** Same exact query again
- **Impact:** 3rd identical DB query

### Recommended Fix
- **Strategy:** `MemoryCache` with cache key `"LayoutSetting_Default"`
- **Duration:** 30 minutes, with cache invalidation on `layoutUpdate()` call
- **Where:** Add caching inside `LayoutSettingAccess.GetDefaultLayout()`
- **Estimated saving:** Eliminates 2-3 DB queries per admin page load

---

## FINDING 2: Admin Notification Query in View

**Severity:** 🟠 HIGH

- **File:** `Areas/Admin/Views/Shared/_Header.cshtml` Lines 21-22
- **Code:**
  ```csharp
  adminotifictaion = new AdminNotificationDataAccess().GetAdminNotificationwithout(10);
  ```
- **What:** Queries the `AdminNotification` table (top 10) synchronously on every admin page load
- **Change frequency:** Frequently (but latest-10 rarely changes within seconds)
- **Impact:** 1 DB query per admin page (runs in Razor view, blocking rendering)

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"AdminNotifications_Top10"`
- **Duration:** 30-60 seconds (short TTL since notifications are near-realtime)
- **Alternative:** Move to AJAX lazy-load instead of synchronous view query

---

## FINDING 3: Country List — Static Reference Data

**Severity:** 🟠 HIGH

### 3A: API Endpoint (Mobile/Frontend)
- **File:** `Service/WebAPI/CountryAccess.cs` Lines 23-37
- **Controller:** `Controllers/CountryController.cs` Lines 22-31 (`GetAllCountries`)
- **What:** `SELECT * FROM Countries WHERE Status = 'active'` — full table scan each call
- **Change frequency:** **Rarely** — countries list is essentially static

### 3B: Admin Dropdown Population
- **Files (12 occurrences):**
  - `Areas/Admin/Controllers/ServiceController.cs` Lines 425, 476
  - `Areas/Admin/Controllers/CityController.cs` Line 128
  - `Areas/Admin/Controllers/PropertyInformationController.cs` Line 215
  - `Areas/Admin/Controllers/PackageController.cs` Lines 213, 264
  - `Areas/Admin/Controllers/MemberAddressController.cs` Lines 194, 229
  - `Areas/Admin/Controllers/OrderRequestProductController.cs` Line 453
  - `Areas/Admin/Controllers/OrderRequestServiceController.cs` Line 163
  - `Areas/Admin/Controllers/AirportController.cs` Lines 141, 177
- **What:** Each admin form page re-queries the countries table for `<select>` dropdowns

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"Countries_Active_{lang}"`
- **Duration:** 1 hour (or until admin adds/edits a country)
- **Where:** Cache in `CountryAccess.GetAll()` and admin `CountryDataAccess.GetAll()`

---

## FINDING 4: City List — Static Reference Data

**Severity:** 🟠 HIGH

### 4A: API Endpoint
- **File:** `Service/WebAPI/CityAccess.cs` Lines 23-37
- **Controller:** `Controllers/CityController.cs` Lines 22-31 (`GetAllCities`)
- **What:** `SELECT * FROM Cities WHERE Status = 'active'`
- **Change frequency:** **Rarely**

### 4B: Cities by Country
- **File:** `Service/WebAPI/CityAccess.cs` Lines 63-78 (`GetCitiesByCountryId`)
- **Controller:** `Controllers/CityController.cs` Lines 53-63
- **What:** `SELECT * FROM Cities WHERE CountryId = @id AND Status = 'active'`

### 4C: Admin Dropdown Population (14 occurrences)
- Same pattern as countries — every form page re-queries cities
- **Files:** ServiceController, PropertyInformationController, PackageController, MemberAddressController, OrderRequestProductController, OrderRequestServiceController, AirportController

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"Cities_Active_{lang}"` and `"Cities_Country_{countryId}_{lang}"`
- **Duration:** 1 hour
- **Where:** Cache in `CityAccess.GetAll()` and `CityAccess.GetCitiesByCountryId()`

---

## FINDING 5: FeatureCategory Lists — Core Reference Data

**Severity:** 🔴 CRITICAL — Used in nearly every admin page and API endpoint

### 5A: API Endpoint
- **File:** `Service/WebAPI/FeatureCategoryServiceAccess.cs` Lines 30-51
- **Controller:** `Controllers/FeatureCategoryController.cs` Lines 23-29 (`GetAllFeatureCategory`)
- **What:** `SELECT * FROM FeatureCategories WHERE IsDelete = false`
- **Change frequency:** **Very rarely** — feature categories are foundational config

### 5B: Service Layer (Admin)
- **File:** `Service/FeatureCategoryAccess.cs` Lines 32-42 (`GetAll`)
- **File:** `Service/FeatureCategoryAccess.cs` Lines 44-60 (`GetAllByFCatagoryKey`)

### 5C: Admin Controller Usage (19+ occurrences)
- `CategoryController.cs` Lines 35, 72
- `BrandController.cs` Line 40
- `WebHtmlController.cs` Lines 160, 173, 189, 337
- `ServiceController.cs` Lines 171, 176, 252, 2072, 2077
- `ProductController.cs` Line 74
- `OrderRequestProductController.cs` Line 136
- `OrderRequestServiceController.cs` Line 131
- `MemberShipDiscountCategoryController.cs` Line 51
- `PackageController.cs` Lines 51, 56
- `FeatureCategoryController.cs` Line 24

### 5D: Inline FeatureCategory Lookups in Service Code
- **File:** `Service/CategoryAccess.cs` Lines 65-67 — Queries FeatureCategory table inside category tree building
- **File:** `Service/BrandAccess.cs` Lines 50-51 — Queries FeatureCategory to filter brands

### Recommended Fix
- **Strategy:** `MemoryCache` with keys `"FeatureCategories_All"`, `"FeatureCategories_{key}"`
- **Duration:** 1 hour (invalidate on admin FeatureCategory CRUD)
- **Where:** Cache in `FeatureCategoryAccess.GetAll()` and `GetAllByFCatagoryKey()`

---

## FINDING 6: Brand Lists — Slow-Changing Reference Data

**Severity:** 🟠 HIGH

### 6A: API Endpoint
- **File:** `Service/WebAPI/BrandServiceAccess.cs` Lines 26-86
- **Controller:** `Controllers/BrandController.cs` Lines 22-33 (`GetBrandAll`)
- **What:** `SELECT * FROM Brands WHERE Status = 'Active'` with various filters
- **Change frequency:** **Daily/Weekly** — brands change infrequently

### 6B: Admin Service Layer
- **File:** `Service/BrandAccess.cs` Lines 35-44 (`GetAll`)
- **File:** `Service/BrandAccess.cs` Lines 46-67 (`GetAllByFeatureCategory`)

### Recommended Fix
- **Strategy:** `MemoryCache` with keys `"Brands_All_{featureCategoryId}_{type}"`, `"Brands_ByFeatureCategory_{key}"`
- **Duration:** 15-30 minutes (invalidate on brand CRUD)
- **Where:** Cache in `BrandAccess.GetAll()` and `BrandServiceAccess.GetAll()`

---

## FINDING 7: Category Trees — Complex & Expensive Reference Data

**Severity:** 🔴 CRITICAL — Category tree building involves recursive in-memory processing

### 7A: API Endpoint
- **File:** `Service/WebAPI/CategoryServiceAccess.cs` Lines 71-128 (`GetAll`)
- **Controller:** `Controllers/CategoryController.cs` Lines 23-32 (`GetAll`)
- **What:** Full `SELECT * FROM Categories WHERE Status = 'Active'` then recursive in-memory tree build
- **Change frequency:** **Weekly** — categories are admin-managed, rarely change

### 7B: Parent-Child Tree Building
- **File:** `Service/CategoryAccess.cs` Lines 28-53 (`GetParentChildWiseCategories`)
- **File:** `Service/CategoryAccess.cs` Lines 54-99 (`GetParentChildWiseFCategories`)
- **What:** CPU-intensive recursive tree assembly on every call

### 7C: Admin Dropdown Population (20+ occurrences)
- Every admin page that shows category dropdowns re-queries and re-builds the tree
- ServiceTypeController, ServiceController, WebHtmlController, ProductController all call `_categoryAccess.GetAllByFeatureCategory()`

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"CategoryTree_{featureCategoryId}_{lang}"`
- **Duration:** 30 minutes (invalidate on category CRUD)
- **Where:** Cache the assembled tree result in `CategoryServiceAccess.GetAll()`
- **Estimated saving:** Eliminates expensive tree assembly + DB query per call

---

## FINDING 8: ProductType List — Static Lookup Data

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/ProductTypeService.cs` Lines 24-47
- **Controller:** `Controllers/ProductTypeController.cs` Lines 22-33 (`GetAllProductTypes`)
- **What:** `SELECT * FROM ProductTypeMaster WHERE Status = 'Active'`
- **Change frequency:** **Very rarely** — product types are system config

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"ProductTypes_Active_{lang}"`
- **Duration:** 1 hour

---

## FINDING 9: PaymentMethod List — Static Lookup Data

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/PaymentMethodServiceAccess.cs` Lines 22-36
- **Controller:** `Controllers/PaymentMethodController.cs` Lines 22-33 (`GetAllPaymentMethod`)
- **What:** `SELECT * FROM PaymentMethods WHERE Status = 'Active'`
- **Change frequency:** **Very rarely**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"PaymentMethods_Active_{lang}"`
- **Duration:** 1 hour

---

## FINDING 10: Airport List — Static Reference Data

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/AirportAccess.cs` Lines 24-31
- **Controller:** `Controllers/AirportController.cs` Lines 22-33 (`GetAllAirports`)
- **What:** `SELECT * FROM Airports INNER JOIN Cities WHERE Status = 'Active'`
- **Change frequency:** **Rarely**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"Airports_Active"`
- **Duration:** 1 hour

---

## FINDING 11: FAQ List — Slow-Changing Content

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/FAQService.cs` Lines 52-64
- **Controller:** `Controllers/FAQController.cs` Lines 24-40 (`GetAlFAQ`)
- **What:** `SELECT * FROM FaqServices WHERE Status = 'Active' AND FeatureTypeId = @id`
- **Change frequency:** **Weekly/Monthly**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"FAQ_{featureCategoryId}"`
- **Duration:** 30 minutes

---

## FINDING 12: DeliverySetting — Per-FeatureCategory Config

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/DeliverySettingsServiceAccess.cs` Lines 22-32
- **Controller:** `Controllers/DeliverySettingController.cs` Lines 22-33
- **What:** Single-row query per feature category for delivery config
- **Change frequency:** **Rarely**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"DeliverySetting_{featureCategoryId}"`
- **Duration:** 30 minutes

---

## FINDING 13: WebHtml/Banner Content — CMS Content

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/WebHtmlServiceAccess.cs` Lines 27-77
- **Controller:** `Controllers/WebhtmlController.cs` Lines 22-32
- **What:** CMS banner/content queries with image URL processing
- **Change frequency:** **Weekly** — admin-managed content

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"WebHtml_{identifier}_{featureCategoryId}_{lang}"`
- **Duration:** 15 minutes

---

## FINDING 14: MemberShip/Subscription Details — Static Config

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/MembershipService.cs` Lines 22-46
- **Controller:** `Controllers/MembershipController.cs` Lines 22-28
- **What:** Queries MemberShip + MemberShipDiscountCategory tables
- **Change frequency:** **Rarely**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"Membership_Active_{lang}"`
- **Duration:** 30 minutes

---

## FINDING 15: CommunitySetup — MonthlyGoals + FeatureCategories

**Severity:** 🟡 MEDIUM

- **File:** `Service/WebAPI/CommunitySetupAccess.cs` Lines 26-35
- **Controller:** `Controllers/CommunitySetupController.cs` Lines 28-42
- **What:** Loads `MonthlyGoals` + `FeatureCategories` tables (raw DbContext, bypasses UoW)
- **Change frequency:** **Very rarely**

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"CommunitySetup"`
- **Duration:** 1 hour

---

## FINDING 16: Role List — Admin Dropdown

**Severity:** 🟢 LOW

- **File:** `Service/RoleAccess.cs` Lines 32-42
- **Admin:** `Areas/Admin/Controllers/UserController.cs` Line 35 (`SelectList(await _roleAccess.GetAll()...)`)
- **What:** `SELECT * FROM Roles WHERE Status = 'Active'`
- **Change frequency:** **Very rarely** — roles never change

### Recommended Fix
- **Strategy:** `MemoryCache` with key `"Roles_Active"`
- **Duration:** 1 hour

---

## FINDING 17: Dashboard — Repeated Repository Calls with No Caching

**Severity:** 🟠 HIGH

- **File:** `Service/Admin/DashboardDataAccess.cs` Lines 28-87
- **Controller:** `Areas/Admin/Controllers/DashboardController.cs` Lines 28-38

### Specific Issues:
1. **Lines 38-42:** 5 separate aggregate queries (COUNT, SUM) — each a full table scan
2. **Lines 44-56:** Full `FeatureCategories` load + loop with per-category COUNT queries (N+1 pattern)
3. **Lines 59-74:** 8 more aggregate queries with redundant `.AnyAsync()` checks before each `.CountAsync()` / `.SumAsync()`

### Chart Data Endpoints (4 endpoints, each loads entire table):
- **Lines 57-72 (DashboardController):** `LastMonthProductOrderstData` — `ToListAsync()` on ALL product orders, then filters in-memory
- **Lines 74-89:** `LastMonthProductSalesData` — same full table load
- **Lines 91-106:** `LastMonthServiceOrdersData` — `ToListAsync()` on ALL service orders
- **Lines 108-123:** `LastMonthServiceSalesData` — same full table load

### Recommended Fix
- **Strategy:** `MemoryCache` for dashboard aggregate data
- **Duration:** 5-10 minutes (dashboard stats don't need to be realtime)
- **Key:** `"Dashboard_Aggregates"`, `"Dashboard_Chart_ProductOrders"`, etc.

---

## FINDING 18: Missing `[OutputCache]` on ALL API Endpoints

**Severity:** 🔴 CRITICAL

None of the WebAPI controllers use `[OutputCache]`. For static-data endpoints, HTTP-level caching would prevent the server from even executing the action method.

### Endpoints that should have OutputCache:

| Controller | Action | Recommended Duration |
|---|---|---|
| `CountryController` | `GetAllCountries` | 3600s (1 hr) |
| `CityController` | `GetAllCities` | 3600s |
| `CityController` | `GetCitiesByCountryId` | 3600s, VaryByParam="countryId,lang" |
| `FeatureCategoryController` | `GetAllFeatureCategory` | 3600s, VaryByParam="lang" |
| `BrandController` | `GetBrandAll` | 900s (15 min) |
| `CategoryController` | `GetAll` | 1800s, VaryByParam="*" |
| `FAQController` | `GetAlFAQ` | 1800s, VaryByParam="featurecategoryId" |
| `ProductTypeController` | `GetAllProductTypes` | 3600s, VaryByParam="lang" |
| `PaymentMethodController` | `GetAllPaymentMethod` | 3600s, VaryByParam="lang" |
| `AirportController` | `GetAllAirports` | 3600s |
| `DeliverySettingController` | `GetDeliverySettings` | 1800s |
| `MembershipController` | `GetSubscription` | 1800s, VaryByParam="lang" |

> **Note:** These are WebAPI endpoints (inherit `ApiController`), so use WebAPI-style caching (custom `ActionFilterAttribute` or `MemoryCache` in service layer) since `[OutputCache]` is MVC-only.

---

## FINDING 19: Web.config Compilation Settings

**Severity:** 🔴 CRITICAL for Production

### Current Setting (Web.config Line 38):
```xml
<compilation debug="true" targetFramework="4.7.2" />
```

### Issues:
1. **`debug="true"`** — Has multiple performance impacts:
   - Razor views are compiled one-at-a-time (no batch compilation)
   - Compiled view assemblies are not cached optimally
   - Bundle minification is disabled
   - Request timeouts are extended (hides performance issues)
   - Additional debug metadata generated

2. **No `optimizeCompilations`** — Not configured at all
3. **No `batch` attribute** — Not configured
4. **No `numRecompilesBeforeAppRestart`** — Not configured

### Recommended Production Web.config:
```xml
<compilation debug="false" targetFramework="4.7.2" 
             optimizeCompilations="true"
             batch="true" 
             batchTimeout="30"
             numRecompilesBeforeAppRestart="50" />
```

### No OutputCache Profiles Defined
```xml
<!-- MISSING from Web.config — should add: -->
<system.web>
  <caching>
    <outputCacheSettings>
      <outputCacheProfiles>
        <add name="StaticData" duration="3600" varyByParam="lang" />
        <add name="SlowChanging" duration="900" varyByParam="*" />
      </outputCacheProfiles>
    </outputCacheSettings>
  </caching>
</system.web>
```

---

## ASP.NET Razor View Compilation (PHP OPcache Equivalent)

### How ASP.NET MVC Compiles Razor Views

ASP.NET MVC's Razor engine has a **built-in equivalent to PHP's OPcache** — it compiles `.cshtml` files to `.dll` assemblies automatically:

| Concept | PHP | ASP.NET MVC 5 |
|---|---|---|
| Template language | PHP | Razor (.cshtml) |
| Opcache/Bytecode cache | OPcache | Razor view compilation to DLL |
| Compiled output | PHP opcodes in shared memory | .NET IL in temporary DLLs |
| Compile location | PHP OPcache shared memory | `Temporary ASP.NET Files` folder |
| First-request penalty | Yes (if OPcache cold) | Yes (view compilation on first hit) |
| Recompilation trigger | File modification time | File modification time |

### The Compilation Flow:
```
Request → .cshtml file → C# code generation → Roslyn compiler → .dll assembly → Execution
           (only on first request or after file change)
```

### `compilation debug="true"` Impact

When `debug="true"` (current Boulevard setting):

| Behavior | debug="true" (CURRENT) | debug="false" |
|---|---|---|
| View batch compilation | ❌ One view at a time | ✅ Batch compiles all views in a folder |
| Memory usage | Higher (debug symbols) | Lower |
| Bundle minification | ❌ Disabled | ✅ Enabled |
| First-request speed | Slower | Faster (batch compilation) |
| Compiled assembly caching | Less aggressive | More aggressive |
| Browser caching headers | ❌ Not set | ✅ Set properly |

### `optimizeCompilations` Setting

```xml
<compilation optimizeCompilations="true" />
```

When `true`:
- ASP.NET only recompiles views that have **actually changed** (checks timestamps)
- Without this, ASP.NET may recompile the entire application when any top-level file changes
- Prevents unnecessary full-app recompilations when a single `.cshtml` is modified
- **Critical for production** — prevents cascading recompilation storms

### Batch Compilation

```xml
<compilation batch="true" batchTimeout="30" />
```

- `batch="true"` — Compiles all views in the same directory together into one DLL
- Reduces the number of compiled assemblies
- First request to a directory compiles ALL views in that directory at once
- Subsequent requests to other views in the same directory are instant

### `numRecompilesBeforeAppRestart`

```xml
<compilation numRecompilesBeforeAppRestart="50" />
```

- Controls how many view recompilations can happen before the AppDomain recycles
- Default is 15 — in a large app with active development, this can cause frequent restarts
- Set to 50+ in production

### Precompilation Alternative

For maximum performance, precompile views at build time:
```
aspnet_compiler -v / -p "C:\path\to\Boulevard" -f "C:\path\to\output"
```
Or add to `.csproj`:
```xml
<MvcBuildViews>true</MvcBuildViews>
```
This eliminates ALL first-request compilation penalties.

---

## Priority Implementation Plan

### Phase 1: Immediate Wins (Highest ROI)

| # | Finding | Fix | Impact |
|---|---|---|---|
| 1 | Triple LayoutSetting calls | `MemoryCache` in `GetDefaultLayout()`, 30 min TTL | Eliminates 3 DB queries/page |
| 2 | AdminNotification in view | Move to AJAX or cache 30s | Eliminates blocking DB call in view |
| 19 | `debug="true"` | Set to `false` for production | View caching, bundling, performance |

### Phase 2: Static Reference Data Caching

| # | Finding | Fix | Estimated Saving |
|---|---|---|---|
| 3 | Countries | Cache 1 hr | ~12 admin pages + API |
| 4 | Cities | Cache 1 hr | ~14 admin pages + API |
| 5 | FeatureCategories | Cache 1 hr | ~19+ admin pages + API |
| 8 | ProductTypes | Cache 1 hr | API |
| 9 | PaymentMethods | Cache 1 hr | API |
| 10 | Airports | Cache 1 hr | API |
| 16 | Roles | Cache 1 hr | Admin dropdown |

### Phase 3: Computed/Assembled Data Caching

| # | Finding | Fix | Estimated Saving |
|---|---|---|---|
| 6 | Brands | Cache 15-30 min | API + admin |
| 7 | Category Trees | Cache 30 min | Eliminates expensive tree rebuilds |
| 11 | FAQs | Cache 30 min | API |
| 13 | WebHtml/Banners | Cache 15 min | API |
| 14 | Membership | Cache 30 min | API |
| 15 | CommunitySetup | Cache 1 hr | API |

### Phase 4: Dashboard & Analytics Caching

| # | Finding | Fix | Estimated Saving |
|---|---|---|---|
| 17 | Dashboard aggregates | Cache 5-10 min | Eliminates 15+ DB queries per load |
| 17 | Chart data endpoints | Cache 5 min | Eliminates 4 full-table loads |

---

## Recommended Cache Infrastructure

### For ASP.NET MVC 5, use `System.Runtime.Caching.MemoryCache`:

```csharp
// Helper class to add to the project
using System;
using System.Runtime.Caching;

public static class CacheHelper
{
    private static readonly ObjectCache Cache = MemoryCache.Default;

    public static T GetOrSet<T>(string key, Func<T> factory, int durationMinutes = 30)
    {
        var cachedItem = Cache.Get(key);
        if (cachedItem != null)
            return (T)cachedItem;

        var result = factory();
        if (result != null)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(durationMinutes)
            };
            Cache.Set(key, result, policy);
        }
        return result;
    }

    public static void Invalidate(string key)
    {
        Cache.Remove(key);
    }

    public static void InvalidateByPrefix(string prefix)
    {
        var keysToRemove = Cache
            .Where(kvp => kvp.Key.StartsWith(prefix))
            .Select(kvp => kvp.Key)
            .ToList();
        foreach (var key in keysToRemove)
            Cache.Remove(key);
    }
}
```

### Example Usage (LayoutSettingAccess):

```csharp
public LayoutSetting GetDefaultLayout()
{
    return CacheHelper.GetOrSet("LayoutSetting_Default", () =>
    {
        var result = uow.LayoutSettingRepository.Get()
            .Where(e => e.IsDefault).FirstOrDefault();
        if (result != null)
            return result;
        return new LayoutSetting
        {
            Name = "Light", LogoHeader = "skin6",
            MainHeader = "skin6", Body = "Light", SideBar = "skin6"
        };
    }, durationMinutes: 30);
}

public void layoutUpdate()
{
    // ... existing update logic ...
    CacheHelper.Invalidate("LayoutSetting_Default"); // ← invalidate on change
}
```

---

## Summary Statistics

| Metric | Count |
|---|---|
| Total findings | 19 |
| Critical severity | 4 |
| High severity | 5 |
| Medium severity | 8 |
| Low severity | 2 |
| Total unnecessary DB queries per admin page | 5-7 (Layout×3 + Notifications + Dropdowns) |
| Admin controllers loading dropdown data from DB | 12+ controllers, 50+ locations |
| API endpoints with no caching | 12+ endpoints |
| Caching mechanisms currently in use | **Zero** |
| Web.config caching configuration | **None** |
