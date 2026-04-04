# تقرير الاختبار المحلي — Laundry & Photography API

**التاريخ:** 4 أبريل 2026  
**البيئة:** `localhost:5000` (IIS Express — Debug)  
**الإصدار:** Build `04/04/2026 14:08:10`  
**الحالة:** ✅ جميع الاختبارات مكتملة

---

## ملخص الإصلاح المطبَّق

| الملف | السطر | المشكلة | الإصلاح |
|---|---|---|---|
| `Controllers/OfferController.cs` | 73 | خطأ إملائي في اسم المعامل: `featureecategoryid` (حرف `e` مزدوج) | تم تصحيحه إلى `featureCategoryId` |

**تأثير الخطأ قبل الإصلاح:**  
كان الخادم يستقبل القيمة `0` دائماً بسبب عدم تطابق اسم المعامل، فيرجع 11 عرضاً عشوائياً من فئات **Motors / Medical / Beauty** بدلاً من الفئة المطلوبة.

**بعد الإصلاح:** يُرجع `[]` فارغاً للفئات التي لا تحتوي على عروض (صحيح تماماً).

---

## إعدادات الفئات

| القسم | `featureCategoryId` | `defaultServiceId` | GUID |
|---|---|---|---|
| Laundry (مغسلة) | `21` | `50094` | `3c4d5e6f-7a8b-9c0d-ef12-345678901abc` |
| Photography (تصوير) | `22` | `50095` | `4d5e6f7a-8b9c-0d1e-f234-5678901abcde` |

---

## نتائج الاختبار

### قسم Laundry — fcId = 21

| # | Endpoint | Method | HTTP | API Code | النتيجة | الحالة |
|---|---|---|---|---|---|---|
| 1 | `SearchingServiceType` | POST | 200 | 406 | `[]` — لا توجد أنواع خدمات مسجّلة | ⚪ متوقع |
| 2 | `GetServiceDetailsById` (serviceId=50094) | GET | 200 | 406 | `[]` — لا يوجد مزودو خدمة بعد | ⚪ متوقع |
| 3 | `GetRelatedServices` | GET | 200 | 200 | `[]` — لا توجد خدمات مرتبطة | ⚪ متوقع |
| 4 | `GetFilterResponse` | GET | 200 | 200 | كائن فلترة فارغ ✓ | ✅ صحيح |
| 5 | `GetServiceAmenities` | GET | 200 | 200 | `[]` — لا توجد amenities | ⚪ متوقع |
| 6 | `GetTrandingServiceOffer` | GET | 200 | 200 | `[]` — **0 عروض (مصحَّح ✓)** | ✅ مُصلَح |

### قسم Photography — fcId = 22

| # | Endpoint | Method | HTTP | API Code | النتيجة | الحالة |
|---|---|---|---|---|---|---|
| 7 | `SearchingServiceType` | POST | 200 | 406 | `[]` — لا توجد أنواع خدمات مسجّلة | ⚪ متوقع |
| 8 | `GetServiceDetailsById` (serviceId=50095) | GET | 200 | 406 | `[]` — لا يوجد مزودو خدمة بعد | ⚪ متوقع |
| 9 | `GetRelatedServices` | GET | 200 | 200 | `[]` — لا توجد خدمات مرتبطة | ⚪ متوقع |
| 10 | `GetFilterResponse` | GET | 200 | 200 | كائن فلترة فارغ ✓ | ✅ صحيح |
| 11 | `GetServiceAmenities` | GET | 200 | 200 | `[]` — لا توجد amenities | ⚪ متوقع |
| 12 | `GetTrandingServiceOffer` | GET | 200 | 200 | `[]` — **0 عروض (مصحَّح ✓)** | ✅ مُصلَح |

**جميع الـ 12 طلباً أعادت HTTP 200 — لا أخطاء في الخادم.**

---

## شرح النتائج الفارغة

