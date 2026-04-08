# Photography Excel Upload — Bug Fixes Report

**Date:** April 8, 2026  
**File:** `Medical & Motors - Photography.csv`  
**Section:** Photography — Admin Bulk Upload Portal  

---

## Summary

Six bugs were discovered and fixed that together would have made it **completely impossible** to upload the Photography data file through the admin portal. All fixes have been applied.

---

## Bug #1 — CRITICAL: Photography Section Not Registered in Bulk Upload Controller

**File:** `Areas/Admin/Controllers/ServiceController.cs`  
**Location:** `AddServiceBulk` action, line ~739  

### Problem
The `AddServiceBulk` controller action uses an `if` condition to select the correct column-mapping branch based on the feature category GUID. The Photography GUID (`4d5e6f7a-8b9c-0d1e-f234-5678901abcde`) was **completely missing** from this condition. Uploading a Photography file would silently fall through without any processing.

The condition only covered:
- `b3e3e680-c8ef-4ab2-a4ac-d75bb48a3647` → Motors
- `25d8c418-2d26-4159-9d7f-970e3b933b42` → Salon
- `bbc98e2d-941b-44c6-8122-0e12a2645b87` → Medical

### Fix
Added the Photography GUID to the `if` condition:

```csharp
// Before
if (model.fCatagoryKey == "B3E3E680-...".ToLower() || ... || model.fCatagoryKey == "BBC98E2D-...".ToLower())

// After
if (model.fCatagoryKey == "B3E3E680-...".ToLower() || ... || model.fCatagoryKey == "BBC98E2D-...".ToLower()
    || model.fCatagoryKey == "4D5E6F7A-8B9C-0D1E-F234-5678901ABCDE".ToLower())
```

---

## Bug #2 — CRITICAL: `CheckOutTime` Overwritten by `CheckInTime` (Service Close)

**File:** `Areas/Admin/Controllers/ServiceController.cs`  
**Location:** Lines 769–770 (validation section) and Lines 839–840 (data-loop section)  

### Problem
Two consecutive assignments both wrote to `CheckInTime`, so `Service Close` time was overwriting `Service Open` time. `CheckOutTime` was never set and always remained `null`/empty in the database.

```csharp
// Before (BUG — two lines both assign CheckInTime)
tempService.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
tempService.CheckInTime = dataTable.Rows[0]["Service Close"].ToString();  // ← overwrites!
```

### Fix
Changed the second assignment to `CheckOutTime`:

```csharp
// After
tempService.CheckInTime  = dataTable.Rows[0]["Service Open"].ToString();
tempService.CheckOutTime = dataTable.Rows[0]["Service Close"].ToString();  // ← fixed

// Same fix applied in the data loop:
data.CheckInTime  = objDataRow["Service Open"].ToString();
data.CheckOutTime = objDataRow["Service Close"].ToString();  // ← fixed
```

---

## Bug #3 — CRITICAL: Sidebar Missing "Photography Service List" and "Photography Bulk Upload" Links

**File:** `Areas/Admin/Views/Shared/_Sidebar.cshtml`  
**Location:** Photography section, lines ~557–578  

### Problem
The Photography section in the admin sidebar had no link to the Service List page and no direct Bulk Upload link. Since the Bulk Upload button is only accessible from the Service List page, there was **no way to reach the bulk upload portal for Photography** from the admin sidebar.

### Fix
Added two new sidebar links inside the Photography section:

```html
<li class="sidebar-item">
    <a href="...Admin_ServiceList...fCatagoryKey=4d5e6f7a-8b9c-0d1e-f234-5678901abcde...">
        Photography Service List
    </a>
</li>
<li class="sidebar-item">
    <a href="...Admin_AddServiceBulk...fCatagoryKey=4d5e6f7a-8b9c-0d1e-f234-5678901abcde...">
        Photography Bulk Upload
    </a>
</li>
```

These were inserted before the existing "Photography Service Type" link.

---

## Bug #4 — CRITICAL: 14 Mismatched Column Names in CSV File

**File:** `docs/Medical & Motors - Photography.csv`  

