# Boulevard Admin – Deep Frontend Performance Audit Report

**Date:** March 12, 2026  
**Scope:** Areas/Admin/ — Controllers, Views, Static Assets, Data Loading Patterns

---

## 1. PAGINATION AUDIT

### 1.1 PaginatedList Implementation

**File:** `Areas/Admin/Pagination/PaginatedList.cs`

A `PaginatedList<T>` class exists but has a **critical flaw** on line 29:

```csharp
public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int total, int pageIndex, int pageSize)
{
    var items = await source.ToListAsync(); // ← LOADS ALL ITEMS into memory, defeats pagination purpose
    return new PaginatedList<T>(items, total, pageIndex, pageSize);
}
```

**Issue:** `source.ToListAsync()` materializes the ENTIRE queryable into memory. There is no `.Skip()` / `.Take()` applied. The pagination metadata (TotalPages, PageIndex) is cosmetic only — all rows are loaded from the database every time.

### 1.2 Controller-by-Controller Pagination Status

| # | Controller | Index/List Action(s) | Has Pagination? | Loads ALL Data? |
|---|-----------|---------------------|----------------|----------------|
| 1 | **ProductController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAllByFCatagoryKey()` returns `IEnumerable<Product>` |
| 2 | **ServiceController** | `Index(fCatagoryKey)`, `IndexForChildService`, `MotorServiceIndex` | ❌ NO | ✅ YES — `GetAllByFeatureCategory()` returns `IEnumerable<Service>` |
| 3 | **MemberController** | `Index(MemberViewModel)` | ⚠️ PARTIAL | Server-side search/paging via `MemberViewModel` but uses cookies for page size |
| 4 | **OrderRequestProductController** | `Index(tabId, fCatagoryKey)` | ❌ NO | ✅ YES — loads ALL orders then filters by status in-memory: `dataList.Where(a => a.OrderStatusId == OrderStatusId)` |
| 5 | **OrderRequestServiceController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAllByFeatureCategory()` returns `List<OrderRequestService>` |
| 6 | **BrandController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAllByFeatureCategory()` returns `IEnumerable<Brand>` |
| 7 | **CategoryController** | `Index(key, isChild, deleteRequest, fCatagoryKey)` | ❌ NO | ✅ YES — `GetAll()` / `GetAllByFeatureCategory()` |
| 8 | **FeatureCategoryController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` returns `IEnumerable<FeatureCategory>` |
| 9 | **ServiceTypeController** | `Index(fCatagoryKey)`, `IndexV2` | ❌ NO | ✅ YES — `GetAll()` / `GetAllServiceType()` |
| 10 | **UserController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` returns `IEnumerable<User>` |
| 11 | **RoleController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 12 | **CityController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 13 | **CountryController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 14 | **NotificationController** | `Index()` | ❌ NO | ✅ YES — `GetAdminNotificationList()` |
| 15 | **CustomerEnqueryController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 16 | **OfferController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAllByFeatureCategory()` + N+1 loop calling `GetOfferBannerById` per item |
| 17 | **PackageController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAllPackageByFeatureCategory()` |
| 18 | **MemberShipController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 19 | **MemberShipDiscountCategoryController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 20 | **MemberAddressController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 21 | **WebHtmlController** | `Index(fCatagoryKey)` | ❌ NO | ✅ YES — `GetAll()` / `GetAllWebHtml()` |
| 22 | **FaqServiceController** | `Index(fCatagoryKey)`, `IndexForAll()` | ❌ NO | ✅ YES |
| 23 | **AirportController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 24 | **ServiceAmenityController** | `Index(fCatagoryKey)`, `IndexForPackage` | ❌ NO | ✅ YES |
| 25 | **ServiceLandmarkController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 26 | **ServiceTypeAmenityController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 27 | **PropertyInformationController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 28 | **VehicalModelController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 29 | **OrderStatusController** | `Index()` | ❌ NO | ✅ YES — `GetAll()` |
| 30 | **UserReportController** | `GetReportandhelp()` | ❌ NO | ✅ YES — `GetuserReport()` |
| 31 | **DashboardController** | `Index()` | N/A (ViewModel) | ⚠️ Chart endpoints load ALL orders into memory |

**Summary: 29 out of 30 listing endpoints load ALL data without server-side pagination.** Only `MemberController` has partial pagination support.

### 1.3 Dashboard Controller – Full Table Scans

`DashboardController.cs` lines 63–130: Four chart endpoints each call `.ToListAsync()` on the full orders table, then iterate in-memory:

```csharp
// Line 69-70 (and similar at 84, 99, 114)
var invoice = await uow.OrderRequestProductRepository.Get().ToListAsync();
for (int i = 0; i < 12; i++) { ... invoice.Where(s => s.CreateDate.Month == i + 1) ... }
```

This loads ALL product orders (and ALL service orders) into memory 4 separate times via 4 AJAX calls on dashboard load.

### 1.4 OfferController – N+1 Query Problem

`OfferController.cs` lines 44-53:
```csharp
foreach (var data in list)
{
    var offerData = await _offerAccess.GetOfferBannerById(data.OfferInformationId); // N+1 query
    if (offerData != null) { data.BannerImage = offerData.BannerImage; }
}
```

Each offer triggers a separate DB query for the banner image.

---

## 2. VIEW PERFORMANCE

### 2.1 DbContext Instantiation in Views — CRITICAL

**File:** `Areas/Admin/Views/Shared/_Header.cshtml` line 9:
```razor
var db = new BoulevardDbContext();
```

The layout header (rendered on EVERY page) instantiates a DbContext directly in the Razor view. This is never disposed, creating potential connection pool exhaustion.

### 2.2 Service Layer Calls in Views — CRITICAL  

On EVERY page load, THREE separate service/DB calls happen from partial views:

| File | Line | Code | Impact |
|------|------|------|--------|
| `_Layout.cshtml` | 3-4 | `new LayoutSettingAccess()` → `GetDefaultLayout()` | DB call on every request |
| `_Header.cshtml` | 8-11 | `new LayoutSettingAccess()` → `GetDefaultLayout()` | **Duplicate** DB call |
| `_Header.cshtml` | 20 | `new AdminNotificationDataAccess().GetAdminNotificationwithout(10)` | DB call for notifications |
| `_Scripts.cshtml` | 5-7 | `new LayoutSettingAccess()` → `GetDefaultLayout()` | **Triplicate** DB call |

**Total: 3 redundant `GetDefaultLayout()` calls + 1 notification query per page load = 4 extra DB calls per request.**

### 2.3 DataTables Configuration — ALL Client-Side Processing

All tables use `id="zero_config"` initialized globally in `datatable-basic.init.js`:
```javascript
$('#zero_config').DataTable({
    "order": [[0, "desc"]]
});
```

**No `serverSide: true` is used anywhere.** All data is loaded into the DOM and processed client-side by DataTables. The only table with `deferRender: true` is `Notification/Index.cshtml`.

Tables using `zero_config` (client-side processing, ALL data in DOM):
- Airport/Index, Brand/Index, Category/Index, City/Index, CommonProductTag/Index
- Country/Index, CustomerEnquery/Index, FaqService/Index, FaqService/IndexForAll
- FeatureCategory/Index, Member/Details, MemberShip/Index, MemberShipDiscountCategory/Index
- Notification/Index, Offer/Index, OrderRequestProduct/Index, OrderRequestService/Index
- Package/Index, Product/Index, PropertyInformation/Index, Role/Index
- Service/Index, ServiceAmenity/Index, ServiceLandmark/Index, ServiceType/Index
- ServiceTypeAmenity/Index, User/Index, UserReport/GetReportandhelp, VehicalModel/Index
- WebHtml/Index + multiple test pages

**~35+ tables are all client-side processing — no server-side DataTables.**

### 2.4 Aggressive Polling — Every 5 Seconds on EVERY Page

**File:** `_Header.cshtml` lines 471-488:
```javascript
setInterval(function () {
    $.ajax({
        url: '@Url.Action("TriggerAction", "OrderRequestProduct", new { area = "Admin" })',
        type: 'POST',
        success: function (res) {
            if (res.success) { playAudio(); }
        }
    });
}, 5000); // Fires every 5 seconds on EVERY admin page
```

Additionally, lines 419-420:
```javascript
pollFunc(autoCall, 60000000, 10000); // Polls notifications every 10 seconds for ~16 hours
```

**Impact:** Every open admin tab fires 2 AJAX requests every 5-10 seconds — one for order trigger checking and one for notification polling. With 5 admin tabs open, that's 60+ requests/minute.

### 2.5 No Lazy Loading on Images

**Zero** `loading="lazy"` attributes found across ALL 276 `.cshtml` files in the Admin area. Product images, service images, dashboard icons — all load eagerly.

### 2.6 No AJAX Debouncing

**Zero** instances of `debounce` or `throttle` found in any view file. Search inputs submit immediately without delay. The `Member/Index.cshtml` search form is especially notable — it has an AJAX search function with no debouncing.

---

## 3. BAD DATA STRUCTURES IN VIEWS

### 3.1 Excessive ViewBag/ViewData Usage

Controllers heavily use ViewBag instead of strongly-typed ViewModels. Examples:

| Controller | ViewBag Keys Used |
|-----------|------------------|
| ProductController | `FCatagoryKey`, `FCatagoryName`, `featureCategory`, `brand`, `TagName`, `ProductType`, `FeatureCategoryKey` |
| ServiceController | `FCatagoryKey`, `FCatagoryName`, `featureCategory`, `Catagory`, `propertyType`, `FeatureCategoryKey` |
| OrderRequestProductController | `FCatagoryKey`, `FCatagoryName`, `OrderStatus`, `OrderStatusActiveId` |
| WebHtmlController | `FCatagoryKey`, `FCatagoryName`, `FeatureCategoryKey`, `fCategoryId`, `service`, `featureCategory`, `categoryId`, `brandId` |

Most controllers pass 5-8 ViewBag values that should be part of a ViewModel.

### 3.2 Views Receiving Unpaged Collections

| View | Model Type | Issue |
|------|-----------|-------|
| `Product/Index.cshtml` | `IEnumerable<Product>` | ALL products loaded |
| `Service/Index.cshtml` | `IEnumerable<Service>` | ALL services loaded |
| `Brand/Index.cshtml` | `IEnumerable<Brand>` | ALL brands loaded |
| `OrderRequestService/Index.cshtml` | `IEnumerable<OrderRequestService>` | ALL orders loaded |
| `ServiceType/Index.cshtml` | `IEnumerable<ServiceType>` | ALL service types loaded |
| `User/Index.cshtml` | `IEnumerable<User>` | ALL users loaded |
| `Notification/Index.cshtml` | List (all notifications) | ALL notifications loaded |
| `Offer/Index.cshtml` | `IEnumerable<OfferInformation>` | ALL offers loaded + N+1 |

### 3.3 Dropdown Data Loading Without Limits

Multiple controllers load ALL records for dropdowns:

| Controller | Method | What It Loads |
|-----------|--------|--------------|
| ServiceController | `Create()` | `GetAll()` for FeatureCategories, Countries, Categories |
| ServiceTypeController | `Create()` | `GetAllByFeatureCategory()` for Services, Categories |
| WebHtmlController | `CreateAndUpdate()` | `GetAll()` for Categories, Brands, Services |
| MemberAddressController | `CreateAndUpdate()` | `GetAll()` for Countries, Cities |
| OfferController | `Create()` | ALL Products, Brands, Categories, ServiceTypes per feature category |
| MemberShipDiscountCategoryController | `CreateAndUpdate()` | `GetAll()` for FeatureCategories, Memberships |
| VehicalModelController | `LoadDdl()` | All brands directly via DbContext |

---

## 4. STATIC ASSETS AUDIT

### 4.1 CSS Files Loaded Per Page (via _Head.cshtml)

Every admin page loads these CSS files (from `_Head.cshtml`):

1. `perfect-scrollbar.min.css`
2. `chartist.min.css`
3. `chartist-plugin-tooltip.css`
4. `jstree/style.min.css` (CDN)
5. `bootstrap-icons.min.css` (CDN)
6. `c3.min.css`
7. `select2.min.css`
8. `dropify.min.css`
9. `jquery-jvectormap.css`
10. `fullcalendar.min.css`
11. `dataTables.bootstrap4.css`
12. `sweetalert2.min.css`
13. `bootstrap-switch.min.css`
14. `custom_style.css`
15. `style.min.css`
16. `PagedList.css`

**Total: 16 CSS files on every page.** Many are only needed on specific pages (e.g., fullcalendar, jvectormap, chartist).

### 4.2 JS Files Loaded Per Page (via _Head.cshtml + _Scripts.cshtml)

From `_Head.cshtml`:
1. `jquery.min.js` (loaded in HEAD — render-blocking)

From `_Scripts.cshtml`:
2. `jquery.min.js` ← **DUPLICATE jQuery from MainContent folder**
3. `popper.min.js`
4. `bootstrap.min.js`
5. `app.min.js`
6. `app-style-switcher.js`
7. `perfect-scrollbar.jquery.min.js`
8. `sparkline.js`
9. `waves.js`
10. `sidebarmenu.js`
11. `feather.min.js`
12. `custom.min.js`
13. `chartist.min.js`
14. `chartist-plugin-tooltip.min.js`
15. `d3.min.js`
16. `c3.min.js`
17. `dropify.min.js`
18. `select2.min.js`
19. `dashboard2.js`
20. `jquery.dataTables.min.js`
21. `custom-datatable.js`
22. `datatable-basic.init.js`
23. `sweetalert2.min.js` (CDN)
24. `bootstrap-switch.min.js`
25. `custom_custom.js`
26. `tinymce.min.js`
27. `jstree.min.js` (CDN)

From `_Header.cshtml`:
28. `jquery.min.js` ← **THIRD jQuery load** (from Content/assets/plugins/)

**Total: 28 JS files, including 3 copies of jQuery loaded on every page.**

### 4.3 Render-Blocking Resources

**`_Head.cshtml` line 33:**
```html
<script src="~/Areas/Admin/Content/assets/libs/jquery/dist/jquery.min.js"></script>
```
jQuery loaded in `<head>` WITHOUT `async` or `defer` — blocks rendering of the entire page.

**No scripts anywhere use `async` or `defer` attributes.** Only one occurrence of `deferRender` exists (DataTables option in Notification/Index).

### 4.4 Duplicate/Multiple jQuery Loads

| Location | jQuery Version | Issue |
|----------|---------------|-------|
| `_Head.cshtml` line 33 | `jquery.min.js` (local) | Render-blocking in `<head>` |
| `_Scripts.cshtml` line 14 | `jquery.min.js` (MainContent) | Duplicate |
| `_Header.cshtml` line 407 | `jquery.min.js` (Content/assets/plugins) | Triplicate |
| Individual views (14+ files) | `http://code.jquery.com/jquery-1.7.1.min.js` | ANCIENT jQuery 1.7.1 loaded over HTTP (not HTTPS) |

