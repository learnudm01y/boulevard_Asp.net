# Legal Analysis Report: Agreement vs. Actual Deliverables

## Boulevard System — Ref No: RX-23850

**Report Date:** March 14, 2026  
**Purpose:** Forensic comparison of contractual obligations against actual delivered system for legal proceedings  
**Methodology:** Line-by-line contract analysis, full source code inspection (70+ controllers, 75 database tables, 55+ Dart files, all configuration files), and cross-referencing with 6 prior technical audit reports

---

## Parties to the Agreement

| Role | Details |
|------|---------|
| **Developer** | ROYEX Technologies LLC (License No: 875209), 3203 The Citadel Tower, 32nd Floor, Marasi Dr, Business Bay, Dubai, UAE |
| **Client** | Boulevard Superapp, Abu Dhabi, UAE — Contact: Ali Alawi |
| **Agreement Date** | 10/05/2024 |
| **Agreement Price** | 39,000.00 AED + 1,950.00 VAT = **40,950 AED** |
| **Completion Time** | 10 weeks from signing |

---

## Part 1: Undelivered Contractual Deliverables

The agreement specifies exactly **4 deliverables** under Item 6 (Particular Conditions) and Schedule A:

> A. Mobile Application for the Users (Android & IOS)  
> B. Mobile Application for the technicians (Android & IOS)  
> C. Admin Panel  
> D. Basic Website

### 1.1 Technician Application — COMPLETELY MISSING

**Contractual Requirement (Item 6.B + Schedule A):**

The agreement explicitly requires a **separate** mobile application for technicians with the following features:

- Login with credentials
- View Current Requests, Completed Requests, Pending Requests
- Receive notifications with order details when assigned a new request
- Multiple status stages: On my way, reached, started, finished — with customer notification at each stage
- Upload before and after images of service
- Switch status (offline / online)
- Google Maps API for real-time location

**Actual Delivery:**

There is **NO separate technician application** anywhere in the delivered codebase. The workspace contains only:

- `Source Code (App)/boulevard-app/` — A single Flutter project (the User app)
- `SourceCode of ADmin/Boulevard/` — The ASP.NET Admin Panel
- `DB backup and Script/` — Database files

The only file referencing "Technician" is `lib/modules/motors/views/technician_view.dart` inside the **user application**, which is merely a "Book a Battery Technician" UI screen for customers — **not** a technician-side management application.

**None** of the contracted technician features exist:
- No technician login system
- No order management for technicians
- No status stage workflow (On my way / reached / started / finished)
- No before/after image upload capability
- No online/offline switching
- No real-time location tracking

**Conclusion:** This is a **complete failure to deliver** an entire primary deliverable — one of only 4 contractual deliverables. This represents approximately **25% of the contracted project scope**.

---

### 1.2 Basic Website — COMPLETELY MISSING

**Contractual Requirement (Item 6.D + Schedule A):**

> "Website will be a basic website which will provide the general information about the application. Download link should be provided to download the user application from the corresponding store (appstore and playstore)"

**Actual Delivery:**

There is **NO website** of any kind. The `web/` folder inside the Flutter project contains only `index.html` — the default Flutter web shell (a boilerplate `flutter.js` loader). This is **not** an informational website with app store download links. It is the standard auto-generated Flutter web build output present in every Flutter project.

There is no separate website project anywhere in the workspace. The Admin Panel is a backend management tool, not a public-facing website.

**Conclusion:** Another **complete failure to deliver** a primary deliverable — the second of 4 contracted deliverables. This represents an additional **25% of the contracted project scope**.

---

### 1.3 Summary: Primary Deliverables

| # | Deliverable (Per Agreement Item 6) | Status | Notes |
|---|-------------------------------------|--------|-------|
| A | Mobile Application for Users (Android & iOS) | **Delivered** | Functional but with severe quality issues documented below |
| B | Mobile Application for Technicians (Android & iOS) | **NOT DELIVERED** | Does not exist in any form |
| C | Admin Panel | **Delivered** | Functional but with catastrophic security vulnerabilities |
| D | Basic Website | **NOT DELIVERED** | Does not exist in any form |

