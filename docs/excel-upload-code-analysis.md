# تحليل كود رفع المنتجات بالجملة (Excel Upload) — Grocery

> **ملاحظة مهمة:** هذا التقرير مبني حصرياً على الكود الفعلي الموجود في المشروع.  
> لا يحتوي على افتراضات أو تخمينات.  
> كل نتيجة مدعومة بمقتطف كود مباشر مع رقم السطر.

---

## الملفات المفحوصة

| الملف | الغرض |
|---|---|
| `Service/Admin/TempProductDataAccess.cs` | منطق معالجة Excel وإدخال المنتجات في قاعدة البيانات |
| `Service/ProductAccess.cs` | منطق إنشاء/تعديل المنتج من واجهة الإدارة |

---

## 1. معالجة حقل `SubCategory`

### السؤال
إذا كانت قيمة الخلية في Excel هي `Face, Body` — هل يُعامَل هذا كـ:
- **(A)** تصنيف واحد اسمه `"Face, Body"` ؟
- **(B)** يُقسَّم إلى تصنيفين `"Face"` و `"Body"` ؟

### الكود الفعلي
**الملف:** `Service/Admin/TempProductDataAccess.cs`  
**الدالة:** `AddProduct(int feacherCategoryId)`  
**السطر:** 121

```csharp
// البحث عن SubCategory في قاعدة البيانات
var subcategory = db.Categories.FirstOrDefault(
    c => c.CategoryName.Trim().ToLower() == item.SubCategory.Trim().ToLower()
      && c.FeatureCategoryId == item.FeatureCategoryId
      && c.ParentId == category.CategoryId
);

// إذا لم يُوجد → إنشاؤه
if (subcategory == null && !string.IsNullOrEmpty(item.SubCategory))
{
    subcategory = new Category();
    subcategory.CategoryKey   = Guid.NewGuid();
    subcategory.CreateBy      = 1;
    subcategory.CreateDate    = DateTimeHelper.DubaiTime();
    subcategory.FeatureCategoryId = feacherCategoryId;
    subcategory.CategoryName  = item.SubCategory.Trim();       // ← القيمة الكاملة بلا تقسيم
    subcategory.CategoryNameAr = item.SubCategoryArabic.Trim();
    subcategory.ParentId      = category.CategoryId;
    subcategory.Status        = "Active";
    db.Categories.Add(subcategory);
    db.SaveChanges();
}
```

### النتيجة: **الخيار (A)**

- `item.SubCategory.Trim()` تُسند مباشرةً إلى `CategoryName` دون أي `Split`.
- تم البحث في كامل الملف عن `Split` لحقل `SubCategory` → **لا يوجد**.
- قيمة `"Face, Body"` تُخزَّن كاسم تصنيف واحد بالضبط: `Face, Body`.

---

## 2. معالجة حقل `SubSubCategory`

### الكود الفعلي
**السطر:** 144

```csharp
// البحث عن SubSubCategory
var subSubcategory = db.Categories.FirstOrDefault(
    c => c.CategoryName.Trim().ToLower() == item.SubSubCategory.Trim().ToLower()
      && c.FeatureCategoryId == item.FeatureCategoryId
      && c.ParentId == subcategory.CategoryId     // ← مرتبط بـ SubCategory وليس بـ Category
);

// إذا لم يُوجد → إنشاؤه
if (subSubcategory == null && !string.IsNullOrEmpty(item.SubSubCategory))
{
    subSubcategory = new Category();
    subSubcategory.CategoryKey   = Guid.NewGuid();
    subSubcategory.CreateBy      = 1;
    subSubcategory.CreateDate    = DateTimeHelper.DubaiTime();
    subSubcategory.FeatureCategoryId = feacherCategoryId;
    subSubcategory.CategoryName  = item.SubSubCategory.Trim();   // ← القيمة الكاملة بلا تقسيم
    subSubcategory.CategoryNameAr = item.SubSubCategoryArabic.Trim();
    subSubcategory.ParentId      = subcategory.CategoryId;
    subSubcategory.Status        = "Active";
    db.Categories.Add(subSubcategory);
    db.SaveChanges();
}
```

