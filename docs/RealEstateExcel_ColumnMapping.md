# تحليل ملف Excel: RealStateServiceEntry 1.xlsx
## كل عمود → الجدول الذي يُكتب فيه + القسم في Sidebar + الأخطاء المنطقية

---

## مسار رفع البيانات (Flow)

```
رفع Excel
    ↓
ServiceController → AddTempService (Stored Procedure)
    ↓
جدول TempServices (مرحلي / Staging)
    ↓
الضغط على "Save All" → UpdateAllTemptoService → AddService (EF Code)
    ↓
الجداول الفعلية (Services, ServiceTypes, PropertyInformations, ...)
```

---

## مخطط الأعمدة الكامل

| # | عمود في Excel | TempService Property | الجدول المستهدف | الحقل في الجدول | قسم Sidebar |
|---|--------------|---------------------|-----------------|-----------------|-------------|
| 1 | `Sr.No` | `SlNo` | `TempServices.SlNo` | SlNo | — |
| 2 | `RealState Name` | `Name` | `Services.Name` | Name | **Real Estates** (الاسم الرئيسي) |
| 3 | `RealState Name Arabic` | `NameAr` | `Services.NameAr` | NameAr | **Real Estates** |
| 4 | `Description-html` | `Description` | `Services.Description` | Description | **Real Estates** (صفحة تفاصيل الخدمة) |
| 5 | `Description Arabic-html` | `DescriptionAr` | `Services.DescriptionAr` | DescriptionAr | **Real Estates** |
| 6 | `AboutUs` | `AboutUs` | `Services.AboutUs` | AboutUs | **Real Estates** |
| 7 | `About us Arabic` | `AboutUsAr` | `Services.AboutUsAr` | AboutUsAr | **Real Estates** |
| 8 | `Languages` | `Languages` | `Services.SpokenLanguages` | SpokenLanguages | **Real Estates** |
| 9 | `Latest Project` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 10 | `Service Type Name` | `ServiceTypeName` | `ServiceTypes.ServiceTypeName` | ServiceTypeName | **Real Estates Type** |
| 11 | `Service Type Name Arabic` | `ServiceTypeNameAr` | `ServiceTypes.ServiceTypeNameAr` | ServiceTypeNameAr | **Real Estates Type** |
| 12 | `Country Name` | `Country` | `Countries.CountryName` → `Services.CountryId` | CountryId | **Real Estates** |
| 13 | `City Name` | `City` | `Cities.CityName` → `Services.CityId` | CityId | **Real Estates** |
| 14 | `Address` | `Address` | `Services.Address` | Address | **Real Estates** |
| 15 | `Latitute` | `Latitute` | `Services.Latitute` | Latitute | **Real Estates** |
| 16 | `Longitute` | `Longitute` | `Services.Logitute` | Logitute | **Real Estates** |
| 17 | `PersoneQuantity` | `PersoneQuantity` | `ServiceTypes.PersoneQuantity` | PersoneQuantity | **Real Estates Type** |
| 18 | `Bed Quantity` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 19 | `Bathroom Quantity` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 20 | `Description` | `TypeDescription` | `ServiceTypes.Description` | Description | **Real Estates Type** |
| 21 | `Description Arabic` | `TypeDescriptionAr` | `ServiceTypes.DescriptionAr` | DescriptionAr | **Real Estates Type** |
| 22 | `Service Type AboutUs` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 23 | `Service Type AboutUs Arabic` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 24 | `Size` | `Size` | `ServiceTypes.Size` | Size | **Real Estates Type** |
| 25 | `Size Arabic` | `SizeAr` | `TempServices.SizeAr` فقط ⚠️ | **لا يُكتب** في ServiceTypes | **Real Estates Type** |
| 26 | `Images` | `Images` + `TypeImage` | `ServiceImages` + `ServiceTypes.Image` + `ServiceTypeFiles` | Image/FileLocation | **Real Estates** و **Real Estates Type** |
| 27 | `Price` | `TypePrice` | `ServiceTypes.Price` | Price | **Real Estates Type** |
| 28 | `Price Trends` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 29 | `Payment Type` | `PaymentType` | `ServiceTypes.PaymentType` | PaymentType | **Real Estates Type** |
| 30 | `Service Type Category` | `ServiceTypeCategory` | `Categories.CategoryName` → `ServiceCategories` | CategoryName | **Real Estates Catagory** |
| 31 | `Service Type Sub Category` | `ServiceTypeSubCategory` | `Categories.CategoryName` (ParentId) → `ServiceCategories` | CategoryName | **Real Estates Catagory** |
| 32 | `Property Type` | `PropertyType` | `TempServices.PropertyType` → ⚠️ لا يُكتب في PropertyInformations | Type | **Property Information** |
| 33 | `Property Type Arabic` | `PropertyTypeArabic` | `TempServices.PropertyTypeArabic` → ⚠️ لا يُكتب | TypeAr | **Property Information** |
| 34 | `Ref No` | `PropertyRefNo` | `TempServices.PropertyRefNo` → ⚠️ لا يُكتب | RefNo | **Property Information** |
| 35 | `Property Purpose` | `PropertyPurpose` | `TempServices.PropertyPurpose` → ⚠️ لا يُكتب | Purpose | **Property Information** |
| 36 | `Property Purpose Arabic` | ❌ لا يوجد في TempService | **لا يُقرأ** | PurposeAr | **Property Information** |
| 37 | `Furnishing` | `Furnishing` | `TempServices.Furnishing` → ⚠️ لا يُكتب | Furnishing | **Property Information** |
| 38 | `Furnishing Arabic` | `FurnishingArabic` | `TempServices.FurnishingArabic` → ⚠️ لا يُكتب | FurnishingAr | **Property Information** |
| 39 | `PropertyWhatsAppNo` | `PropertyWhatsAppNo` | `TempServices.PropertyWhatsAppNo` → ⚠️ لا يُكتب | PropertyWhatsAppNo | **Property Information** |
| 40 | `Property email` | `PropertyEmail` | `TempServices.PropertyEmail` → ⚠️ لا يُكتب | PropertyEmail | **Property Information** |
| 41 | `Project Unit` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 42 | `Project Unit Type` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 43 | `Project Unit Image` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 44 | `Exterior Details` | `ExteriorDetails` | `TempServices.ExteriorDetails` → ⚠️ لا يُكتب | Exteriors | **Property Information** |
| 45 | `Exterior Details Arabic` | `ExteriorDetailsArabic` | `TempServices.ExteriorDetailsArabic` → ⚠️ لا يُكتب | ExteriorsAr | **Property Information** |
| 46 | `Exterior Image` | `ExteriorImage` | `TempServices.ExteriorImage` → ⚠️ لا يُكتب | ServiceTypeFiles (Exterior) | **Property Information** |
| 47 | `Interior Details` | `InteriorDetails` | `TempServices.InteriorDetails` → ⚠️ لا يُكتب | Interiors | **Property Information** |
| 48 | `Interior Details Arabic` | `InteriorDetailsArabic` | `TempServices.InteriorDetailsArabic` → ⚠️ لا يُكتب | InteriorsAr | **Property Information** |
| 49 | `Interior Image` | `InteriorImage` | `TempServices.InteriorImage` → ⚠️ لا يُكتب | ServiceTypeFiles (Interior) | **Property Information** |
| 50 | `Amenities Name` | `AmenitiesName` | `ServiceTypeAmenities.AmenitiesName` | AmenitiesName | **Real Estate Type Amenity** |
| 51 | `Amenities name arabic` | `AmenitiesNameArabic` | `ServiceTypeAmenities.AmenitiesNameAr` | AmenitiesNameAr | **Real Estate Type Amenity** |
| 52 | `Amenitise Image` | `AmenitiesImage` | `ServiceTypeAmenities.AmenitiesLogo` | AmenitiesLogo | **Real Estate Type Amenity** |
| 53 | `Amenities File` | `AmenitiesFile` | `TempServices.AmenitiesFile` → ⚠️ لا يُكتب | ServiceTypeFiles (Amenity) | **Real Estate Type Amenity** |
| 54 | `CloserProperty Name` | `CloserPropertyName` | `TempServices.CloserPropertyName` → ⚠️ لا يُكتب | ServiceTypeAmenities (CloserProperty) | **Property Information** |
| 55 | `CloserProperty Name Arabic` | `CloserPropertyNameArabic` | `TempServices.CloserPropertyNameArabic` → ⚠️ لا يُكتب | ServiceTypeAmenities (CloserProperty) | **Property Information** |
| 56 | `CloserProperty Logo` | `CloserPropertyLogo` | ⛔ **خطأ في اسم العمود** (فراغان) → دائماً فارغ | ServiceTypeAmenities.AmenitiesLogo | **Property Information** |
| 57 | `CloserProperty File` | `CloserPropertyFile` | `TempServices.CloserPropertyFile` → ⚠️ لا يُكتب | ServiceTypeFiles (CloserProperty) | **Property Information** |
| 58 | `Materials name` | `MaterialsName` | `TempServices.MaterialsName` → ⚠️ لا يُكتب | ServiceTypeAmenities (Materials) | **Property Information** |
| 59 | `Materials Name Arabic` | `MaterialsNameArabic` | `TempServices.MaterialsNameArabic` → ⚠️ لا يُكتب | ServiceTypeAmenities (Materials) | **Property Information** |
| 60 | `Materials Logo` | `MaterialsLogo` | `TempServices.MaterialsLogo` → ⚠️ لا يُكتب | ServiceTypeAmenities.AmenitiesLogo | **Property Information** |
| 61 | `Materials File` | `MaterialsFile` | `TempServices.MaterialsFile` → ⚠️ لا يُكتب | ServiceTypeFiles (Materials) | **Property Information** |
| 62 | `Utility Name` | `UtilityName` | `TempServices.UtilityName` → ⚠️ لا يُكتب | ServiceTypeAmenities (Utility) | **Property Information** |
| 63 | `Utility Name Arabic` | `UtilityNameArabic` | `TempServices.UtilityNameArabic` → ⚠️ لا يُكتب | ServiceTypeAmenities (Utility) | **Property Information** |
| 64 | `Utility Logo` | `UtilityLogo` | `TempServices.UtilityLogo` → ⚠️ لا يُكتب | ServiceTypeAmenities.AmenitiesLogo | **Property Information** |
| 65 | `Utility File` | `UtilityFile` | `TempServices.UtilityFile` → ⚠️ لا يُكتب | ServiceTypeFiles (Utility) | **Property Information** |
| 66 | `Video` | `Video` | `TempServices.Video` → ⚠️ **لا يوجد حقل Video في Service/ServiceType** | — | **Real Estates** |
| 67 | `Construction Update` | ❌ لا يوجد | **لا يُقرأ** | — | — |
| 68 | *(عمود فارغ)* | — | — | — | — |