**2 out of 4 primary deliverables (50%) were never delivered.**

---

## Part 2: Undelivered or Incomplete Features (Schedule A)

### 2.1 Chat System — NOT DELIVERED

**Contractual Requirement:**
> "There will also be a text based chat system through which they can get their queries answered."

**Actual Delivery:**

All chat-related code is **entirely commented out**:

- `lib/modules/medical/views/chat_home_view.dart` — the **entire file** is commented out (every line starts with `//`)
- Route `CHAT_HOME_VIEW` is commented out in `lib/routes/app_routes.dart` and `lib/routes/app_pages.dart`
- Chat icon in `lib/modules/real_estate/views/real_estate_details_view.dart` is commented out
- Chat navigation in `lib/modules/medical/widgets/doctor_list_tile_widget.dart` is commented out

There is **no functional chat system** — no messaging UI, no chat API, no real-time communication of any kind.

**Conclusion:** Feature explicitly contracted but not delivered. Only commented-out placeholder code exists.

---

### 2.2 Twitter Sign-In — NOT IMPLEMENTED

**Contractual Requirement:**
> "Sign up by UAE pass, Gmail, Meta, **Twitter** and generic details like Email, Phone number."

**Actual Delivery:**

| Sign-In Method | Status | Evidence |
|----------------|--------|----------|
| Email/Password | Implemented | Sign-in view with email + password fields |
| Google (Gmail) | Implemented | `google_sign_in: ^7.2.0` in pubspec.yaml |
| Facebook (Meta) | Implemented | `flutter_facebook_auth: ^7.1.2` in pubspec.yaml |
| UAE Pass | Implemented | `flutter_uae_pass: ^1.1.1` in pubspec.yaml |
| Apple Sign-In | Implemented | `sign_in_with_apple: ^7.0.1` — **not in contract** |
| **Twitter** | **NOT Implemented** | No Twitter SDK in pubspec.yaml, no login method, no controller code |
| Phone/OTP | **Commented Out** | OTP flow at `sign_in_view.dart` line 270 wrapped in `/* */` |

Twitter sign-in was explicitly contracted but was never implemented. There is only an asset reference to a Twitter logo in `assets_manager.dart` — no SDK dependency, no authentication code.

Phone/OTP sign-in code exists but is entirely commented out and non-functional.

---

### 2.3 Real-Time Location Tracking — NOT IMPLEMENTED

**Contractual Requirement (Technician Application section):**
> "Google Maps API for realtime location"

**Actual Delivery:**

Google Maps is integrated for **static map display** (showing a pin at a location) in grocery checkout and real estate views. However, **real-time location tracking** — showing a technician's live movement on a map — does not exist. This feature is inherently tied to the missing Technician Application.

---

### 2.4 Flowers and Chocolates — PARTIALLY IMPLEMENTED

**Actual Delivery:**

The module directory exists at `lib/modules/flowers_and_chocolate/` with bindings, controllers, and views. However, the dedicated routes (`FLOWER_AND_CHOCOLATE_HOME_VIEW`, `CATEGORY_WISE_PRODUCT_VIEW`) are **commented out** in the routing files, making this section unreachable in the app.

---

### 2.5 Medical Services — PARTIALLY IMPLEMENTED

**Actual Delivery:**

The module exists at `lib/modules/medical/` with views and widgets. However, key routes including `HEALTH_HOME_VIEW` and `NEARBY_HOSPITALS_VIEW` are **commented out** in the routing files.

---

### 2.6 Feature Delivery Summary