### النتيجة: نفس الخيار (A) — قيمة واحدة بلا تقسيم

لا يوجد أي `Split` على `item.SubSubCategory` في أي مكان بالكود.

### تحذير إضافي ⚠️

إذا كان حقل `SubCategory` **فارغاً** في Excel → ستكون `subcategory == null` عند وصول الكود لسطر:
```csharp
c.ParentId == subcategory.CategoryId   // NullReferenceException هنا
```
هذا الخطأ يُبتلع صامتاً بسطر:
```csharp
catch (Exception ex) { continue; }    // السطر ~320 — يتجاوز المنتج كله بصمت
```
بمعنى أن أي منتج له `SubSubCategory` لكن `SubCategory` فارغ → **يُتجاهل كلياً دون إشعار**.

---

## 3. منطق البحث عن التصنيفات (Matching Logic)

### هل يُطابَق التصنيف بالاسم فقط أم بالاسم + ParentId؟

| مستوى التصنيف | شرط الاسم | شرط FeatureCategoryId | شرط ParentId | ملاحظة |
|---|---|---|---|---|
| `Category` (الجذر) | ✅ | ✅ | ❌ | ParentId يُسند = 0 عند الإنشاء |
| `SubCategory` | ✅ | ✅ | ✅ `c.ParentId == category.CategoryId` | مرتبط بـ Category الجذر |
| `SubSubCategory` | ✅ | ✅ | ✅ `c.ParentId == subcategory.CategoryId` | مرتبط بـ SubCategory |

### الكود المقارن للمستويات الثلاثة

```csharp
// ── Category (الجذر) — السطر 100 ──
var category = db.Categories.FirstOrDefault(
    c => c.CategoryName.Trim().ToLower() == item.Category.Trim().ToLower()
      && c.FeatureCategoryId == item.FeatureCategoryId
    // لا يوجد شرط ParentId هنا
);

// ── SubCategory — السطر 121 ──
var subcategory = db.Categories.FirstOrDefault(
    c => c.CategoryName.Trim().ToLower() == item.SubCategory.Trim().ToLower()
      && c.FeatureCategoryId == item.FeatureCategoryId
      && c.ParentId == category.CategoryId       // ✅ شرط ParentId موجود
);

// ── SubSubCategory — السطر 144 ──
var subSubcategory = db.Categories.FirstOrDefault(
    c => c.CategoryName.Trim().ToLower() == item.SubSubCategory.Trim().ToLower()
      && c.FeatureCategoryId == item.FeatureCategoryId
      && c.ParentId == subcategory.CategoryId    // ✅ شرط ParentId موجود
);
```

### الخلاصة

- تصنيف بنفس الاسم `"Fruits"` لكن تحت والدَيْن مختلفَيْن **لن يتعارضا** — سيُعاد استخدام الأول الذي يُطابق `ParentId`.
- تصنيف `Category` جذر لو تكرر بنفس الاسم ونفس `FeatureCategoryId` → يُعاد استخدام الأول (لا يُنشأ مكرر).

---

## 4. ربط المنتج بالتصنيفات — عدد السجلات في `ProductCategories`

### الكود الفعلي
**السطرات:** 288–312

