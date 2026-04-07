# Grocery Import System — Implementation Report

## Overview

This document records all changes made to fix the Excel bulk upload pipeline for Grocery and related feature categories, together with a guide for running the full import cycle.

---

## 1. Bugs Fixed

### C1 — Form Endpoint (Critical)
**File:** `Areas/Admin/Views/Product/AddBulk.cshtml`

The upload form was posting to a non-existent `FileImport/AddBulkProduct` controller/action pair. No data ever reached the server.

**Before:**
```csharp
@using (Html.BeginForm("AddBulkProduct", "FileImport", FormMethod.Post, ...))
```
**After:**
```csharp
@using (Html.BeginForm("AddBulk", "Product", new { area = "Admin" }, FormMethod.Post, ...))
```

---

### C2 — Ghost Object (Critical)
**File:** `Areas/Admin/Controllers/ProductController.cs`

The row-reading loop populated a shared outer `TempProduct product` variable but appended a separate inner `TempProduct data` (always empty/default) to the XML. Result: every row's XML contained only default/null field values.

**Fix:** Replaced the outer shared `product` variable with a per-row `TempProduct data` that is correctly populated in each loop iteration. The XML writer and counters now reference `data.*` instead of `product.*`. The unused `list`/`Duplicatelist` collections were removed and replaced with an integer counter `xmlRowCount`.

**Column name mismatches also fixed:**
| Header check used | Excel column name (actual) |
|---|---|
| `"images"` | `"Images"` |
| `"Stocks quantity"` | `"Stocks Quantity"` |

---

### C3 — Delimiter Support (High)
**File:** `Service/Admin/TempProductDataAccess.cs`

Multi-value columns (Selling Price, Quantity, Stocks) only split on `,`. The Excel template used by the client uses `;` as the delimiter.

**Fix:** Added a normalizing helper:
```csharp
static string[] SplitValues(string raw)
    => (raw ?? "").Replace(';', ',').Split(new[] { ',' }, StringSplitOptions.None);
```
All three multi-value columns now call `SplitValues()` instead of `.Split(',')`. Both `,` and `;` are accepted as delimiters interchangeably.

---

### C4 — NullReferenceException in Category Lookup (Critical)
**File:** `Service/Admin/TempProductDataAccess.cs`

Three related null-safety failures:
1. `SubCategory` DB lookup ran unconditionally even when `item.SubCategory` was empty, producing a `null` reference that crashed on the next line.
2. `SubSubCategory` lookup used `subcategory.CategoryId` without first checking that `subcategory != null`.
3. `Convert.ToInt32(item.Stocks)` crashed with a `FormatException` when Stocks contained a comma-separated list (e.g., `"10,20"`).

**Fixes:**
```csharp
// SubCategory — wrap in null/empty guard
Category subcategory = null;
if (!string.IsNullOrEmpty(item.SubCategory)) {
    subcategory = db.Categories.FirstOrDefault(...);
    if (subcategory == null) { /* create */ }
}

// SubSubCategory — guard on parent AND on own value
Category subSubcategory = null;
if (subcategory != null && !string.IsNullOrEmpty(item.SubSubCategory)) {
    subSubcategory = db.Categories.FirstOrDefault(...);
    if (subSubcategory == null) { /* create */ }
}

// Stocks — sum split integer parts
var stockParts = SplitValues(item.Stocks);
product.StockQuantity = stockParts
    .Where(s => !string.IsNullOrWhiteSpace(s) && int.TryParse(s.Trim(), out _))
    .Sum(s => int.Parse(s.Trim()));
```

Added array-bounds guard for Price/Qty/Stock parallel arrays:
```csharp
int priceRowCount = Math.Min(qtys.Length, Math.Min(Prices.Length, stockquantity.Length));
```

---

### C5 — No Duplicate Guard (High)
**File:** `Service/Admin/TempProductDataAccess.cs`

Products were inserted unconditionally. Re-running the import doubled every entry.

**Fix:**
```csharp
bool alreadyExists = db.Products.Any(p =>
    p.ProductName.Trim().ToLower() == item.ProductName.Trim().ToLower() &&
    p.FeatureCategoryId == feacherCategoryId &&
    p.Status == "Active");
if (alreadyExists) continue;
```

---

### SP Replacement — Missing Stored Procedure (Critical)
**File:** `Service/Admin/TempProductDataAccess.cs`

`AddTempProduct()` called stored procedure `pr_upload_bulk_product` via Dapper, but that procedure does not exist in the codebase or database. Stage 1 of the import (persisting the XML staging rows) silently failed on every upload.

**Fix:** Replaced the Dapper/SP call with a direct Entity Framework insert that parses the XML produced by the controller:
```csharp
var doc = XDocument.Parse(xmlFileData);
foreach (var elem in doc.Root.Elements("Product")) {
    var row = new TempProduct { TempId = Guid.NewGuid(), Brand = GetAttr("brand"), ... };
    db.TempProducts.Add(row);
}
await db.SaveChangesAsync();
```
All 28 XML attributes are mapped to `TempProduct` fields. `FeatureCategoryId` is set from the method parameter.