| Feature (Per Schedule A) | Status | Evidence |
|--------------------------|--------|----------|
| Day mode / Night mode | **Delivered** | `adaptive_theme: ^3.6.0`, toggle in settings |
| Modern Auto search | **Delivered** | Search functionality exists |
| English and Arabic Language | **Delivered** | Full l10n with `app_en.arb` and `app_ar.arb` |
| Sign up — UAE Pass | **Delivered** | `flutter_uae_pass: ^1.1.1` |
| Sign up — Gmail | **Delivered** | `google_sign_in: ^7.2.0` |
| Sign up — Meta (Facebook) | **Delivered** | `flutter_facebook_auth: ^7.1.2` |
| Sign up — Twitter | **NOT Delivered** | No SDK, no code |
| Sign up — Email | **Delivered** | Email + password form |
| Sign up — Phone/OTP | **Commented Out** | Code exists but disabled |
| Profile View/Edit | **Delivered** | Profile module exists |
| Real Estate Journey | **Delivered** | Full module at `lib/modules/real_estate/` |
| Hotel Booking (simple form) | **Delivered** | Module at `lib/modules/hotel_flight_booking/` |
| Flight Booking (simple form) | **Delivered** | Part of hotel/flight module |
| Salon Services | **Delivered** | Combined under motors/beauty module |
| Motor Service | **Delivered** | Full module at `lib/modules/motors/` |
| Grocery | **Delivered** | Full module at `lib/modules/grocery/` |
| Typing Services | **Delivered** | Module at `lib/modules/typing/` |
| Medical Services | **Partial** | Routes partially commented out |
| Flowers and Chocolates | **Partial** | Routes commented out |
| Text-based Chat System | **NOT Delivered** | All code commented out |
| Push Notifications | **Delivered** | Firebase Messaging + Awesome Notifications |
| Payment Gateway | **Delivered** | MyFatoorah integrated |
| Google Maps Location | **Partial** | Static maps only, no real-time tracking |
| Technician App Features | **NOT Delivered** | Entire app missing |
| Basic Website | **NOT Delivered** | Does not exist |

---

## Part 3: Breach of Professional Standards (Agreement Clause 7.1)

Clause 7.1 of the agreement states:

> "The developer represents and warrants to the Client that it has the experience and ability to perform the services required by this agreement; that **it will perform said services in a professional and competent manner**; that it has the power to enter into and perform this agreement."

The delivered system contains **147+ documented technical issues** across 6 independent audit reports, verified through direct code inspection. These issues constitute a fundamental breach of the professional competency warranty.

---

### 3.1 CATASTROPHIC SECURITY VULNERABILITIES

#### 3.1.1 Complete Absence of Authentication on ALL Controllers

**Finding:** There is **ZERO** authentication on any of the 70 controllers in the system. A grep search for `[Authorize]` across the entire codebase returns no results.

- **33 API Controllers** (in `Controllers/`) — No `[Authorize]` attribute
- **37 Admin Area Controllers** (in `Areas/Admin/Controllers/`) — No `[Authorize]` attribute
- No global authorization filter registered in `FilterConfig` or `WebApiConfig`
- The `FormsAuthentication` cookie is created on admin login, but **no controller checks for it**

**Impact:** Every endpoint is publicly accessible. Anyone who knows the URL structure can:
- Delete any member account
- View all order histories and payment data
- Modify payment statuses
- Upload arbitrary files
- Access the full admin dashboard
- Create, modify, or delete any product, service, or category

This is the most fundamental security failure possible in a web application.

#### 3.1.2 SSL Certificate Validation Completely Disabled

**File:** `lib/main.dart`, lines 82-87

```dart
class MyHttpOverrides extends HttpOverrides {
  @override
  HttpClient createHttpClient(SecurityContext? context) {
    return super.createHttpClient(context)
      ..badCertificateCallback =
          (X509Certificate cert, String host, int port) => true;
  }
}
```

**Impact:** The mobile app accepts ALL SSL certificates — expired, self-signed, revoked, or fraudulent. This enables man-in-the-middle attacks on every API call, including login credentials, payment data, and personal information.

#### 3.1.3 Live Payment Gateway API Key Hardcoded in Client App

