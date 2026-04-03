# دليل رفع المنتجات بالجملة (Excel Upload) — قسم GROCERY

---

## 1. هل يمكن لمنتج واحد أن ينتمي لأكثر من تصنيف؟

**نعم — بشكل تلقائي وعبر 3 مستويات هرمية في نفس الصف.**

كل صف في ملف Excel يُنشئ:
- سجلاً واحداً في جدول `Products`
- من 1 إلى 3 سجلات في جدول `ProductCategories` (بحسب عدد المستويات المملوءة)

---

## 2. أسماء الأعمدة المطلوبة بالضبط

| # | اسم العمود في ملف Excel | ملاحظة |
|---|---|---|
| 1 | `Brand` | اسم الماركة بالإنجليزية |
| 2 | `Barcode` | الباركود (لا يُستخدم للتحقق من التكرار حالياً) |
| 3 | `Category` | التصنيف الرئيسي (Level 1) |
| 4 | `Sub Category` | التصنيف الفرعي (Level 2) — يمكن تركه فارغاً |
| 5 | `Sub Sub Category` | التصنيف الفرعي الثاني (Level 3) — يمكن تركه فارغاً |
| 6 | `ProductName` | اسم المنتج بالإنجليزية |
| 7 | `Item Desc` | وصف المنتج بالإنجليزية |
| 8 | `Attribute Code` | كود الخاصية (مثل: ML001) |
| 9 | `Attribute Name` | اسم الخاصية (مثل: Volume) |
| 10 | `Images` | أسماء ملفات الصور، مفصولة بفاصلة (يجب رفعها مسبقاً في `Content/Upload/Product/`) |
| 11 | `Quantitys` | الكميات، مفصولة بفاصلة — **ملاحظة: مكتوبة بـ `s` في النهاية** |
| 12 | `Selling Price` | الأسعار، مفصولة بفاصلة، بنفس ترتيب الكميات |
| 13 | `Product Tags` | الوسوم، مفصولة بفاصلة |
| 14 | `Stocks Quantity` | كميات المخزون، مفصولة بفاصلة — **Q كبيرة ومسافة بين الكلمتين** |
| 15 | `Product Type` | نوع المنتج: `now` أو `scheduled` |
| 16 | `Delivery Info` | معلومات التوصيل بالإنجليزية |
| 17 | `Delivery Info Arabic` | معلومات التوصيل بالعربية |
| 18 | `Brand Arabic` | اسم الماركة بالعربية |
| 19 | `Category Arabic` | التصنيف الرئيسي بالعربية |
| 20 | `Sub Category Arabic` | التصنيف الفرعي بالعربية |
| 21 | `Sub Sub Category Arabic` | التصنيف الفرعي الثاني بالعربية |
| 22 | `ProductName Arabic` | اسم المنتج بالعربية |
| 23 | `Item Desc Arabic` | وصف المنتج بالعربية |
| 24 | `Attribute Name Arabic` | اسم الخاصية بالعربية |
| 25 | `Category images` | اسم صورة التصنيف الرئيسي |
| 26 | `Sub Category images` | اسم صورة التصنيف الفرعي |
| 27 | `Sub Sub Category images` | اسم صورة التصنيف الفرعي الثاني |

> **تحذير:** أسماء الأعمدة يجب أن تطابق ما في الجدول بالضبط (الحروف الكبيرة والمسافات). العمود رقم 11 مكتوب `Quantitys` (بـ s) عمداً في الكود.

---

## 3. كيف يعمل النظام خطوة بخطوة

```
Excel Row  →  TempProducts (XML)  →  AddProduct()  →  Products + ProductCategories
```

### داخل `TempProductDataAccess.AddProduct()`:

