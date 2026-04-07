# ما الذي تغيّر مؤخراً وكيف تُدخل البيانات

> **تاريخ:** 7 أبريل 2026 — يختص فقط بالأعمدة والجداول والبيانات التي عملنا عليها في هذه الجلسة.

---

## 1. ما الذي تغيّر في قاعدة البيانات

### أعمدة جديدة أُضيفت (Migration `202604070001000`)

| الجدول | العمود الجديد | النوع | الغرض |
|---|---|---|---|
| `dbo.Products` | `IcvBoulevardScore` | `NVARCHAR(50)` | نسبة ICV للمنتج (مثل `"85%"`) — تُعرض كـ badge على بطاقة المنتج |
| `dbo.TempProducts` | `IcvBoulevardScore` | `NVARCHAR(50)` | نفس الحقل في الجدول المؤقت قبل الموافقة |

### عمود جديد أُضيف بـ SQL مباشر (ليس Migration)

| الجدول | العمود الجديد | النوع | الملف |
|---|---|---|---|
| `dbo.FeatureCategories` | `CommissionRate` | `DECIMAL(5,2)` | `social_impact_migration.sql` |

---

## 2. تطبيق التغييرات على السيرفر

### الخطوة 1 — تشغيل EF Migration الجديد

هذا يُنشئ عمود `IcvBoulevardScore` في `Products` و `TempProducts`:

```powershell
# من Package Manager Console في Visual Studio
Update-Database
```

أو من سطر الأوامر:

```powershell
cd "i:\entire system - boulevard\SourceCode of ADmin\Boulevard\Boulevard"
.\packages\EntityFramework.6.5.1\tools\migrate.exe Boulevard.dll /startupConfigurationFile=Boulevard.dll.config
```

آخر Migration سيُطبَّق: **`202604070001000_addIcvBoulevardScore`**

---

### الخطوة 2 — تشغيل Social Impact SQL

هذا يُنشئ عمود `CommissionRate` ويُدخل النسب المبدئية لكل البوابات:

```powershell
$root = "i:\entire system - boulevard\SourceCode of ADmin\Boulevard\Boulevard"
sqlcmd -S ".\SQLEXPRESS" -d "BoulevardDb" -i "$root\social_impact_migration.sql"
```

**ما الذي يفعله هذا الملف تحديداً:**
1. يضيف `CommissionRate DECIMAL(5,2)` على جدول `FeatureCategories` إذا لم يكن موجوداً
2. يُحدّث النسب لكل بوابة موجودة في الـ DB — **بشرط أن تكون `NULL` أو `0`** فقط، لن يُغيّر قيمة مدخلة يدوياً

**النسب المبدئية المُدخلة:**

| البوابة | النسبة |
|---|---|
| Grocery | 3% |
| Restaurant | 15% |
| Retail | 7% |
| Desserts & Flowers | 15% |
| Typing Services | 10% |
| Insurance | 5% |
| Medical | 15% |
| Beauty / Salon | 15% |
| Motors | 7% |
| Real Estate | 5% |
| Laundry | 0% |
| Photography | 0% |

---

### الخطوة 3 — تشغيل فهارس الأداء (اختياري — لكن مُوصى به)

```powershell
sqlcmd -S ".\SQLEXPRESS" -d "BoulevardDb" -i "$root\db_indexes_to_apply.sql"
```

---

## 3. إدخال بيانات Social Impact Tracker من اللوحة

### الوصول إلى اللوحة
```
http://localhost:5000/admin/social-impact-tracker
```

### ماذا ستجد؟
جدول بكل البوابات (FeatureCategories) مع نسبة العمولة الحالية لكل واحدة.

### كيف تُعدّل النسبة؟
1. اضغط على حقل النسبة مباشرة في الجدول
2. اكتب الرقم الجديد (بدون %)
3. اضغط **Save** أو **Enter**
4. يُحفظ فوراً في `dbo.FeatureCategories.CommissionRate` عبر AJAX

> لا توجد زر "حفظ كل شيء" — كل سطر يُحفظ بشكل مستقل فور الضغط.

### التحقق من البيانات مباشرة في الـ DB
```sql
SELECT Name, CommissionRate
FROM dbo.FeatureCategories
WHERE IsDelete = 0
ORDER BY Name;
```

---

## 4. إدخال ICV Boulevard Score على المنتجات

العمود `IcvBoulevardScore` **لا يُدخل يدوياً من اللوحة** — يُقرأ تلقائياً من ملف Excel عند رفعه.

### الطريقة: Excel Bulk Upload
1. افتح `Admin → Product → Add Bulk`
2. ارفع ملف Excel يحتوي على العمود **"ICV Boulevard Score"** (يجب أن يكون هذا هو اسم الـ header بالضبط)
3. يقبل الكود أي قيمة نصية: `"85%"` أو `"85"` أو `"High"` — كلها صحيحة
4. العمود **اختياري** — إذا لم يكن موجوداً في الملف سيُكمل الرفع بشكل طبيعي

**ملف الاختبار الجاهز:** `docs/insert_grocery.xlsx` — يحتوي 3 منتجات بقيم `85%` و `90%` و `72%`

### التحقق بعد الرفع
```sql
SELECT ProductName, IcvBoulevardScore
FROM dbo.Products
WHERE IcvBoulevardScore IS NOT NULL AND IcvBoulevardScore != ''
ORDER BY ProductId DESC;
```

أو عبر API مباشرة:
```powershell
Invoke-WebRequest "http://localhost:5000/api/v1/general/GetSingelCategorywiseProduct?categoryId=2334&size=5&count=0" -UseBasicParsing | Select-Object -ExpandProperty Content
```
ابحث عن `"icvBoulevardScore"` في الـ JSON — يجب أن تجد القيمة مثل `"85%"`.

---

*آخر تحديث: 7 أبريل 2026*