**File:** `lib/core/utils/services/store_key_const.dart`, line 12

The full MyFatoorah **LIVE production API key** is hardcoded as a static variable in the mobile app source code. Anyone who decompiles the APK (trivial with publicly available tools) can extract this key and:
- Process unauthorized transactions
- View transaction history
- Issue unauthorized refunds
- Perform any payment operation the key authorizes

#### 3.1.4 Unrestricted File Upload — Remote Code Execution

**File:** `Controllers/UploadController.cs`

The upload endpoints (`PostImages()`, `PostFiles()`, `PostVideos()`) accept files of **any type** without validation:
- No file extension whitelist
- No MIME type verification
- No file size limits
- No magic byte validation
- Files saved inside the web root with original extensions
- No authentication required

An attacker can upload a `.aspx` web shell and gain full control of the server.

#### 3.1.5 OTP Bypass in Password Reset

The password reset flow allows an attacker to skip OTP verification by sending `OTP=0`. Additionally, the OTP value is returned in the API response, allowing an attacker to read it directly.

#### 3.1.6 Weak Password Hashing

**File:** `Helper/HashConfig.cs`, lines 12-20

Passwords are hashed with plain **SHA-256 without any salt**. This is cryptographically inadequate:
- Two users with the same password have identical hashes
- Vulnerable to rainbow table attacks
- SHA-256 is a general-purpose hash (billions of attempts/second on modern hardware)
- Industry standard requires bcrypt, scrypt, Argon2, or PBKDF2 with salt

#### 3.1.7 Hardcoded Credentials Throughout Source Code

| Credential | File | Content |
|------------|------|---------|
| SMTP Password | `Helper/EmailService.cs` lines 40, 68 | `partners@boulevardsuperapp.com` / `partners@123` |
| Production DB Password | `Web.config` lines 12-14 (commented) | `password=o50i!32qK`, Server IP: `109.203.124.192` |
| DB SA Password | `Web.config` (commented) | `password=123456` |
| Courier API Key | `Controllers/CourierController.cs` line 48 | `Jeebly123` |
| Courier Outbound API Key | `Service/WebAPI/CourierService.cs` lines 88-90 | `JjEeEeBbLlYy1200` |
| Courier Client Key | `Service/WebAPI/CourierService.cs` | `967X250731093419Y4d6f7374616661536862616972` |
| Android Keystore Password | `android/key.properties` | `storePassword=123456`, `keyPassword=123456` |
| Google OAuth Client IDs | `sign_in_controller.dart` lines 72-75 | Full client IDs exposed |
| UAE Pass Credentials | `sign_in_controller.dart` lines 51-62 | `sandbox_stage` / `sandbox_stage` |
| Google Maps API Key | `AndroidManifest.xml` line 47 | `AIzaSyCzFiSh8ZG6PJfBn8EhMq9288dNVrkPbjM` |
| Firebase API Keys | `firebase_options.dart` | All platform keys exposed |

#### 3.1.8 Missing CSRF Protection

Only **4 out of 30+** POST actions in the admin area have `[ValidateAntiForgeryToken]`. The login form itself has no CSRF protection.

#### 3.1.9 Stored XSS Vulnerabilities

`ValidateInput(false)` is set on 14 admin controller actions, combined with `Html.Raw` in views rendering user content. Since API endpoints have no authentication, attackers can inject JavaScript through the API that executes in admin browsers.

#### 3.1.10 Destructive Operations via GET Requests

Member deletion, address removal, cart clearing, order cancellation, push notification sending, and payment status updates are all accessible via GET requests — vulnerable to CSRF via simple link clicks.

#### 3.1.11 API Service Sends No Authentication Token

**File:** `lib/core/utils/services/api_service.dart`, lines 40-45

The method is named `_getHeadersBearer()` but sends **NO Bearer token and NO Authorization header**. Only `Content-Type` and `Accept` headers are included. Combined with the server having zero `[Authorize]` attributes, there is **no authentication whatsoever** between the mobile app and the API — a complete authentication vacuum.