Views loading jQuery 1.7.1 over insecure HTTP:
- `Member/Index.cshtml` line 37
- `OrderRequestProduct/Index.cshtml` line 12
- `OrderRequestService/Index.cshtml` line 7
- `Offer/Index.cshtml` line 13
- `WebHtml/Index.cshtml` line 7
- `City/Index.cshtml` line 6
- `MemberShipDiscountCategory/Index.cshtml` line 6
- `MemberShip/Index.cshtml` line 6
- `VehicalModel/Index.cshtml` line 6
- `UserReport/GetReportandhelp.cshtml` line 7
- `UserReport/ReportandhelpDetails.cshtml` line 7
- `WebHtml/WebHtmlTestIndex.cshtml` line 6
- `WebHtml/IndexMotorTest.cshtml` line 7

### 4.5 Unused Libraries Loaded on Every Page

| Library | Loaded In | Pages That Actually Use It |
|---------|-----------|---------------------------|
| `chartist.min.js` + CSS | `_Head.cshtml`, `_Scripts.cshtml` | Dashboard only |
| `d3.min.js` + `c3.min.js` + CSS | `_Scripts.cshtml` | Dashboard charts only |
| `dashboard2.js` | `_Scripts.cshtml` | Dashboard only |
| `fullcalendar.min.css` | `_Head.cshtml` | No calendar views exist |
| `jquery-jvectormap.css` | `_Head.cshtml` | No map views found |
| `sparkline.js` | `_Scripts.cshtml` | Not referenced in views |
| `tinymce.min.js` | `_Scripts.cshtml` | Only used in Create/Edit forms |
| `jstree.min.js` + CSS | `_Head.cshtml`, `_Scripts.cshtml` | Only Category/Product tree views |
| `bootstrap-switch.min.js` + CSS | `_Head.cshtml`, `_Scripts.cshtml` | A few toggle switches |
| `dropify.min.js` + CSS | `_Head.cshtml`, `_Scripts.cshtml` | Only file upload forms |
| `select2.min.js` + CSS | `_Head.cshtml`, `_Scripts.cshtml` | Only forms with select2 dropdowns |

