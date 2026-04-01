# معمارية قسم Grocery — كيف يعمل وكيف تبني قسماً جديداً بنفس الكفاءة

---

## 1. الفكرة المحورية: `fCatagoryKey`

كل قسم في الـ Admin Panel (Grocery، Dessert & Flower، ...) **ليس كوداً مستقلاً**.
الكود واحد — والفصل بين الأقسام يتم بمعامل واحد فقط يُمرَّر في الـ URL:

```
?fCatagoryKey=3b317e3f-cb2f-4fdd-b9c8-3f2186695771
```

هذا الـ GUID هو `FeatureCategoryKey` الخاص بالقسم في جدول `FeatureCategories` بقاعدة البيانات.

---

## 2. جدول `FeatureCategory` — قلب النظام

```csharp
// Models/FeatureCategory.cs
public class FeatureCategory
{
    public int    FeatureCategoryId  { get; set; }  // PK (int)
    public Guid   FeatureCategoryKey { get; set; }  // GUID يُستخدم في الـ URL
    public string Name               { get; set; }  // اسم القسم (مثلاً: Grocery)
    public string NameAr             { get; set; }  // الاسم بالعربي
    public string Image              { get; set; }
    public bool   IsActive           { get; set; }
    public bool   IsDelete           { get; set; }
    public string FeatureType        { get; set; }  // "Product" أو "Service"
    ...
}
```

**كل الجداول الأخرى (Brand, Product, Category, Offer...) تحمل `FeatureCategoryId` كـ Foreign Key.**

---

## 3. مخطط قاعدة البيانات لقسم Grocery

```
FeatureCategory (FeatureCategoryId = 5, FeatureCategoryKey = 3b317e3f-...)
│
├── Product          (FeatureCategoryId = 5)  ← جميع منتجات Grocery
├── Category         (FeatureCategoryId = 5)  ← تصنيفات Grocery
├── Brand            (FeatureCategoryId = 5)  ← براندات Grocery
├── WebHtml          (FeatureCategoryId = 5)  ← البانرات (Grocery Banner)
├── CommonProductTag (FeatureCategoryId = 5)  ← Tags خاصة بـ Grocery
└── OfferInformation (FeatureCategoryId = 5)  ← العروض
```

---

## 4. كيف يعمل كل Controller

كل Controller للصفحات الست يطبق نفس النمط:

```csharp
// مثال: BrandController.cs
public async Task<ActionResult> Index(string fCatagoryKey)
{
    ViewBag.FCatagoryKey = fCatagoryKey;
    if (!string.IsNullOrEmpty(fCatagoryKey))
    {
        ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
        // ← اسم القسم يُعرض في الـ Page Title تلقائياً
    }
    return View();
}
```

ثم في الـ DataAccess layer يتم الفلترة:

```csharp
// مثال: BrandAccess.cs
public async Task<List<Brand>> GetAllByFeatureCategory(string fCatagoryKey)
{
    // يحوّل الـ GUID إلى FeatureCategoryId ثم يفلّتر
    var fcId = context.featureCategories
                .Where(f => f.FeatureCategoryKey == Guid.Parse(fCatagoryKey))
                .Select(f => f.FeatureCategoryId)
                .FirstOrDefault();
    return context.Brands.Where(b => b.FeatureCategoryId == fcId).ToList();
}
```

---

## 5. مخطط المعمارية الكاملة (Layer Diagram)

```
URL: /admin/Brand-List?fCatagoryKey=3b317e3f-...
       │
       ▼
AdminAreaRegistration.cs
  → Route: "Admin_BrandList" → admin/Brand-List  (controller=Brand, action=Index)
       │
       ▼
Areas/Admin/Controllers/BrandController.cs
  → Index(string fCatagoryKey)
  → ViewBag.FCatagoryKey   = fCatagoryKey
  → ViewBag.FCatagoryName  = FeatureCategoryAccess.GetFeatureCategoryName(fCatagoryKey)
  → return View()
       │
       ▼
Areas/Admin/Views/Brand/Index.cshtml
  → JS يستدعي: GET /admin/brand-paged?fCatagoryKey=3b317e3f-...
       │
       ▼
BrandController.GetPagedBrands(fCatagoryKey, ...)
  → Service/BrandAccess.cs → SELECT * FROM Brands WHERE FeatureCategoryId = 5
       │
       ▼
JSON → يُرسَم الجدول في الـ Browser
```

---