النتائج الفارغة **طبيعية ومتوقعة** لأن هذين القسمين تم إنشاؤهما حديثاً ولم تُضَف إليهما بيانات بعد:

| Endpoint | سبب الفراغ |
|---|---|
| `SearchingServiceType` 406 | لم يُسجَّل أي **ServiceType** مرتبط بـ fcId=21 أو 22 في جدول `ServiceTypeInformation` |
| `GetServiceDetailsById` 406 | لا يوجد مزودو خدمة نشروا إعلانات تحت Laundry أو Photography بعد |
| `GetRelatedServices` 200 `[]` | لا توجد خدمات أخرى مرتبطة بنفس الفئة |
| `GetFilterResponse` 200 كائن فارغ | لا توجد قوائم بيعية لاستخراج خيارات الفلترة منها |
| `GetServiceAmenities` 200 `[]` | لم تُضَف أي amenities لهذه الفئات في لوحة التحكم |
| `GetTrandingServiceOffer` 200 `[]` | لا توجد عروض في جدول `OfferInformation` لـ FeatureCategoryId=21 أو 22 |

---

## استجابة GetFilterResponse (نموذج)

```json
{
  "result": {
    "categories": [],
    "propertyType": [],
    "pricePeriod": [],
    "minprice": 0.0,
    "maxprice": 0.0,
    "bedRooms": [],
    "bathRooms": [],
    "furnishing": [],
    "maxSize": 0,
    "minSize": 0,
    "amenities": []
  },
  "code": 200,
  "message": "success",
  "isSuccess": true
}
```

---

## الخطوات التالية لاستكمال البيانات

لتظهر نتائج حقيقية في هذه Endpoints، يجب إضافة البيانات من لوحة التحكم:

1. **SearchingServiceType** → أضف **Service Types** من: `Admin > Laundry > Service Types`
2. **GetServiceDetailsById / GetRelatedServices** → ليتم تعبئتها تلقائياً عندما يُسجّل مزودو الخدمة إعلاناتهم
3. **GetServiceAmenities** → أضف **Amenities** من إعدادات القسم
4. **GetTrandingServiceOffer** → أضف **Offers** من: `Admin > Offers` وحدد `FeatureCategoryId = 21` أو `22`

---

## الاختبار اليدوي — URLs المستخدمة

```
# Laundry
POST http://localhost:5000/api/v1/Service/SearchingServiceType?featureCategoryId=21&keyword=&size=10&count=0&memberId=0&lang=en
GET  http://localhost:5000/api/v1/Service/GetServiceDetailsById?serviceId=50094&memberId=0&lang=en
GET  http://localhost:5000/api/v1/Service/GetRelatedServices?featureId=21&serviceId=50094&lang=en
GET  http://localhost:5000/api/v1/Service/GetFilterResponse?featureCategoryId=21&lang=en
GET  http://localhost:5000/api/v1/Service/GetServiceAmenities?featureCategoryId=21&lang=en
GET  http://localhost:5000/api/v1/Offers/GetTrandingServiceOffer?featureCategoryId=21&size=10&count=0&lang=en

# Photography
POST http://localhost:5000/api/v1/Service/SearchingServiceType?featureCategoryId=22&keyword=&size=10&count=0&memberId=0&lang=en
GET  http://localhost:5000/api/v1/Service/GetServiceDetailsById?serviceId=50095&memberId=0&lang=en
GET  http://localhost:5000/api/v1/Service/GetRelatedServices?featureId=22&serviceId=50095&lang=en
GET  http://localhost:5000/api/v1/Service/GetFilterResponse?featureCategoryId=22&lang=en
GET  http://localhost:5000/api/v1/Service/GetServiceAmenities?featureCategoryId=22&lang=en
GET  http://localhost:5000/api/v1/Offers/GetTrandingServiceOffer?featureCategoryId=22&size=10&count=0&lang=en
```