```csharp
// ── ربط المنتج بـ Category الجذر ──
if (category != null)
{
    var productCategory = new ProductCategory();
    productCategory.ProductId  = product.ProductId;
    productCategory.CategoryId = category.CategoryId;     // ← الجذر
    productCategory.Status     = "Active";
    db.ProductCategories.Add(productCategory);
    db.SaveChanges();
}

// ── ربط المنتج بـ SubCategory ──
if (subcategory != null)
{
    var productCategory = new ProductCategory();
    productCategory.ProductId  = product.ProductId;
    productCategory.CategoryId = subcategory.CategoryId;  // ← Sub
    productCategory.Status     = "Active";
    db.ProductCategories.Add(productCategory);
    db.SaveChanges();
}

// ── ربط المنتج بـ SubSubCategory ──
if (subSubcategory != null)
{
    var productCategory = new ProductCategory();
    productCategory.ProductId  = product.ProductId;
    productCategory.CategoryId = subSubcategory.CategoryId; // ← SubSub
    productCategory.Status     = "Active";
    db.ProductCategories.Add(productCategory);
    db.SaveChanges();
}
```

### جدول عدد السجلات الناتجة

| حالة Excel | سجلات `ProductCategories` |
|---|---|
| Category فقط | 1 سجل |
| Category + SubCategory | 2 سجلات |
| Category + SubCategory + SubSubCategory | 3 سجلات (الحد الأقصى) |

### النتيجة:

- **شجرة هرمية واحدة فقط** per صف في Excel.
- لا يوجد أي آلية لربط المنتج بشجرتين مستقلتين **عبر Excel**.
- الحد الأقصى = 3 سجلات في `ProductCategories`، وكلها من نفس السلسلة الهرمية.

---

## 5. التحقق من التكرار — Duplicate Prevention

### أولاً: العداد الإحصائي (لا يمنع الإضافة)
**السطرات:** 38–44

```csharp
// هذا الكود يَعُدّ فقط — لا يمنع أي شيء
int TotalDuplicate = (from temp in db.TempProducts
                      where db.Products.Any(f => f.ProductName == temp.ProductName)
                      select temp).Count();
```

هذا الرقم يظهر في واجهة الإدارة قبل الضغط على "تأكيد الرفع" كمعلومة فقط.

### ثانياً: داخل دالة `AddProduct` — لا يوجد أي تحقق

```csharp
// السطر 166 — إنشاء المنتج مباشرة
var product = new Product();
product.ProductKey  = Guid.NewGuid();
product.ProductName = item.ProductName;
product.Barcode     = item.Barcode;      // ← يُسند فقط، لا يُتحقق منه
// ...
db.Products.Add(product);               // ← يُضاف دون أي FirstOrDefault سابق
db.SaveChanges();
```

**لا يوجد** أي استعلام من نوع:
```csharp
// هذا الكود غير موجود — مجرد توضيح للغياب
var existing = db.Products.FirstOrDefault(p => p.Barcode == item.Barcode);
if (existing != null) { /* استخدم الموجود */ }
```

### جدول سلوك التكرار

| السيناريو | النتيجة الفعلية |
|---|---|
| رفع نفس الـ Barcode مرتين | **منتجان منفصلان** في قاعدة البيانات |
| رفع نفس الاسم مرتين | **منتجان منفصلان** (يظهر كـ "مكرر" في العداد فقط) |
| رفع Barcode موجود مسبقاً من Admin UI | **منتج جديد إضافي** |

---

## 6. الدعم متعدد التصنيفات — Admin UI مقابل Excel

### في Admin UI — `Service/ProductAccess.cs` السطر 197

```csharp
if (!string.IsNullOrEmpty(node.SelectedCategoryId))
{
    // SelectedCategoryId = "12,45,78" مثلاً (قائمة IDs مفصولة بفاصلة)
    List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();

    foreach (var catId in LeafCategories)
    {
        // يُنشئ سجل ProductCategory لكل ID
        db.ProductCategories.Add(new ProductCategory {
            ProductId  = product.ProductId,
            CategoryId = int.Parse(catId),
            Status     = "Active"
        });
    }
}
```

هذا المسار متاح أيضاً في دالة Update (السطر 313 — نفس المنطق).

### في Excel Upload — `TempProductDataAccess.AddProduct`