---

## الجداول الفعلية المُستهدفة

| الجدول | يُكتب فيه؟ | البيانات المُكتبة |
|--------|-----------|-----------------|
| `TempServices` | ✅ (مرحلي) | جميع البيانات الواردة في Excel |
| `Services` | ✅ | Name, NameAr, Description, DescriptionAr, AboutUs, AboutUsAr, SpokenLanguages, Address, Latitute, Logitute, CityId, CountryId |
| `ServiceImages` | ✅ | صور العمود `Images` (comma-separated) |
| `ServiceTypes` | ✅ | ServiceTypeName, ServiceTypeNameAr, PersoneQuantity, Description, DescriptionAr, Size, Price, PaymentType, BigDescription |
| `ServiceTypeFiles` | ✅ | صور العمود `Images` (الصورة الثانية فما بعد) |
| `ServiceCategories` | ✅ | ربط Service + ServiceType بـ Category |
| `Categories` | ✅ | `Service Type Category` و `Service Type Sub Category` |
| `Countries` | ✅ | `Country Name` |
| `Cities` | ✅ | `City Name` |
| `ServiceTypeAmenities` | ⚠️ جزئي | `Amenities Name` فقط — CloserProperty/Materials/Utility **لا تُكتب** |
| `PropertyInformations` | ❌ **لا يُكتب أبداً** | كل بيانات Property مفقودة بعد الحفظ النهائي |
| `FaqServices` | ❌ فارغ | لا توجد أعمدة FAQ في Excel |
| `ServiceLandmark` | ❌ فارغ | لا توجد أعمدة Landmark في Excel |
| `ServiceOffers` | ❌ | لا تأثير للرفع عليه |