---

## 2. New Files Created

| File | Purpose |
|---|---|
| `seed_grocery.sql` | One-time insert of the Grocery `FeatureCategory` row with GUID `3b317e3f-cb2f-4fdd-b9c8-3f2186695771`. Uses `IF NOT EXISTS` guard; safe to re-run. |
| `delete_grocery_data.sql` | Cascading transactional delete of all grocery data — use before re-importing to start clean. |
| `docs/grocery-mobile.html` | Mobile-friendly, standalone HTML page that reads grocery categories and products from the live API. |

---

## 3. Import Workflow

Run these steps in order:

### Step 1 — Seed FeatureCategory (once)
```sql
-- In SSMS or any SQL client connected to the Boulevard database
:r seed_grocery.sql
```
This creates the `Grocery` feature category if it does not already exist.

### Step 2 — Clear Existing Grocery Data (if re-importing)
```sql
:r delete_grocery_data.sql
```
This removes all products, categories, brands, images, prices and staging rows tied to the Grocery feature category.

### Step 3 — Upload the Excel File
1. Navigate to `Admin → Grocery → Bulk Upload` (or use the URL `/Admin/Product/AddBulk?fCatagoryKey=3b317e3f-cb2f-4fdd-b9c8-3f2186695771`)
2. Select `docs/insert_grocery.xlsx`
3. Click **Upload**
4. Review the staging preview table
5. Click **Confirm / Save All**

### Step 4 — Verify
Run the verification queries below to confirm the import.

---

## 4. Verification Queries

```sql
-- Count imported products
SELECT COUNT(*)
FROM dbo.Products p
JOIN dbo.FeatureCategories fc ON fc.FeatureCategoryId = p.FeatureCategoryId
WHERE fc.FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
  AND p.Status = 'Active';

-- List top-level categories created
SELECT CategoryId, CategoryName, ParentId
FROM dbo.Categories c
JOIN dbo.FeatureCategories fc ON fc.FeatureCategoryId = c.FeatureCategoryId
WHERE fc.FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
  AND c.ParentId = 0
ORDER BY c.CategoryId;

-- Sample products with prices
SELECT TOP 20
  p.ProductId, p.ProductName,
  pp.Quantity, pp.SellingPrice, pp.StockQuantity
FROM dbo.Products p
JOIN dbo.FeatureCategories fc ON fc.FeatureCategoryId = p.FeatureCategoryId
LEFT JOIN dbo.ProductPrices pp ON pp.ProductId = p.ProductId AND pp.Status = 'Active'
WHERE fc.FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
  AND p.Status = 'Active'
ORDER BY p.ProductId DESC;

-- Check no rows stuck in staging
SELECT COUNT(*) AS StagingRowsLeft FROM dbo.TempProducts
WHERE FeatureCategoryId = (
  SELECT FeatureCategoryId FROM dbo.FeatureCategories
  WHERE FeatureCategoryKey = '3b317e3f-cb2f-4fdd-b9c8-3f2186695771'
);
```

---

## 5. Mobile HTML Page

**File:** `docs/grocery-mobile.html`

Open this file directly in a browser while the Boulevard API server is running.

### Features
- Resolves the Grocery `FeatureCategoryId` at runtime via `GET /api/v1/GetAllfeatureCategory`
- Displays all parent categories in a 3-column responsive card grid with emoji fallback icons
- Category card tap loads products via `GET /api/v1/general/GetSingelCategorywiseProduct`
- Product cards show: image, brand, name, size/attribute, AED price, discount badge
- Paginated "Load More" (20 products per page)
- Configurable API base URL (stored in `localStorage`) via the ⚙ API button

### API Endpoints Used
| Purpose | Endpoint |
|---|---|
| Find Grocery fcId | `GET /api/v1/GetAllfeatureCategory` |
| Load parent categories | `GET /api/v1/general/ParentcategoriesWiseproduct?featureCategoryid={id}&size=20&count=0` |
| Load products by category | `GET /api/v1/general/GetSingelCategorywiseProduct?categoryId={id}&size=20&count={page}` |

---

## 6. Excel Template Format

The upload expects an `.xlsx` file with the following columns (row 1 = headers):

| Column | Notes |
|---|---|
| `Product Name` | Required |
| `Description` | Optional |
| `Brand` | Matched or created |
| `Category` | Level-1 category name |
| `Sub Category` | Level-2 category name (optional) |
| `Sub Sub Category` | Level-3 category name (optional) |
| `Selling Price` | Comma or semicolon separated for multiple variants |
| `Quantity` | Comma or semicolon separated (matches Price order) |
| `Stocks Quantity` | Comma or semicolon separated (matches Price order) |
| `Images` | Image filename or URL |
| `Product Type` | `1` = regular, `3` = scheduled |
| `Tags` | Comma-separated tag list |
| `Unit` | e.g. `kg`, `pcs` |
| … | All other TempProduct fields |

Multi-value delimiter: **both `,` and `;` are accepted** (e.g., `"100,200"` or `"100;200"`).

---

*Report generated after implementing all 9 fixes across 3 files.*