```csharp
// لا يوجد Split على أي حقل تصنيف
// لا يوجد حلقة foreach على تصنيفات متعددة
// 3 سجلات فقط من نفس الشجرة
```

### مقارنة نهائية

| الإمكانية | Admin UI (يدوي) | Excel Upload |
|---|---|---|
| منتج في تصنيف واحد | ✅ | ✅ |
| منتج في 3 تصنيفات من نفس الشجرة | ✅ | ✅ |
| منتج في شجرتين مستقلتين تماماً | ✅ (عبر SelectedCategoryId) | ❌ غير ممكن |
| التحقق من التكرار بالـ Barcode | ✅ (يمكن إضافته) | ❌ غير موجود |

---

## 7. الملخص التنفيذي النهائي

### السؤال 1: هل `"Face, Body"` ينشئ تصنيفاً واحداً أم اثنين؟

> **تصنيف واحد** اسمه `Face, Body` بالكامل.  
> **السبب:** السطر 129 في `TempProductDataAccess.cs`:
> ```csharp
> subcategory.CategoryName = item.SubCategory.Trim();
> ```
> لا يوجد `Split(',')` قبله أو بعده.

---

### السؤال 2: هل يمكن لمنتج أن ينتمي لشجرتين مستقلتين عبر Excel؟

> **لا — مستحيل حالياً عبر Excel.**  
> الحد الأقصى = **3 سجلات** في `ProductCategories`، وكلها من نفس الصف (نفس الشجرة الهرمية).

---

### السؤال 3: أين القيد في الكود بالضبط؟

**القيد الأول — لا تقسيم للتصنيفات:**

```csharp
// السطر 129 — TempProductDataAccess.cs
subcategory.CategoryName = item.SubCategory.Trim();
// يجب أن يكون:
// foreach (var name in item.SubCategory.Split(',')) { ... }
```

**القيد الثاني — لا تحقق من التكرار:**

```csharp
// السطر ~166
db.Products.Add(product);  // بلا أي تحقق مسبق من Barcode
```

**القيد الثالث — لا ربط بشجرات متعددة:**

```csharp
// لا يوجد Split على SelectedCategoryId داخل TempProductDataAccess
// هذه الميزة موجودة فقط في Service/ProductAccess.cs (Admin UI)
```

---

## 8. التعديلات المقترحة (للرجوع إليها لاحقاً)

> **تحذير:** هذه مقترحات فقط — لم تُطبَّق بعد في الكود.

### أ) دعم تقسيم SubCategory بالفاصلة

```csharp
// بدلاً من:
subcategory.CategoryName = item.SubCategory.Trim();

// يصبح:
var subCatNames = item.SubCategory.Split(',')
                      .Select(s => s.Trim())
                      .Where(s => !string.IsNullOrEmpty(s));

foreach (var subCatName in subCatNames)
{
    var sub = db.Categories.FirstOrDefault(
        c => c.CategoryName.Trim().ToLower() == subCatName.ToLower()
          && c.FeatureCategoryId == feacherCategoryId
          && c.ParentId == category.CategoryId
    ) ?? CreateSubCategory(subCatName, category.CategoryId, feacherCategoryId, db);

    // ربط المنتج بهذا التصنيف
    db.ProductCategories.Add(new ProductCategory {
        ProductId  = product.ProductId,
        CategoryId = sub.CategoryId,
        Status     = "Active"
    });
    db.SaveChanges();
}
```

### ب) منع تكرار المنتج بالـ Barcode

```csharp
// إضافة قبل db.Products.Add(product)
if (!string.IsNullOrEmpty(item.Barcode))
{
    var existing = db.Products.FirstOrDefault(
        p => p.Barcode == item.Barcode && p.FeatureCategoryId == feacherCategoryId
    );
    if (existing != null)
    {
        // استخدم الموجود بدلاً من إنشاء جديد
        product = existing;
        goto LinkCategories;
    }
}
```