#### 3.1.12 Additional Security Issues

- Debug mode enabled in production (`Web.config` line 31: `<compilation debug="true">`)
- No custom error pages — ASP.NET default pages expose internal paths
- HTTPS not enforced anywhere — no `RequireHttps`, no HSTS headers
- Authentication cookie set without Secure, HttpOnly, or SameSite flags (15-day expiration)
- Swagger UI publicly accessible with full API map
- `FakeController` and `DemoController` present in production code
- `IsInRole` method throws `NotImplementedException` — role-based authorization impossible
- Connection string injection possible through Excel upload password field
- Silent authentication failure in `Global.asax` — exceptions swallowed
- No root/jailbreak detection on mobile app
- No code obfuscation configured
- Sensitive data logged via `print()` in 50+ locations in the mobile app
- Passwords stored unencrypted in SharedPreferences on mobile
- No session timeout or token expiration
- iOS push notification environment set to "development" instead of "production"

---

### 3.2 CRITICAL PERFORMANCE ISSUES

#### 3.2.1 Complete Absence of Caching

The entire system has **ZERO caching** of any kind:
- No `MemoryCache` usage
- No `[OutputCache]` attributes
- No `HttpRuntime.Cache` usage
- No `outputCacheProfiles` in Web.config
- No HTTP `Cache-Control` headers on API responses
- No client-side caching in mobile app (`sqflite` is commented out)

Every single request hits the database directly, including static reference data (countries, cities, feature categories) that changes rarely or never.

#### 3.2.2 Dashboard Fires 22+ Sequential Database Queries

`DashboardDataAccess.GetAll()` fires 18+ sequential queries per page load. Then 4 AJAX chart endpoints each load the **entire orders table** into memory. A single dashboard page load generates 22+ database queries plus 4 full table scans.

#### 3.2.3 N+1 Query Pattern — 12 Documented Instances

| # | Location | Impact |
|---|----------|--------|
| 1 | `ProductAccess.GetByKey` — upsell/crosssell checks | 100 products = 201 queries |
| 2 | `ServiceAccess.GetAllByFeatureCategoryForChildService` — parent name lookup | 1 query per child service |
| 3 | `ServiceAccess.GetServicesTypeByFeatureCategoryForPackage` — type loading | 1 query per service |
| 4 | `ProductAccess.GetAllByFCatagoryKey` — stock calculation | 1 query per product |
| 5 | `ProductAccess.Insert` — category hierarchy walk | 1 query + 1 save per level |
| 6 | `ServiceAmenityAccess.GetAllServiceAmenityByFeatureCategory` | **Loads ENTIRE table N times** |
| 7 | `ServiceTypeAccess.GetAllServiceType` | 1 query per service ID |
| 8 | `OrderRequestProductDataAccess.GetAll` | 4 queries per order |
| 9 | `OrderRequestServiceDataAccess.GetAll` | 1 query per order |
| 10 | `OrderRequestServiceAccess.getOrderForMember` | Creates new DbContext per call |
| 11 | `ServiceAccess.GetServiceDetailsById` — reviews + service types | 2 queries per review |
| 12 | `DashboardDataAccess.GetAll` — category statistics | 2 queries per category |

Instance #6 is catastrophic: for N services, the **entire ServiceAmenity table** is loaded N times into memory, then filtered in C#.

#### 3.2.4 No Server-Side Pagination — 29 of 30 Endpoints

**29 out of 30** admin listing endpoints load **ALL records** from the database. Only `MemberController` has partial pagination.

The `PaginatedList.cs` class exists but has a critical flaw:
```csharp
var items = await source.ToListAsync(); // Loads ALL items into memory
```
It calls `ToListAsync()` on the full queryable without `.Skip()` / `.Take()` — pagination is cosmetic only.

All 35+ DataTables use client-side processing — all data rendered into DOM.