```csharp
// 1. ينشئ أو يجد Brand
var brand = db.Brands.FirstOrDefault(b => b.Title == item.Brand && ...);
if (brand == null) { /* ينشئ brand جديد */ }

// 2. ينشئ أو يجد Category (Level 1)
var category = db.Categories.FirstOrDefault(c => c.CategoryName == item.Category && ...);
if (category == null) { /* ينشئ category جديد بـ ParentId = 0 */ }

// 3. ينشئ أو يجد Sub Category (Level 2)
var subcategory = db.Categories.FirstOrDefault(c => c.CategoryName == item.SubCategory
                  && c.ParentId == category.CategoryId && ...);
if (subcategory == null && !string.IsNullOrEmpty(item.SubCategory)) { /* ينشئ */ }

// 4. ينشئ أو يجد Sub Sub Category (Level 3)
var subSubcategory = db.Categories.FirstOrDefault(c => c.CategoryName == item.SubSubCategory
                     && c.ParentId == subcategory.CategoryId && ...);
if (subSubcategory == null && !string.IsNullOrEmpty(item.SubSubCategory)) { /* ينشئ */ }

// 5. ينشئ المنتج مرة واحدة
db.Products.Add(product);

// 6. يربطه بالتصنيفات الثلاثة
if (category != null)     db.ProductCategories.Add(new ProductCategory { ProductId, CategoryId = category.CategoryId });
if (subcategory != null)  db.ProductCategories.Add(new ProductCategory { ProductId, CategoryId = subcategory.CategoryId });
if (subSubcategory != null) db.ProductCategories.Add(new ProductCategory { ProductId, CategoryId = subSubcategory.CategoryId });
```

---

## 4. السيناريوهات الخمسة في ملف المثال

### السيناريو 1 — ثلاثة مستويات كاملة ✅

| العمود | القيمة |
|---|---|
| Category | `Dairy Products` |
| Sub Category | `Fresh Milk` |
| Sub Sub Category | `Full Fat Milk` |
| ProductName | `Full Fat Milk 1L` |

**النتيجة في DB:**
```
Products:          ProductId=X, ProductName="Full Fat Milk 1L"
ProductCategories: (X, Dairy Products)   ← Level 1
ProductCategories: (X, Fresh Milk)       ← Level 2
ProductCategories: (X, Full Fat Milk)    ← Level 3
```

**نتيجة API:**
```
GET /api/v1/category-products?categoryId={DairyProductsId}  → يظهر المنتج ✓
GET /api/v1/category-products?categoryId={FreshMilkId}      → يظهر المنتج ✓
GET /api/v1/category-products?categoryId={FullFatMilkId}    → يظهر المنتج ✓
```

---

### السيناريو 2 — شجرة تصنيف مستقلة ثانية ✅

| العمود | القيمة |
|---|---|
| Category | `Beverages` |
| Sub Category | `Juices` |
| Sub Sub Category | `Orange Juice` |
| ProductName | `Orange Juice 1L` |

ينتج 3 سجلات `ProductCategory` مستقلة تماماً عن أي منتج آخر.

---

### السيناريو 3 — مستويان فقط (Sub Sub Category فارغ) ✅

| العمود | القيمة |
|---|---|
| Category | `Bakery` |
| Sub Category | `Bread & Loaves` |
| Sub Sub Category | *(فارغ)* |
| ProductName | `White Bread Loaf` |

**النتيجة:** سجلان فقط في `ProductCategories` (يُتخطى Level 3 تلقائياً).

---

### السيناريو 4 — أسعار وكميات متعددة ✅

```
Quantitys       = 100,200
Selling Price   = 1.50,2.75
Stocks Quantity = 200,150
```

**النتيجة:** منتج واحد + **سجلان في `ProductPrices`** + سجلان في `StockLogs`.

> يجب أن يكون عدد القيم متساوياً في الحقول الثلاثة.

---

### السيناريو 5 — نفس المنتج في شجرة مستقلة ثانية ⚠️

رفع نفس المنتج (نفس الباركود) مرة ثانية تحت تصنيف مختلف:

```
Category = Healthy Foods / Sub Category = Low Fat / Sub Sub Category = Low Fat Milk
ProductName = Full Fat Milk 1L    ← نفس الاسم
Barcode = 6281006120019           ← نفس الباركود
```

**النتيجة:** يُنشئ النظام **منتجاً جديداً منفصلاً** — لأنه لا يتحقق من الباركود لمنع التكرار.

**الحل الصحيح للتصنيف المستقل الثاني:**
1. ارفع المنتج مرة واحدة عبر Excel
2. افتح المنتج في: `Admin → Product List → Edit`
3. اختر التصنيفات الإضافية من الشجرة
4. اضغط **Save**

داخلياً ينفذ الكود:
```csharp
List<string> LeafCategories = node.SelectedCategoryId.Split(',').ToList();
foreach (string catId in LeafCategories) {
    ProductCategory productCategory = new ProductCategory() {
        CategoryId = ctgId,
        ProductId = product.ProductId,
        Status = "Active"
    };
    await uow.ProductCategoryRepository.Add(productCategory);
}
```

---

## 5. جدول المقارنة: ما تدعمه Excel وما لا تدعمه

| الحالة | مدعوم عبر Excel؟ | الطريقة |
|---|---|---|
| ظهور المنتج في 1 تصنيف | ✅ | Category فقط |
| ظهور المنتج في 2 تصنيفات هرمية | ✅ | Category + Sub Category |
| ظهور المنتج في 3 تصنيفات هرمية | ✅ | Category + Sub Category + Sub Sub Category |
| ظهور المنتج في شجرتَي تصنيف مستقلتَين | ❌ | يدوياً من Admin UI أو يُنشئ نسخة مكررة |
| منتج بسعرين وكميتين | ✅ | فاصلة في عمودَي Quantitys و Selling Price |
| منتج بصورتين | ✅ | فاصلة في عمود Images |

---

## 6. قواعد التحقق عند الرفع

- كل صف يجب أن يحتوي على `Brand` و`Category` و`ProductName` كحد أدنى
- إذا كان الصف فارغاً بالكامل يُتخطى تلقائياً
- إذا فشل صف بسبب خطأ يتابع النظام (`catch → continue`) والباقي يُعالج
- التصنيفات تُنشأ تلقائياً إذا لم تكن موجودة (لا حاجة لإنشائها مسبقاً)
- الماركات تُنشأ تلقائياً إذا لم تكن موجودة

---

## 7. مسار العملية الكاملة

```
1. Admin → Product List → اختر قسم GROCERY → Add Bulk
2. ارفع ملف Excel (.xlsx أو .xls)
3. النظام يقرأ البيانات ويحفظها في TempProducts
4. تظهر صفحة تأكيد تعرض عدد المنتجات الجديدة والمكررة
5. اضغط "Confirm Upload" → يُنفَّذ AddProduct()
6. تُنشأ: Brands + Categories + Products + ProductImages + ProductPrices + ProductCategories + StockLogs
```

---

## 8. استعلام للتحقق من نجاح العملية

```sql
-- تحقق من عدد التصنيفات المرتبطة بكل منتج
SELECT 
    p.ProductName,
    COUNT(pc.ProductCategoryId) AS LinkedCategories,
    STRING_AGG(c.CategoryName, ' → ') AS CategoryPath
FROM Products p
LEFT JOIN ProductCategories pc ON p.ProductId = pc.ProductId AND pc.Status = 'Active'
LEFT JOIN Categories c ON pc.CategoryId = c.CategoryId
WHERE p.FeatureCategoryId = (SELECT FeatureCategoryId FROM FeatureCategories WHERE Name = 'Grocery')
  AND p.Status = 'Active'
GROUP BY p.ProductName
ORDER BY LinkedCategories DESC
```

منتج بـ 3 تصنيفات هرمية يجب أن يظهر `LinkedCategories = 3`.