---

## الأخطاء المنطقية

### ❌ خطأ 1 (الأكبر): PropertyInformations لا يُكتب فيه أبداً

**الوصف:** دالة `AddService` في `TempServiceDataAccess.cs` تقرأ TempServices وتكتب في الجداول الفعلية، لكنها **لا تحتوي على أي كود يُنشئ سجلاً في جدول `PropertyInformations`**.

**الأعمدة المتأثرة (14 عمود مفقود):**
- Property Type / Property Type Arabic
- Ref No
- Property Purpose *(Arabic غير موجود أصلاً، انظر خطأ 5)*
- Furnishing / Furnishing Arabic
- PropertyWhatsAppNo / Property email
- Exterior Details / Exterior Details Arabic / Exterior Image
- Interior Details / Interior Details Arabic / Interior Image

**الأثر:** قسم **Property Information** في Sidebar سيظل فارغاً بالكامل بعد الرفع بالجملة.

**الحل:** إضافة كود في `AddService` لإنشاء `PropertyInformation` وربطه بـ `ServiceTypeId`.

---

### ❌ خطأ 2: CloserProperty / Materials / Utility لا تُكتب في `ServiceTypeAmenities`

**الوصف:** الكود في `AddService` يكتب الـ Amenities العادية في `ServiceTypeAmenities`، لكنه لا يكتب CloserProperty أو Materials أو Utility رغم أنها محفوظة في TempServices.