#### 3.2.5 No Database Indexes

The largest tables (`ProductCategories` with 15,535 rows, `ProductPrices` with 8,959 rows, `Products` with 4,369 rows) have only Primary Key Clustered Indexes. No non-clustered indexes exist on the columns used in WHERE clauses (`Status`, `FeatureCategoryId`, `BrandId`, `ProductId`, `CategoryId`). Every query performs a full table scan.

#### 3.2.6 Layout Settings Queried 3 Times Per Page

On every admin page load, `LayoutSettingAccess` is instantiated 3 times — once in `_Layout.cshtml`, once in `_Header.cshtml`, once in `_Scripts.cshtml`. Each creates a new `UnitOfWork` which creates a new `DbContext`. Additionally, `_Header.cshtml` creates yet another standalone `DbContext`. That is **4 DbContext instances** per page load just from layout files, none disposed.

#### 3.2.7 Excessive Static Asset Loading

Every admin page loads:
- **16 CSS files** individually (including FullCalendar, jVectorMap, Chartist — not used on most pages)
- **28 JavaScript files** individually (including TinyMCE ~500KB, D3.js ~280KB — not used on most pages)
- **jQuery loaded 3 times** from 3 different locations (~300KB wasted)
- **14 views** additionally load jQuery 1.7.1 (from 2011) over **insecure HTTP**
- No bundling or minification despite BundleConfig.cs existing
- No `async` or `defer` attributes on any script tag
- No HTTP compression configured in Web.config

#### 3.2.8 Images Without Compression

`MediaHelper` saves images at 100% quality with no compression, no resizing, and no thumbnail generation. With 4,235 product images, a listing page with 20 products could require 40-100 MB of image data.

---

### 3.3 CRITICAL ARCHITECTURE ISSUES

#### 3.3.1 Repository Pattern Defeats Unit of Work

Every `GenericRepository` method (`Add`, `Edit`, `Remove`, `MultipleRemove`) calls `SaveChanges` immediately. `MultipleRemove` calls `SaveChanges` inside a `foreach` loop — one database commit per deletion. This completely defeats the Unit of Work pattern, making transactional consistency impossible.

#### 3.3.2 IUnitOfWork Does Not Extend IDisposable

`IUnitOfWork` does not inherit from `IDisposable`. While `UnitOfWork` implements it directly, consumers using the interface cannot call `Dispose()`. No service class ever disposes its `UnitOfWork`.

#### 3.3.3 No Dependency Injection

The entire application uses `new` keyword for all instantiation. No IoC container exists. Every service class creates its own `UnitOfWork` in its constructor:

```csharp
public ServiceAccess()
{
    uow = new UnitOfWork();  // Creates new DbContext, never disposed
}
```

This pattern is repeated in **ALL 22+ service classes**. A single HTTP request can cascade into 10-20+ `DbContext` instances.

#### 3.3.4 DbContext Leaks — 30+ Locations

Over 30 locations create `new BoulevardDbContext()` directly without `using` statements or `Dispose()` calls. `NotoficationService` alone creates one in 6+ methods. Database connections remain open until garbage collection.

#### 3.3.5 God Classes

- `ServiceAccess.cs` — 1,804 lines
- `CategoryServiceAccess.cs` — 834 lines
- `ProductAccess.cs` — 807 lines
- `UnitOfWork.cs` — ~1,100 lines with 50+ repository properties

#### 3.3.6 Test/Fake Data in Production Code

`ServiceAccess.cs` contains `GetAllMotorServices()` and `GetAllSalonServices()` methods that return hardcoded fake data with manually constructed service objects.

#### 3.3.7 GenericRepository.Get() Returns Null

When no `orderBy` parameter is provided, the `result` variable stays `null` and the method returns `null` — silently discarding all filters and includes.

#### 3.3.8 Naming Errors in Code