### Problem
The CSV file had 14 column headers that did **not match** what `ServiceController.cs` expects. When the controller tries to access a DataRow column by name that doesn't exist, it throws `System.ArgumentException: Column 'X' does not belong to table`. This would crash the upload for every single row.

### Mismatched Columns (Before → After)

| # | CSV Header (Wrong) | Expected by Controller (Correct) |
|---|---|---|
| 1 | `Photography Company Name` | `Company Name` |
| 2 | `Photography Company Name Arabic` | `Company Name Arabic` |
| 3 | `SubCategory Name` | `SubCategory` |
| 4 | `SubCategory name Arabic` | `SubCategory Arabic` |
| 5 | `Sub category image` | `SubCategory Image` |
| 6 | `Language` | `Languages` |
| 7 | `Latitude` | `Latitute` *(matches controller's spelling)* |
| 8 | `Longitude` | `Longitute` *(matches controller's spelling)* |
| 9 | `Service open` | `Service Open` |
| 10 | `Service close` | `Service Close` |
| 11 | `Photography Service Type Name` | `Service Type Name` |
| 12 | `Laundry Service Type Name Arabic` | `Service Type Name Arabic` *(was using Laundry label!)* |
| 13 | `Service hour` | `ServiceHour` |
| 14 | `Service minutes` | `Service Min` |
| 15 | `Service Type Big Details Html` | `Service Type Big Details` |
| 16 | `Service Type Big Details Arabic Html` | `Service Type Big Details Arabic` |

> **Note:** Column 12 (`Laundry Service Type Name Arabic`) appears to be a copy-paste error from the Laundry template — particularly serious because it would cause the Arabic service type name to be lost entirely.

### Fix
The first line (header row) of the CSV was updated in-place with all corrected column names. The data rows were not changed.

---

## Bug #5 — IMPORTANT: CSV File Extension Not Accepted by Upload Portal

**File:** `docs/Medical & Motors - Photography.csv` → converted to `docs/Medical & Motors - Photography.xlsx`  

### Problem
The upload controller only accepts `.xls` and `.xlsx` files:

```csharp
if (fileExtension == ".xls" || fileExtension == ".xlsx")
{
    // process...
}
// else → redirects back with "Invalid File Format" error
```

A `.csv` file would be rejected immediately with the message:  
*"Invalid File Formate. Please Upload .xls or .xlsx format."*

### Fix
Converted the corrected CSV into a proper `.xlsx` workbook using EPPlus:
- **Output file:** `docs/Medical & Motors - Photography.xlsx`
- **Sheet name:** `Photography`
- **Rows:** 64 (1 header + 63 data rows)
- **Columns:** 30
- **File size:** ~93 KB

The original `.csv` file is also kept (with corrected headers) as a source reference.

---

## Files Modified

| File | Change |
|---|---|
| `Areas/Admin/Controllers/ServiceController.cs` | Added Photography GUID to if-condition; fixed CheckInTime→CheckOutTime bug (2 locations) |
| `Areas/Admin/Views/Shared/_Sidebar.cshtml` | Added "Photography Service List" and "Photography Bulk Upload" links |
| `docs/Medical & Motors - Photography.csv` | Fixed 16 column header mismatches in header row |
| `docs/Medical & Motors - Photography.xlsx` | **New file** — ready-to-upload XLSX version (63 data rows, 30 columns) |

---

## How to Upload

1. Open the admin panel and navigate to **Photography → Photography Bulk Upload** (newly added to sidebar).
2. On the bulk upload page, upload the file: `docs/Medical & Motors - Photography.xlsx`
3. The system will preview the records in the temp table.
4. Click **"Update All to Service"** to move all records to the live database.

---

## Notes

- The `Latitute`/`Longitute` spelling in column headers follows the controller's own spelling (typos preserved by design to match the controller's `DataRow[...]` lookups).
- `CheckInTime`/`CheckOutTime` fix (Bug #2) applies to all services that use this same `if` branch (Motors, Salon, Medical, and now Photography).
- All 63 data rows contain complete HTML content for both English and Arabic descriptions.