### 4.6 CDN Resources Over Insecure HTTP

Multiple views load scripts over `http://` (not `https://`):
```html
<script type="text/javascript" src="http://code.jquery.com/jquery-1.7.1.min.js"></script>
```
This is both a **security risk** (mixed content) and a **performance issue** (no HTTP/2 benefits).

### 4.7 Dashboard Page – Extra CDN Resources

`Dashboard/Index.cshtml` loads additional CDN resources on top of the global ones:
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/chartist/0.11.0/chartist.min.css">
<script src="https://cdnjs.cloudflare.com/ajax/libs/chartist/0.11.0/chartist.min.js"></script>
<script src="https://unpkg.com/chartist-plugin-tooltips@0.0.17"></script>
<script src="https://unpkg.com/chartist-plugin-pointlabels@0.0.6"></script>
```
These **duplicate** the chartist already loaded in `_Head.cshtml` and `_Scripts.cshtml`.

---

## 5. ADMIN AREA REGISTRATION

**File:** `Areas/Admin/AdminAreaRegistration.cs`

The file defines extensive named route registrations. No performance issues in routing itself, but all routes map to controllers that load full datasets as documented above.

---

## 6. CRITICAL ISSUES SUMMARY (Ranked by Impact)

### 🔴 CRITICAL (Fix Immediately)

| # | Issue | Files | Impact |
|---|-------|-------|--------|
| 1 | **29/30 listing endpoints load ALL data without pagination** | All controllers | DB overwhelm, memory bloat, slow page loads |
| 2 | **3× LayoutSettingAccess DB calls per page** | `_Layout.cshtml:3`, `_Header.cshtml:8`, `_Scripts.cshtml:5` | 3 unnecessary DB queries per request |
| 3 | **DbContext in _Header.cshtml (line 9)** | `_Header.cshtml` | Connection pool leak, undisposed DbContext |
| 4 | **5-second setInterval on EVERY page** | `_Header.cshtml:471` | Server hammering (12 req/min per tab) |
| 5 | **3 copies of jQuery loaded per page** | `_Head.cshtml:33`, `_Scripts.cshtml:14`, `_Header.cshtml:407` | ~300KB wasted, JS conflicts |
| 6 | **DataTables ALL client-side processing** | All Index views (~35 tables) | Full DOM rendering, browser memory |
| 7 | **DashboardController loads ALL orders 4 times** | `DashboardController.cs:63-130` | Full table scan × 4 on dashboard |

### 🟠 HIGH (Fix Soon)

| # | Issue | Files | Impact |
|---|-------|-------|--------|
| 8 | **OfferController N+1 query** | `OfferController.cs:44-53` | 1 query per offer item in loop |
| 9 | **14+ views load jQuery 1.7.1 over HTTP** | Multiple Index views | Security (mixed content), outdated |
| 10 | **28 JS files loaded on every page** | `_Head.cshtml`, `_Scripts.cshtml` | ~2-3MB JS payload per page |
| 11 | **16 CSS files loaded on every page** | `_Head.cshtml` | Render-blocking, unused styles |
| 12 | **Zero lazy loading on images** | All 276 cshtml files | All images load eagerly |
| 13 | **No debouncing on AJAX search/input** | `Member/Index.cshtml` and others | Rapid-fire server requests |
| 14 | **Notification polling every 10 seconds** | `_Header.cshtml:419` | Additional constant server load |

### 🟡 MEDIUM (Improve)

| # | Issue | Files | Impact |
|---|-------|-------|--------|
| 15 | **Unused libraries on every page** | Fullcalendar, jvectormap, sparkline, etc. | Wasted bandwidth |
| 16 | **Dashboard duplicates chartist from CDN** | `Dashboard/Index.cshtml:8-11` | Double-loading chartist |
| 17 | **No script async/defer anywhere** | All script tags | Render-blocking |
| 18 | **Excessive ViewBag usage** | All controllers | Maintainability, type safety |
| 19 | **PaginatedList.CreateAsync loads all items** | `PaginatedList.cs:29` | Defeats pagination purpose |
| 20 | **All dropdown data loaded without limits** | Create/Edit forms | Unnecessary full table loads |

---

## 7. ESTIMATED PERFORMANCE COST PER PAGE LOAD

For a typical admin Index page:

| Resource | Current | Optimal |
|----------|---------|---------|
| DB Queries | 5-8 (layout×3 + notifications + data + dropdown) | 1-2 |
| Data Rows Loaded | ALL (potentially thousands) | 10-25 per page |
| CSS Files | 16 | 4-5 (load others on demand) |
| JS Files | 28+ | 8-10 (bundle, lazy-load rest) |
| jQuery Copies | 3 | 1 |
| Background AJAX/sec | 2 (every 5-10s) | 0-1 (WebSocket or long-poll) |
| Lazy-Loaded Images | 0 | All below fold |

---

*End of Frontend Performance Audit Report*