- Method named `Addd` (triple 'd') in GenericRepository
- Property `IsTrenbding` (misspelling of IsTrending) in Category model
- Property `PersoneQuantity` (misspelling of PersonQuantity) in ServiceType model
- Class named `NotoficationService` (misspelling of Notification)

#### 3.3.9 Wrong Variable Returned in Third-Party Login

In `MemberController`, after third-party registration, the code sets `ThirdPartyLogin = true` on the registered member but returns a **different empty variable** (`loginMember`), silently discarding the registration result.

#### 3.3.10 Duplicate Condition Check

`OrderRequestController` checks `model.MemberId == 0 || model.MemberId == 0` instead of the intended `model.MemberId == 0 || model.MemberAddressId == 0`. The `MemberAddressId` is never validated.

---

## Part 4: Legal Basis for Claims

### 4.1 Breach of Clause 6 — Project Deliverables

The agreement specifies 4 deliverables. **2 were never delivered** (Technician App and Basic Website), representing **50% of the contracted project scope**.

### 4.2 Breach of Clause 7.1 — Professional Competency Warranty

> "The developer represents and warrants... that it will perform said services in a professional and competent manner"

**147+ documented technical issues** including:
- Zero authentication on 70 controllers (the most basic security requirement)
- Live payment API key embedded in decompilable client code
- Arbitrary file upload enabling remote code execution
- OTP bypass allowing unauthorized password changes
- Passwords hashed without salt
- Production credentials in source code
- No caching, no pagination, no database indexes
- Systematic resource leaks (DbContext instances never disposed)

No competent professional developer would deliver a system with **zero authentication** across all endpoints.

### 4.3 Breach of Schedule A — Scope of Work

Features explicitly specified in Schedule A but not delivered or non-functional:
- Text-based chat system — all code commented out
- Twitter sign-in — no implementation
- Real-time location tracking — not implemented (tied to missing Technician App)
- Phone/OTP sign-in — code commented out
- Flowers and Chocolates routes — commented out
- Medical Services routes — partially commented out

### 4.4 Breach of Clause 5.1 — Development Services

> "building the platform as per the deliverables for the User as specified in Schedule A of this Agreement and according to the 'Project Details'"

The delivered system does not match the contracted specifications.

### 4.5 Breach of Technology Specification (Schedule A)

The agreement specifies:
> "Source code: We will provide full source code to the client"

The delivered source code contains **hardcoded production credentials** (database passwords, payment API keys, SMTP passwords, courier API keys) — exposing the client to immediate security risks upon receiving the code.

---

## Part 5: Damage Assessment

### 5.1 Undelivered Primary Deliverables

| Item | Contractual Value Proportion | Estimated Value |
|------|------------------------------|-----------------|
| Technician App (Item 6.B) | ~25% of project | ~10,237 AED |
| Basic Website (Item 6.D) | ~25% of project | ~10,237 AED |
| **Subtotal** | **~50%** | **~20,475 AED** |

### 5.2 Undelivered Features

| Feature | Status |
|---------|--------|
| Chat System | Not delivered (all code commented out) |
| Twitter Sign-In | Not implemented |
| Phone/OTP Sign-In | Commented out |
| Real-time Location Tracking | Not implemented |

### 5.3 Remediation Costs

The delivered system requires extensive remediation to reach a minimally acceptable standard:

| Category | Work Required |
|----------|---------------|
| **Security overhaul** | Add authentication to all 70 controllers, fix file upload validation, implement proper password hashing, remove hardcoded credentials, add CSRF protection, fix OTP bypass, implement SSL certificate validation, secure payment key handling |
| **Performance remediation** | Add database indexes, implement caching layer, fix 12 N+1 query patterns, add server-side pagination to 29 endpoints, fix PaginatedList, optimize dashboard queries, add image compression |
| **Architecture remediation** | Implement dependency injection, fix UnitOfWork pattern, dispose DbContext instances, separate concerns, remove dead code |
| **Frontend optimization** | Bundle and minify assets, remove duplicate jQuery loads, implement lazy loading |