## 6. الصفحات الست لقسم Grocery والـ Routes الخاصة بها

| الصفحة | Route Name | URL Template | Controller | Action |
|--------|-----------|--------------|------------|--------|
| Grocery List | `Admin_ProductList` | `admin/Product-List` | Product | Index |
| Grocery Category | `Admin_CategoryList` | `admin/Category-List` | Category | Index |
| Grocery Brand | `Admin_BrandList` | `admin/Brand-List` | Brand | Index |
| Grocery Banner | `Admin_WebHtml` | `admin/webHtml` | WebHtml | Index |
| Grocery Product Tag | `Admin_Common_Product_Tag_List` | `admin/common/product/tag/list` | CommonProductTag | Index |
| Grocery Product Offers | `Admin_OfferList` | `admin/Offer-List/{id}` | Offer | Index |

> **الملاحظة:** الـ URL `?fCatagoryKey=...` يُمرَّر كـ Query String لجميع الصفحات، وهو لا يتغير إلا بين الأقسام.

---

## 7. الـ Sidebar — كيف يتم التمييز بين الأقسام

الملف: `Areas/Admin/Views/Shared/_Sidebar.cshtml`

كل قسم يحتل `<li class="sidebar-item">` منفصلاً، وكل رابط فيه يُمرِّر الـ `fCatagoryKey` المختلف:

```html
<!-- قسم Grocery -->
<li class="sidebar-item">
    <a href="javascript:void(0)">
        <img src=".../Grocery.png" /> <span>Grocery</span>
    </a>
    <ul class="collapse first-level">
        <li><a href="@Url.RouteUrl("Admin_ProductList",  new {fCatagoryKey = "3b317e3f-..."})">Grocery List</a></li>
        <li><a href="@Url.RouteUrl("Admin_CategoryList", new {fCatagoryKey = "3b317e3f-..."})">Grocery Category</a></li>
        <li><a href="@Url.RouteUrl("Admin_BrandList",    new {fCatagoryKey = "3b317e3f-..."})">Grocery Brand</a></li>
        <li><a href="@Url.RouteUrl("Admin_WebHtml",      new {fCatagoryKey = "3b317e3f-..."})">Grocery Banner</a></li>
        <li><a href="@Url.RouteUrl("Admin_Common_Product_Tag_List", new {fCatagoryKey = "3b317e3f-..."})">Grocery Product Tag</a></li>
        <li><a href="@Url.RouteUrl("Admin_OfferList",    new {fCatagoryKey = "3b317e3f-..."})">Grocery Product Offers</a></li>
    </ul>
</li>
```

---

## 8. كيف تضيف قسماً جديداً — الخطوات الكاملة

### الخطوة 1: أضف صفاً في `FeatureCategories` (قاعدة البيانات)

```sql
INSERT INTO FeatureCategories (FeatureCategoryKey, Name, NameAr, IsActive, IsDelete, FeatureType)
VALUES (NEWID(), 'Electronics', 'إلكترونيات', 1, 0, 'Product');
```

ثم احفظ الـ `FeatureCategoryKey` الذي تولّد (GUID الجديد).
مثال افتراضي: `A1B2C3D4-0000-0000-0000-000000000001`

---

### الخطوة 2: أضف القسم في الـ Sidebar

ملف: `Areas/Admin/Views/Shared/_Sidebar.cshtml`

أضف هذا الكود بعد قسم Grocery (أو في أي مكان مناسب):

```html
<li class="sidebar-item">
    <a class="sidebar-link waves-effect waves-dark" href="javascript:void(0)" aria-expanded="false">
        <img src="~/Areas/Admin/Content/assets/images/menu_icon/Electronics.png" alt="Electronics" class="img-fluid" />
        <span class="hide-menu">Electronics</span>
    </a>
    <ul aria-expanded="false" class="collapse first-level">
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_ProductList", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics List</span>
            </a>
        </li>
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_CategoryList", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics Category</span>
            </a>
        </li>
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_BrandList", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics Brand</span>
            </a>
        </li>
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_WebHtml", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics Banner</span>
            </a>
        </li>
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_Common_Product_Tag_List", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics Product Tag</span>
            </a>
        </li>
        <li class="sidebar-item">
            <a class="sidebar-link waves-effect waves-dark"
               href="@Url.RouteUrl("Admin_OfferList", new {fCatagoryKey = "A1B2C3D4-0000-0000-0000-000000000001"})"
               aria-expanded="false">
                <i data-feather="disc" class="feather-icon"></i>
                <span class="hide-menu">Electronics Product Offers</span>
            </a>
        </li>
    </ul>
</li>
```