**الأعمدة المتأثرة (12 عمود مفقود):**
- CloserProperty Name / CloserProperty Name Arabic / CloserProperty Logo / CloserProperty File
- Materials name / Materials Name Arabic / Materials Logo / Materials File
- Utility Name / Utility Name Arabic / Utility Logo / Utility File

**الأثر:** هذه البيانات تُحفظ في TempServices لكنها تختفي عند الضغط على "Save All".

**الحل:** إضافة 3 حلقات في `AddService` تكتب كل مجموعة في `ServiceTypeAmenities` مع `AmenitiesType = "CloserProperty"` / `"Materials"` / `"Utility"`.

---

### ❌ خطأ 3: اسم عمود `CloserProperty Logo` يحتوي على فراغين

**الوصف:** في الكود:
```csharp
data.CloserPropertyLogo = safeRead(objDataRow, "CloserProperty  Logo"); // فراغان!
```
بينما اسم العمود في Excel هو `CloserProperty Logo` (فراغ واحد).

**الأثر:** `CloserPropertyLogo` دائماً `""` ولا تُرفع أي صورة للـ CloserProperty.

**الحل:** تصحيح إلى `"CloserProperty Logo"` (فراغ واحد).

---

### ❌ خطأ 4: عمود `Images` يُستخدم لكلا الـ Service والـ ServiceType

**الوصف:**
```csharp
data.Images    = safeRead(objDataRow, "Images");  // → ServiceImages
data.TypeImage = safeRead(objDataRow, "Images");  // → ServiceTypes.Image + ServiceTypeFiles
```
نفس العمود يُكتب في جدولين مختلفين.

**الأثر:** كل صورة في Excel تظهر مرتين: مرة على مستوى الـ Service ومرة على مستوى الـ ServiceType. هذا قد يكون مقصوداً، لكنه يُشكّل ازدواجية.

**التوصية:** إنشاء عمود منفصل `Service Images` للـ Service و`Service Type Images` للـ ServiceType.

---

### ❌ خطأ 5: `Property Purpose Arabic` لا يوجد في نموذج TempService

**الوصف:** العمود موجود في Excel (`Property Purpose Arabic`) لكن لا يوجد له حقل في `TempService` model ولا في الـ SP، ولم يُكتب أي `safeRead` له في الـ Controller.

**الأثر:** `PropertyInformations.PurposeAr` لن يُملأ أبداً.

**الحل:** 
1. إضافة `public string PropertyPurposeArabic { get; set; }` لـ `TempService` model
2. إضافة `propertyPurposeAr NVARCHAR(MAX)` في SP
3. إضافة `safeRead(row, "Property Purpose Arabic")` في Controller

---

### ❌ خطأ 6: `Size Arabic` لا يُكتب في `ServiceTypes`

**الوصف:** `TempService.SizeAr` يُحفظ في TempServices، لكن في `AddService`:
```csharp
var serviceType = new ServiceType {
    Size = item.Size,
    // SizeAr مفقود!
};
```

**الأثر:** عمود `Size Arabic` في Excel يُضيع بعد الحفظ النهائي.

**الحل:** إضافة `SizeAr = item.SizeAr` في ServiceType constructor.

---

### ⚠️ خطأ 7: `Video` لا يوجد له حقل في جداول الإنتاج

**الوصف:** `TempService.Video` و `TempServices.Video` موجودان، لكن **لا يوجد حقل `Video`** في نموذج `Service` ولا `ServiceType`.

**الأثر:** رابط الفيديو يختفي تماماً بعد الحفظ.

**الحل:** إضافة حقل `Video NVARCHAR(MAX)` في جدول `Services` أو `ServiceTypes` + migration + كود في `AddService`.

---

### ⚠️ خطأ 8: أعمدة في Excel لا تُقرأ إطلاقاً (8 أعمدة ضائعة)