### 5.4 Security Risk Exposure

The client is exposed to the following risks from the delivered system:

- **Data breach** — All user data accessible without authentication
- **Financial fraud** — Live payment API key extractable from app
- **Server compromise** — Arbitrary file upload enables remote code execution
- **Account takeover** — OTP bypass allows unauthorized password changes
- **Credential theft** — Production database credentials in source code
- **Reputational damage** — All of the above affecting end users

---

## Part 6: Recommended Legal Actions

### 6.1 Per Agreement Clause 19 — Dispute Resolution

**Step 1:** Issue a written Dispute Notice per Clause 19.1 detailing all breaches documented in this report. The notice must include "reasonable details of the dispute."

**Step 2:** Allow the 30-day period for amicable resolution per Clause 19.1.

**Step 3:** If unresolved within 30 days, refer the matter to the **Dubai International Arbitration Centre (DIAC)** per Clause 19.2, under DIAC Arbitration Rules with:
- One (1) arbitrator
- Legal seat: Dubai
- Language: English

### 6.2 Applicable Law

Per Clause 18: Laws of the Emirate of Dubai and the laws of the United Arab Emirates as applicable in the Emirate of Dubai.

### 6.3 Potential Claims

1. **Breach of contract** — Non-delivery of 50% of primary deliverables (Clauses 1, 6)
2. **Breach of warranty** — Failure to perform services in a professional and competent manner (Clause 7.1)
3. **Breach of scope** — Non-delivery of contracted features (Schedule A)
4. **Damages** — Cost of undelivered work, remediation costs, security exposure costs
5. **Liability cap** — Per Clause 8.7: "The liability of the Developer... shall not exceed the greater of the total amount paid and payable by the Client to the Developer under this Agreement" (40,950 AED)

---

## Part 7: Evidence Index

### 7.1 Prior Technical Audit Reports (in `reports/` directory)

| Report | Date | Focus |
|--------|------|-------|
| `BOULEVARD_SYSTEM_AUDIT_REPORT.md` | March 12, 2026 | 60+ issues across 12 categories |
| `BOULEVARD_UNIFIED_SYSTEM_AUDIT_REPORT.md` | March 13, 2026 | 147+ issues across 18 categories (Admin + Mobile + DB) |
| `DEEP_ARCHITECTURE_AUDIT.md` | March 12, 2026 | Repository, UoW, service layer deep analysis |
| `CACHING_ANALYSIS_REPORT.md` | March 12, 2026 | Complete absence of caching documented |
| `FRONTEND_PERFORMANCE_AUDIT.md` | March 12, 2026 | Pagination, views, static assets audit |
| `PERFORMANCE_REPORT.md` | March 12, 2026 | Arabic-language comprehensive performance report |

### 7.2 Key Source Code Files Referenced

| File | Relevance |
|------|-----------|
| `agreement.md` | Original contract (Ref No: RX-23850) |
| All 70 Controllers | Zero `[Authorize]` attributes |
| `Helper/HashConfig.cs` | SHA-256 without salt |
| `Helper/EmailService.cs` | Hardcoded SMTP credentials |
| `Controllers/UploadController.cs` | No file validation |
| `Controllers/CourierController.cs` | Hardcoded API key |
| `BaseRepository/GenericRepository.cs` | SaveChanges per operation |
| `BaseRepository/IUnitOfWork.cs` | Missing IDisposable |
| `Web.config` | Debug mode, exposed credentials |
| `lib/main.dart` | SSL bypass |
| `lib/core/utils/services/store_key_const.dart` | Live payment API key |
| `lib/core/utils/services/api_service.dart` | No auth headers |
| `android/key.properties` | Keystore password "123456" |

---

*This report was compiled through direct forensic analysis of the complete delivered source code (70 controllers, 75 database tables, 55+ Dart files, all configuration files), the signed agreement (Ref No: RX-23850), and 6 prior independent technical audit reports. All findings have been verified against the actual source code.*