> استبدل `A1B2C3D4-0000-0000-0000-000000000001` بالـ GUID الفعلي من قاعدة البيانات.
> ضع صورة أيقونة القسم في: `Areas/Admin/Content/assets/images/menu_icon/`

---

### الخطوة 3: انتهيت ✓

**لا تحتاج إلى:**
- ❌ إنشاء Controllers جديدة
- ❌ إنشاء Views جديدة
- ❌ إضافة Routes في `AdminAreaRegistration.cs`
- ❌ كتابة أي Service أو DataAccess جديد
- ❌ أي Migrations

الكود الحالي يدعم أي عدد من الأقسام تلقائياً طالما الـ `fCatagoryKey` موجود في قاعدة البيانات.

---

## 9. ملخص — لماذا هذه المعمارية ذكية

| الميزة | التفسير |
|--------|---------|
| **Zero code duplication** | كل الصفحات الست مشتركة بين جميع الأقسام |
| **فصل البيانات بدون فصل الكود** | `fCatagoryKey` يعمل كـ "namespace" للبيانات |
| **Page titles ديناميكية** | `ViewBag.FCatagoryName` يُملأ تلقائياً من DB |
| **Add buttons ديناميكية** | `Add @ViewBag.FCatagoryName` = "Add Grocery" أو "Add Electronics" |
| **قابل للتوسع بلا حدود** | أضف صفاً واحداً في DB + كتلة sidebar = قسم كامل |

---

## 10. نقاط يجب الانتباه إليها

### ⚠️ الـ `FeatureType` مهم جداً

قيمة `FeatureType` في `FeatureCategory` تحدد سلوك بعض الصفحات:

```csharp
// في OfferController:
var featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
ViewBag.FeatureType = featureCategory.FeatureType;
// إذا كانت "Product" → عرض قائمة المنتجات في العروض
// إذا كانت "Service" → عرض قائمة الخدمات في العروض
```

- اضبطها على `"Product"` للأقسام التي تبيع منتجات (Grocery, Dessert, Electronics...)
- اضبطها على `"Service"` للأقسام التي تقدم خدمات (Salon, Hotel, Motor...)

### ⚠️ الـ GUID يجب أن يُحفظ

بعد إضافة الصف في قاعدة البيانات، احفظ الـ `FeatureCategoryKey` الناتج في ملف آمن — هذا هو المفتاح الوحيد الذي يربط الـ Sidebar بالبيانات.

### ⚠️ لا يوجد صفحة "أقسام" ديناميكية في الـ Sidebar

الـ Sidebar يُبنى يدوياً في `_Sidebar.cshtml`. إضافة قسم جديد تتطلب تعديل هذا الملف يدوياً كما في الخطوة 2 أعلاه.

---

## 11. الأقسام الموجودة حالياً في النظام (من الـ Sidebar)

| القسم | `fCatagoryKey` | النوع |
|-------|----------------|-------|
| Grocery | `3b317e3f-cb2f-4fdd-b9c8-3f2186695771` | Product |
| Dessert & Flower | `88d5d23e-470f-409a-bb6b-def7ab1346fa` | Product |
| Grocery Orders | `3b317e3f-cb2f-4fdd-b9c8-3f2186695771` | Product Orders |
| Dessert & Flower Orders | `88d5d23e-470f-409a-bb6b-def7ab1346fa` | Product Orders |
| Hotel Booking | `6440039B-6E5A-4E65-A0E4-F38B69C46C8C` | Service |
| Flight Booking | `2CDCEBCF-BA2C-4C2C-9C53-F32C06233FFD` | Service |
| Real State | `DD501B2D-FE22-4C31-B340-1B4237FAB5CC` | Service |
| Motor Service | `b3e3e680-c8ef-4ab2-a4ac-d75bb48a3647` | Service |
| Salon Service | `25d8c418-2d26-4159-9d7f-970e3b933b42` | Service |
| Medical Service | `bbc98e2d-941b-44c6-8122-0e12a2645b87` | Service |
| Typing Service | `f4309df5-9121-41ad-831a-994c46b62766` | Service |
| Insurance Service | `c286a46b-5b9a-4519-bb10-8d47ec254ffb` | Service |