| العمود في Excel | السبب |
|----------------|-------|
| `Latest Project` (col 9) | لا يوجد حقل مقابل في TempService |
| `Bed Quantity` (col 18) | لا يوجد حقل مقابل |
| `Bathroom Quantity` (col 19) | لا يوجد حقل مقابل |
| `Service Type AboutUs` (col 22) | لا يوجد حقل مقابل |
| `Service Type AboutUs Arabic` (col 23) | لا يوجد حقل مقابل |
| `Price Trends` (col 28) | لا يوجد حقل مقابل |
| `Project Unit` (col 41) | لا يوجد حقل مقابل |
| `Project Unit Type` (col 42) | لا يوجد حقل مقابل |
| `Project Unit Image` (col 43) | لا يوجد حقل مقابل |
| `Construction Update` (col 67) | لا يوجد حقل مقابل |

---

### ⚠️ خطأ 9: أعمدة في الكود لا توجد في Excel (تُقرأ دائماً `""`)

| اسم العمود في safeRead | ملاحظة |
|----------------------|--------|
| `ServiceHour` | لا يوجد في Excel → `Services.ServiceHour = 0` دائماً |
| `Category` | لا يوجد في Excel → لا تُنشأ Categories من هذا الحقل |
| `Category arabic` | لا يوجد في Excel |
| `Category Image` | لا يوجد في Excel |
| `AdultQuantity` | لا يوجد في Excel → `ServiceTypes.AdultQuantity = 0` |
| `ChildrenQuantity` | لا يوجد في Excel → `ServiceTypes.ChildrenQuantity = 0` |

---

### ⚠️ خطأ 10: لا توجد أعمدة Landmark في Excel

الـ Sidebar يحتوي على **Real Estate Landmark** لكن Excel لا يحتوي على أي عمود لبيانات Landmark (اسم، مسافة، إحداثيات). **لا يمكن ملء هذا القسم بالرفع بالجملة**.

---

### ⚠️ خطأ 11: `Amenities File` لا يُكتب في `ServiceTypeFiles`

الكود في `AddService` يكتب `AmenitiesLogo` في `ServiceTypeAmenities` لكنه لا يكتب `AmenitiesFile` في `ServiceTypeFiles`.

---

## ملخص الأقسام في Sidebar بعد الرفع

| قسم Sidebar | هل يُملأ بعد الرفع؟ | ملاحظة |
|------------|---------------------|--------|
| **Real Estates** (القائمة الرئيسية) | ✅ يُملأ | Services table |
| **Real Estates Catagory** | ✅ يُملأ | من عمودَي Service Type Category + Sub Category |
| **Real Estates Type** | ✅ يُملأ جزئياً | ServiceTypes — لكن SizeAr مفقود |
| **Real Estate Type Amenity** | ✅ يُملأ جزئياً | Amenities فقط — File مفقود |
| **Real Estate Landmark** | ❌ لا يُملأ | لا توجد أعمدة Landmark في Excel |
| **Property Information** | ❌ **لا يُملأ أبداً** | الخطأ الأكبر — AddService لا يكتب في PropertyInformations |
| **Real Estates Offers** | ❌ لا ينطبق | لا علاقة له بالرفع بالجملة |
| **Real Estates Banner** | ❌ لا ينطبق | لا علاقة له بالرفع بالجملة |

---

## أولويات الإصلاح

| الأولوية | الخطأ | التأثير |
|---------|-------|---------|
| 🔴 عاجل | خطأ 1: PropertyInformations لا يُكتب | Property Information فارغ تماماً |
| 🔴 عاجل | خطأ 2: CloserProperty/Materials/Utility مفقودة | بيانات تختفي بعد الحفظ |
| 🟠 مهم | خطأ 3: فراغان في CloserProperty Logo | صور CloserProperty لا ترفع |
| 🟠 مهم | خطأ 7: Video لا يُحفظ | روابط الفيديو مفقودة |
| 🟡 متوسط | خطأ 5: Property Purpose Arabic مفقود من النموذج | عربية الغرض مفقودة |
| 🟡 متوسط | خطأ 6: SizeAr لا يُكتب في ServiceTypes | حجم عربي مفقود |
| 🟢 منخفض | خطأ 8: أعمدة Excel لا تُقرأ | بيانات كاملة غير مدعومة بعد |
| 🟢 منخفض | خطأ 11: Amenities File مفقود | ملفات Amenity لا ترفع |
