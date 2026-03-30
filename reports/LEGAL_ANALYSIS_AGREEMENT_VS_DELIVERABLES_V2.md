# COMPREHENSIVE LEGAL ANALYSIS: AGREEMENT VS. DELIVERED SYSTEM
## Boulevard Superapp vs. ROYEX Technologies LLC

**Agreement Ref No:** RX-23850  
**Agreement Date:** 10/05/2024  
**Agreement Price:** 40,950 AED (39,000 AED + 1,950 AED VAT)  
**Completion Deadline:** 10 weeks from signing  
**Analysis Date:** March 2026  
**Purpose:** Legal proceedings — complete forensic analysis of contract compliance

---

# TABLE OF CONTENTS

1. [PART 1: Parties and Agreement Identification](#part-1-parties-and-agreement-identification)
2. [PART 2: Clause-by-Clause Contract Analysis](#part-2-clause-by-clause-contract-analysis)
3. [PART 3: Schedule A — Deliverable-by-Deliverable Compliance Audit](#part-3-schedule-a-deliverable-by-deliverable-compliance-audit)
4. [PART 4: Complete Catalogue of System Defects (147+ Issues)](#part-4-complete-catalogue-of-system-defects)
5. [PART 5: Breach of Professional Standards (Clause 7.1)](#part-5-breach-of-professional-standards)
6. [PART 6: Legal Basis for Claims](#part-6-legal-basis-for-claims)
7. [PART 7: Damage Assessment and Quantification](#part-7-damage-assessment-and-quantification)
8. [PART 8: Recommended Legal Actions](#part-8-recommended-legal-actions)
9. [PART 9: Evidence Index](#part-9-evidence-index)

---

# PART 1: PARTIES AND AGREEMENT IDENTIFICATION

## 1.1 The Developer (Defendant)

| Field | Value |
|-------|-------|
| Company Name | ROYEX Technologies LLC |
| License No | 875209 |
| Address | 3203, The Citadel Tower, 32nd Floor, Marasi Dr, Business Bay, Dubai – UAE |
| CEO / Signatory | Rajib Roy |
| Phone | +971 566027916 |
| Email | info@royex.net |
| Website | www.royex.ae |
| Other Offices | Bangladesh: House No: 113, Road No: 10, O.R. Nizam Road, Chittagong; India: 110/8 B.T Road Kolkata - 700108, Kolkata, India |

## 1.2 The Client (Claimant)

| Field | Value |
|-------|-------|
| Company Name | Boulevard Superapp |
| Contact Person | Ali Alawi |
| Address | Abu Dhabi, UAE |
| Email | boulevardsuperapp@gmail.com |
| Contact Number | +971509879809 |

## 1.3 Agreement Particulars

| Field | Value |
|-------|-------|
| Reference Number | RX-23850 |
| Date of Execution | 10/05/2024 |
| Project Value | 39,000.00 AED |
| VAT (5%) | 1,950.00 AED |
| Total Cost | 40,950 AED |
| Completion Time | 10 weeks from signing |
| Governing Law | Laws of the Emirate of Dubai and the UAE |
| Dispute Resolution | Dubai International Arbitration Centre (DIAC) |
| Arbitration Language | English |
| Number of Arbitrators | 1 |
| Maintenance Period | 3 months from go-live (Clause 16) |

## 1.4 Agreed Deliverables (Item 6 of Particular Conditions)

The agreement explicitly defines four (4) deliverables:

| Deliverable | Description |
|-------------|-------------|
| A | Mobile Application for the Users (Android & iOS) |
| B | Mobile Application for the Technicians (Android & iOS) |
| C | Admin Panel |
| D | Basic Website |

---

# PART 2: CLAUSE-BY-CLAUSE CONTRACT ANALYSIS

## Clause 1: Project Deliverables and Scope of Work

**Contract Text:** "The deliverables and scope of work under the project shall be according to Item no 6 of Particular Conditions of Agreement. Details of deliverables shall be in accordance with Schedule A."

**Breach Analysis:**
- **Deliverable B (Technician App):** ENTIRELY MISSING. No separate technician mobile application exists in the delivered codebase. The workspace contains only one Flutter project (`boulevard-app`) which is the customer-facing app. There is no second Flutter project, no separate APK build, no technician-specific modules. This is a complete non-delivery of 25% of the agreed deliverables.
- **Deliverable D (Basic Website):** ENTIRELY MISSING. No standalone website was delivered. The only web-related content is Flutter's auto-generated `web/index.html` boilerplate and the ASP.NET MVC default template home page ("ASP.NET MVC gives you a powerful, patterns-based way to build dynamic websites"). Neither constitutes a "basic website which will provide the general information about the application" with "download link to download the user application from the corresponding store."
- **Deliverable A (User App):** PARTIALLY DELIVERED with significant missing features (detailed in Part 3).
- **Deliverable C (Admin Panel):** PARTIALLY DELIVERED with significant missing features and catastrophic security vulnerabilities (detailed in Parts 3 and 4).

**Conclusion:** 2 out of 4 deliverables were never delivered at all. The remaining 2 were delivered with substantial deficiencies.

---

## Clause 2: Agreement Price and Breakdown

**Contract Text:** Total Cost shall be 40,950 AED as specified.

**Analysis:** The client paid for four deliverables. Two were never delivered (Technician App and Website). The remaining two have critical defects that render them unfit for production use due to catastrophic security vulnerabilities.

---

## Clause 3: Time of Completion

**Contract Text:** "Total time of completion shall be within 10 (Ten) Weeks."

**Analysis:** The 10-week deadline applied from the signing date (10/05/2024), meaning the system should have been fully completed by approximately 19/07/2024. The system was delivered with 2 out of 4 deliverables completely missing and the remaining 2 with 147+ documented defects, indicating that the developer did not meet the contractual deadline for full completion.

---

## Clause 4: Payment Terms

**Contract Text:** Payment schedule per Schedule B. Invoices within 15 days. Bank transfer, cheque, or cash.

**Analysis:** This clause requires the client to pay per the schedule. If the client has fully paid, the developer's non-delivery and defective delivery constitute a failure to provide the contracted consideration in exchange for payment received.

---

## Clause 5: Development Services

### Clause 5.1
**Contract Text:** "The above-named Client retains the developer, and the developer agrees to perform the following services: building the platform as per the deliverables for the User as specified in Schedule A of this Agreement."

**Breach Analysis:** The developer failed to build the platform "as per the deliverables" specified in Schedule A. Specifically:
- The Technician Application was not built at all
- The Website was not built at all
- The Chat System was entirely commented out (every line of `chat_home_view.dart` starts with `//`)
- The Twitter sign-up method was never implemented (only an icon asset exists)
- Phone/OTP sign-up was commented out in certain code paths
- Provider Management in the admin panel was never built
- Report download functionality (CSV/XLS) was never built
- Technician Management in the admin panel was never built
- Multiple dashboard statistics were never implemented
- Pagination was not implemented "in all the pages" as specified
- Breadcrumbs were not implemented "in all the pages" as specified
- Offer pop-ups were never implemented
- Multiple static pages (Privacy Policy, About Us, Quick Links) were never created

### Clause 5.2
**Contract Text:** "The client is solely responsible for supplying Design, Images and Content for the project unless the developer is hired specifically for this purpose."

**Analysis:** This clause limits to design/images/content. The missing deliverables (Technician App, Website, Chat, Reports, Provider Management) are functional software components, not design/content issues. The developer cannot invoke this clause to excuse non-delivery.

### Clause 5.4
**Contract Text:** "The Developer shall keep the Client informed of the progress of the Development Services and, in particular, shall inform the Client of any substantial obstacles or likely delays."

**Analysis:** The developer had an obligation to inform the client of obstacles. The complete non-delivery of 2 out of 4 deliverables and the extensive list of missing features suggests either the developer did not communicate adequately, or communicated without intending to complete the work.

### Clause 5.5
**Contract Text:** "Client acknowledges that the Application will not go live until and unless the Agreement's Total Cost including any cost for variation/changes is fully paid."

**Analysis:** This clause conditions going live on full payment. It does NOT condition delivery quality or feature completeness on payment. The developer's obligation to deliver a professional, complete system exists independently of this clause.

---

## Clause 6: Change in Services

**Contract Text:** Changes require developer quote and client election; additional work chargeable per-day.

**Analysis:** The missing features (Technician App, Website, Chat, Reports, etc.) were part of the ORIGINAL Schedule A, not change requests. The developer cannot characterize these as "additional work" since they were part of the agreed scope from inception.

---

## Clause 7: Warranties by the Developer

### Clause 7.1
**Contract Text:** "The developer represents and warrants to the Client that it has the experience and ability to perform the services required by this agreement; that it will perform said services in a professional and competent manner; that it has the power to enter into and perform this agreement."

**Breach Analysis — THIS IS THE MOST CRITICAL CLAUSE FOR LITIGATION:**

The developer warranted THREE things:
1. **Experience and ability** to perform the services
2. **Professional and competent manner** of performance
3. **Power to enter into and perform** the agreement

**Evidence of breach of the "professional and competent manner" warranty:**

The delivered system contains 147+ documented defects including:

**A. CATASTROPHIC SECURITY FAILURES (no reasonably competent developer would deliver these):**
- ZERO authentication on ALL 70 controllers (33 API + 37 Admin). Every endpoint is publicly accessible. Any person who knows the URL can delete members, modify orders, modify payment statuses, and access the full admin panel without any login.
- SSL certificate validation is completely DISABLED in the mobile app (`MyHttpOverrides` class sets `badCertificateCallback` to always return `true`), making ALL network traffic vulnerable to man-in-the-middle attacks.
- Unrestricted file upload allowing Remote Code Execution — the upload endpoint accepts ANY file type (including `.aspx`, `.exe`, `.config`) without validation, saves files in the web root where IIS will execute them.
- Payment API key (MyFatoorah) is hardcoded in the mobile app source code, accessible to anyone who decompiles the APK.
- OTP bypass in password reset — an attacker can skip OTP verification entirely by setting OTP to 0.
- SHA-256 password hashing WITHOUT salt — vulnerable to rainbow table attacks.
- Hardcoded SMTP credentials in source code: `partners@boulevardsuperapp.com` / `partners@123`.
- Hardcoded database credentials in Web.config comments: `sa` / `123456`, server `109.203.124.192`.
- Hardcoded courier API key: `Jeebly123`.
- Android keystore password is `123456` (stored in `key.properties`).
- Passwords returned in API login response and stored in unencrypted `SharedPreferences`.
- The mobile app sends NO authorization headers in API requests.
- IDOR vulnerabilities on every endpoint that accepts a member ID.
- Debug mode enabled in production Web.config (`debug="true"`).
- Swagger UI publicly accessible without authentication, exposing all API endpoints.
- 14 admin views load jQuery 1.7.1 over insecure HTTP.

**B. FUNDAMENTAL ARCHITECTURE FAILURES:**
- No dependency injection anywhere in the entire system
- Repository pattern defeats its own purpose (SaveChanges called per operation)
- IUnitOfWork interface does NOT extend IDisposable (DbContext never disposed)
- 30+ locations create BoulevardDbContext directly without `using` or `Dispose()`, causing connection pool exhaustion
- Zero caching of any kind in the entire system
- 12 documented N+1 query patterns causing exponential database load
- A single API request can cascade into 10–20 DbContext instances
- Service layer God classes: ServiceAccess.cs (1,804 lines), CategoryServiceAccess.cs (834 lines)
- Test controllers (FakeController, DemoController) and hardcoded fake data in production code
- IsInRole method throws NotImplementedException, making role-based access impossible

These defects individually and collectively demonstrate a failure to meet the minimum standard of professional competence for software development.

---

## Clause 8: Limitations and Exclusions of Liability

### Clause 8.1
**Contract Text:** "Nothing in this Agreement (i) will limit or exclude any liability for fraud or fraudulent misrepresentation."

**Analysis:** If the developer represented that the system was complete and production-ready when it was not, this may constitute fraudulent misrepresentation, for which liability CANNOT be limited.

### Clause 8.7
**Contract Text:** "The liability of the Developer to the Client under this Agreement in respect of any event or series of related events shall not exceed the greater of the total amount paid and payable by the Client to the Developer under this Agreement."

**Analysis:** The maximum liability cap is 40,950 AED (the total agreement price). However, this cap does NOT apply to fraud (Clause 8.1) or to liabilities that cannot be excluded under UAE law (Clause 8.1(ii)(iii)).

---

## Clause 9: Term of the Agreement

**Contract Text:** "This Agreement shall commence on the date and shall remain in effect until all obligations under this Agreement have been properly completed unless termination in accordance with clause 10."

**Analysis:** The agreement remains in effect because obligations have NOT been "properly completed." Two deliverables were never delivered. The remaining two have 147+ defects. The developer's obligations remain unfulfilled.

---

## Clause 10: Termination

### Clause 10.1(a)
**Contract Text:** "Either party may terminate this Agreement immediately by giving written notice of termination to the other party if the other party commits any material breach of this Agreement, and such breach is not remediable."

**Analysis:** Applicable. The non-delivery of the Technician App and Website, and the catastrophic security failures in delivered components, constitute material breaches. Non-delivery is not remediable in the context of the original 10-week timeline and 40,950 AED budget.

### Clause 10.1(c)
**Contract Text:** "the other party persistently breaches this Agreement."

**Analysis:** With 147+ defects across all delivered components, plus 2 entirely missing deliverables, plus multiple missing features in delivered components, the breaches are persistent and pervasive.

---

## Clause 11: Effects of Termination

### Clause 11.4
**Contract Text:** "Upon Termination Developer is not further liable to complete the services and developer shall handover the work completed upto the date of termination to the client, subject to receipt of payment for such work."

**Analysis:** This clause applies to future work. It does NOT relieve the developer of liability for work already paid for but not delivered or defectively delivered.

### Clause 11.5
**Contract Text:** "In the event of any termination of this agreement by the client under clause 10.1, no refunds shall be given under any circumstances whatsoever."

**Analysis:** This is a ONE-SIDED no-refund clause that may be unenforceable under UAE law as an unfair contract term, particularly when the developer has committed material breaches. UAE Civil Code Article 267 permits a court to rescind a contract (with restitution) where one party fails to perform its obligations. Courts may not enforce a clause that allows a party to keep payment for work it never performed.

---

## Clause 13: Assignment of Intellectual Property Rights

### Clause 13.1
**Contract Text:** "On and from the date of delivery of deliverables to the client and subject to receipt of full payment as per Payment Schedule, the Developer hereby assigns to the Client all of its right, title and interest in the IP of application and its deliverables."

**Analysis:** IP assignment is conditional on delivery AND full payment. Since two deliverables were never delivered, the IP assignment for those components never occurred. The client may not have clear IP rights to undelivered components.

### Clause 13.3
**Contract Text:** "The entire source code of the project shall be provided to the Client."

**Analysis:** Source code was apparently provided (it exists in the workspace). However, providing source code that contains hardcoded credentials (database passwords, SMTP credentials, API keys) constitutes a security liability, not a proper handover. A professional developer should sanitize credentials before handover.

---

## Clause 16: Maintenance

**Contract Text:** "The developer will provide 3 (Three) months of Maintenance support to the client from the day the project goes live without any charge. This support will cover App maintenance (Version update, patch update, Data Backup, Data cleaning, Fragmentation etc), bug and technical issues."

**Analysis:** With 147+ documented bugs and defects, the maintenance obligation is significant. The question is whether these are "bugs" requiring maintenance or whether they were never properly developed in the first place (pre-existing defects vs. post-launch issues). Many of the issues (zero authentication, no caching, N+1 queries, missing deliverables) are fundamental design and implementation failures that existed from initial delivery, not bugs that emerged during operation.

---

## Clause 18: Governing Law & Jurisdiction

**Contract Text:** "This Agreement...shall be governed by, and construed in accordance with, the laws of the Emirate of Dubai and the laws of the United Arab Emirates."

**Analysis:** UAE law applies. Relevant provisions include:
- UAE Civil Code (Federal Law No. 5 of 1985) — contract formation, performance, remedies
- UAE Federal Decree-Law No. 34 of 2021 (Cybercrime Law) — potential criminal liability for delivering systems with deliberate security vulnerabilities
- UAE Consumer Protection Law — if applicable to B2B transactions
- Dubai International Arbitration Centre (DIAC) rules — dispute resolution

---

## Clause 19: Dispute Resolution

**Contract Text:** Amicable resolution within 30 days of Dispute Notice, then DIAC arbitration. One arbitrator. Seat: Dubai. Language: English.

**Analysis:** The client must first send a written Dispute Notice with reasonable details. If unresolved within 30 days, submit to DIAC. This report serves as the technical foundation for the Dispute Notice.

---

## Clause 22: Entire Agreement

**Contract Text:** "This Agreement constitutes the entire agreement between the Parties hereto and supersedes all prior representations, understandings, undertakings or agreements (whether oral or written and whether expressed or implied)."

**Analysis:** This clause means Schedule A is the definitive scope. The developer cannot claim features were added or removed based on verbal agreements. Everything in Schedule A was contractually required.

---

# PART 3: SCHEDULE A — DELIVERABLE-BY-DELIVERABLE COMPLIANCE AUDIT

## 3.1 Deliverable B: Technician Application — ENTIRELY MISSING

Schedule A specifies the following features for the Technician Application:

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Login with their credentials | ❌ NOT DELIVERED | No technician app exists at all |
| 2 | View Current Requests, Completed Requests, Pending Requests | ❌ NOT DELIVERED | No technician app exists |
| 3 | Receive notification with all order details when new request assigned by admins | ❌ NOT DELIVERED | No technician app exists |
| 4 | Various stages: On my way, reached, started, finished — client notified at each stage | ❌ NOT DELIVERED | No technician app exists |
| 5 | Update before and after images of service | ❌ NOT DELIVERED | No technician app exists |
| 6 | Switch status (offline/online) | ❌ NOT DELIVERED | No technician app exists |
| 7 | Google Maps API for real-time location | ❌ NOT DELIVERED | No technician app exists |

**Total: 0 out of 7 features delivered. 0% compliance.**

---

## 3.2 Deliverable D: Basic Website — ENTIRELY MISSING

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Basic website with general information about the application | ❌ NOT DELIVERED | Only Flutter web boilerplate and default ASP.NET template page exist |
| 2 | Download link to user application from App Store | ❌ NOT DELIVERED | No website exists |
| 3 | Download link to user application from Play Store | ❌ NOT DELIVERED | No website exists |

**Total: 0 out of 3 features delivered. 0% compliance.**

---

## 3.3 Deliverable A: User Application — PARTIALLY DELIVERED

### 3.3.1 Essential Features

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Day mode / Night mode | ✅ DELIVERED | AdaptiveTheme implementation with Settings toggle |
| 2 | Modern Auto search option on search bar | ⚠️ PARTIAL | Basic search exists in multiple modules; no auto-suggest/autocomplete |
| 3 | Option to interact with admins | ⚠️ PARTIAL | Enquiry form exists; no direct email sending or real-time chat |
| 4 | Easy to order items | ⚠️ PARTIAL | Order flow exists but payment integration is incomplete |
| 5 | Order/Booking status | ✅ DELIVERED | Order status tracking exists in mobile app |
| 6 | English and Arabic Language | ⚠️ PARTIAL | l10n.yaml + AppLocalizations exist but implementation is inconsistent — many screens have hardcoded English strings |
| 7 | Sign up by UAE Pass | ⚠️ PARTIAL | Implemented but using SANDBOX credentials (`clientId: "sandbox_stage"`), not production |
| 8 | Sign up by Gmail | ✅ DELIVERED | google_sign_in package implemented |
| 9 | Sign up by Meta (Facebook) | ✅ DELIVERED | flutter_facebook_auth implemented |
| 10 | Sign up by Twitter | ❌ NOT DELIVERED | Only an icon asset exists (`twitterLogo`); no Twitter sign-in code anywhere |
| 11 | Sign up by Email | ✅ DELIVERED | Email login implemented |
| 12 | Sign up by Phone number | ⚠️ PARTIAL | Code exists but OTP verification paths are commentable/bypassable |
| 13 | Profile View/Edit | ✅ DELIVERED | Profile controller and views exist |

### 3.3.2 Service Booking Features

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Real estate journey | ✅ DELIVERED | Full real estate module exists |
| 2 | Hotel booking (simple form, no integration) | ✅ DELIVERED | Hotel booking form implemented |
| 3 | Flight (simple form, no integration) | ✅ DELIVERED | Flight booking form implemented |
| 4 | Salon (services and packages for home salon) | ✅ DELIVERED | Salon module exists |
| 5 | Motor Service (services and packages for vehicle) | ✅ DELIVERED | Motors module exists |
| 6 | Grocery (products by vendor, customer purchase) | ✅ DELIVERED | Grocery/product module exists |
| 7 | Typing (services listed, customers place request) | ✅ DELIVERED | Typing module exists |
| 8 | Medical (services categorized and displayed) | ⚠️ PARTIAL | Medical routes partially commented out in app routing |
| 9 | Flowers and Chocolates (products by vendors) | ⚠️ PARTIAL | Flowers routes partially commented out in app routing |
| 10 | Push notifications and email on each booking stage | ⚠️ PARTIAL | Push notification service exists but config uses wrong app name ("Mirsal" instead of "Boulevard") |
| 11 | Payment through integrated payment gateway | ⚠️ PARTIAL | MyFatoorah key found but API key is hardcoded in source; no actual payment processing flow verified; payment options are displayed as radio buttons but actual transaction processing is absent |
| 12 | Flight/Insurance quotation system | ⚠️ PARTIAL | Quotation backend exists; insurance UI has hardcoded dummy text ("Al Ain Ahlia provides a comprehensive range...") |
| 13 | Text-based chat system | ❌ COMMENTED OUT | Entire `chat_home_view.dart` is commented out (every line starts with `//`), chat route also commented out |
| 14 | Grocery section with categories/sub-categories | ✅ DELIVERED | Category and product browsing exists |

### 3.3.3 Checkout and Payments

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Payment Gateway Integrated (client's choice) | ⚠️ PARTIAL | MyFatoorah reference found but payment key is hardcoded in client source code — a critical security violation |
| 2 | All security protocols in place | ❌ NOT DELIVERED | SSL validation disabled in mobile app; no pinning; no PCI compliance; payment API key exposed in source |
| 3 | Terms and conditions customer must agree upon payments | ❌ NOT DELIVERED | No terms agreement checkbox found anywhere in payment flow |

### 3.3.4 Other Functionalities

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Push Notifications for various steps/promotions | ⚠️ PARTIAL | Service exists but uses wrong app name "Mirsal"; no global notification handler |
| 2 | Google Maps integrated Location System | ✅ DELIVERED | Google Maps integration present |
| 3 | Offer pop-ups or Offer banners | ⚠️ PARTIAL | Offer banners exist as horizontal scroll strip; NO popup functionality |
| 4 | Email/Reachout to admins | ⚠️ PARTIAL | Enquiry form exists (stored in DB); no actual email sending |
| 5 | Pagination and Breadcrumb in all the pages | ❌ NOT DELIVERED | Admin: PaginatedList broken (no Skip/Take), only MemberController has partial support (1/30). Mobile: no pagination or breadcrumb components found |
| 6 | Terms and condition page | ⚠️ PARTIAL | Referenced in localization files but no dedicated page |
| 7 | Privacy policy page | ❌ NOT DELIVERED | No privacy policy page found |
| 8 | About Us page | ❌ NOT DELIVERED | Only exists as a real estate property field, not standalone |
| 9 | Quick links page | ❌ NOT DELIVERED | Not found anywhere |
| 10 | FAQs page | ✅ DELIVERED | FAQ management in admin; FAQ display in mobile app |
| 11 | Contact us page | ⚠️ PARTIAL | Enquiry form exists but not a traditional Contact Us page |

---

## 3.4 Deliverable C: Admin Panel — PARTIALLY DELIVERED

### 3.4.1 Login & General Features

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Login via username | ✅ DELIVERED | Admin login form exists |
| 2 | Multiple access roles | ❌ BROKEN | Role CRUD exists in admin, but `IsInRole()` throws `NotImplementedException` — any role check crashes the app. No `[Authorize]` attribute exists on ANY controller. Roles cannot be enforced. |
| 3 | Multiple admin access | ⚠️ PARTIAL | Multiple users can be created but no role differentiation works |
| 4 | Manage categories and subscriptions | ✅ DELIVERED | CategoryController with CRUD exists |
| 5 | Order Management | ⚠️ PARTIAL | Order listing exists but no search, no filters |
| 6 | User management (active/password reset) | ⚠️ PARTIAL | User CRUD exists; no password reset for admin users |
| 7 | Users booking/order with payment history | ⚠️ PARTIAL | Order listing by user exists; payment history limited |

### 3.4.2 Dashboard

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Statistics | ✅ DELIVERED | Total customers, orders, sales statistics |
| 2 | Lifetime Sales | ⚠️ PARTIAL | Only current totals shown; no "lifetime" period distinction |
| 3 | Bestraders | ❌ NOT DELIVERED | No best traders/vendors analysis exists |
| 4 | Most Viewed | ❌ NOT DELIVERED | No view tracking or most-viewed analytics exist |
| 5 | New Customers | ✅ DELIVERED | Top 10 newly joined customers table |
| 6 | Last Booked Service | ❌ NOT DELIVERED | No last booked service widget exists on dashboard |
| 7 | Top Customer | ❌ NOT DELIVERED | Only "newly joined" exists, not "top customer" by revenue |
| 8 | Sales Report | ⚠️ PARTIAL | Monthly charts exist but fire 22+ sequential DB queries and load entire tables into memory |
| 9 | Latest Orders | ❌ NOT DELIVERED | No latest orders widget on dashboard |

### 3.4.3 Provider Management

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Provider Listing Table - Add, Edit, Update | ❌ NOT DELIVERED | Zero provider/vendor management code exists anywhere |
| 2 | Search with suggestions | ❌ NOT DELIVERED | Provider management does not exist |
| 3 | Filters | ❌ NOT DELIVERED | Provider management does not exist |
| 4 | Download reports in CSV/XLS | ❌ NOT DELIVERED | Zero export/download functionality exists in the entire system; EPPlus package is included but only used for Excel IMPORT (bulk product upload), and even that code is commented out |

### 3.4.4 Customer Management

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Customer Listing Table | ✅ DELIVERED | MemberController Index with listing |
| 2 | Add/Delete/Edit Customers | ⚠️ PARTIAL | Create/Update exist; Delete not found in MemberController |
| 3 | Search with Suggestions | ⚠️ PARTIAL | Basic search filtering via MemberViewModel; no autocomplete suggestions |
| 4 | Filters | ⚠️ PARTIAL | Limited filtering capability |

### 3.4.5 Reports

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Various types of sales reports | ❌ NOT DELIVERED | No reporting system exists |
| 2 | Number of vendors/providers | ❌ NOT DELIVERED | Provider management does not exist |
| 3 | Booking reports | ❌ NOT DELIVERED | No report generation or export |
| 4 | Order Reports | ❌ NOT DELIVERED | No report generation or export |
| 5 | Print Reports | ❌ NOT DELIVERED | No print functionality exists |

### 3.4.6 Orders/Booking Management

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Orders Listing Table | ✅ DELIVERED | Product and service order listing exists |
| 2 | Invoices | ⚠️ PARTIAL | Invoice view exists in order details |
| 3 | Transactions | ❌ NOT DELIVERED | No separate transaction management |
| 4 | Search with Suggestions with Filters | ❌ NOT DELIVERED | No search or filter on orders |
| 5 | Status of orders | ✅ DELIVERED | Order status tabs and management |

### 3.4.7 Content

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Change Banners | ✅ DELIVERED | WebHtmlController with banner management |
| 2 | Add banners | ✅ DELIVERED | Banner create functionality exists |
| 3 | Offer Banners | ✅ DELIVERED | OfferController with offer banner management |
| 4 | Pop ups | ❌ NOT DELIVERED | No popup management system |
| 5 | Static Page content | ⚠️ PARTIAL | WebHtml management exists but limited |

### 3.4.8 Settings

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Payment Management | ⚠️ PARTIAL | Payment method listing exists but limited management |
| 2 | User Management | ✅ DELIVERED | User CRUD exists |

### 3.4.9 Technician Management (Admin Side)

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | Orders Listing Table | ❌ NOT DELIVERED | No technician management exists in admin |
| 2 | Search with Suggestions | ❌ NOT DELIVERED | No technician management exists |
| 3 | Filters | ❌ NOT DELIVERED | No technician management exists |
| 4 | Download reports in CSV/XLS | ❌ NOT DELIVERED | No export functionality exists anywhere |
| 5 | Status of orders (Processing, Cancelled) | ❌ NOT DELIVERED | No technician management exists |

### 3.4.10 Profile

| # | Required Feature | Status | Evidence |
|---|-----------------|--------|----------|
| 1 | View Profile | ✅ DELIVERED | Admin profile viewing exists |
| 2 | Edit Profile | ✅ DELIVERED | Admin profile editing exists |

### 3.4.11 Technology Requirements

| # | Required Technology | Status | Evidence |
|---|-------------------|--------|----------|
| 1 | Tools: Android Studio, Visual Studio | ✅ COMPLIANT | Android Studio for Flutter; Visual Studio for ASP.NET |
| 2 | Development language: C#, XML, Flutter | ✅ COMPLIANT | C# backend; Flutter mobile app |
| 3 | Database: MsSQL | ✅ COMPLIANT | SQL Server 2022 Express |
| 4 | Realtime Activity: JS and Ajax | ⚠️ PARTIAL | AJAX used for admin polling; no real-time functionality (no SignalR, no WebSocket) |
| 5 | Source code: Full source code to client | ⚠️ PARTIAL | Source code provided but contains hardcoded credentials (security risk) |

---

## 3.5 COMPLIANCE SUMMARY

| Deliverable | Features Specified | Fully Delivered | Partial | Not Delivered | Compliance Rate |
|-------------|-------------------|-----------------|---------|---------------|----------------|
| A. User App (Essential) | 13 | 6 | 6 | 1 | 46% full, 46% partial |
| A. User App (Services) | 14 | 7 | 6 | 1 | 50% full, 43% partial |
| A. User App (Payments) | 3 | 0 | 1 | 2 | 0% full, 33% partial |
| A. User App (Other) | 11 | 2 | 4 | 5 | 18% full, 36% partial |
| B. Technician App | 7 | 0 | 0 | 7 | 0% |
| C. Admin Panel | 37 | 12 | 11 | 14 | 32% full, 30% partial |
| D. Website | 3 | 0 | 0 | 3 | 0% |
| **TOTAL** | **88** | **27** | **28** | **33** | **31% full, 32% partial** |

**Overall: Only 31% of specified features were fully delivered. 37% were not delivered at all.**

---

# PART 4: COMPLETE CATALOGUE OF SYSTEM DEFECTS (147+ Issues)

## Category 1: CATASTROPHIC SECURITY VULNERABILITIES (3 issues)

| # | Issue | Files Affected | Severity |
|---|-------|---------------|----------|
| 1.1 | Complete absence of authentication on ALL 70 controllers (33 API + 37 Admin). No `[Authorize]` attribute anywhere. No global authorization filter. Every endpoint publicly accessible to anonymous users. | All controllers | CATASTROPHIC |
| 1.2 | SSL certificate validation completely disabled in mobile app. `MyHttpOverrides` class in `main.dart` sets `badCertificateCallback` to always return `true`. ALL network traffic vulnerable to MITM attacks. | `main.dart` | CATASTROPHIC |
| 1.3 | MyFatoorah payment API key hardcoded in mobile app source code (`store_key_const.dart`). Anyone who decompiles the APK can use this key. | `store_key_const.dart` | CATASTROPHIC |

## Category 2: CRITICAL SECURITY VULNERABILITIES (25 issues)

| # | Issue | Severity |
|---|-------|----------|
| 2.1 | IDOR on every endpoint that accepts memberId — no ownership verification | CRITICAL |
| 2.2 | Unrestricted file upload (`.aspx`, `.exe`, `.config` accepted) — Remote Code Execution possible | CRITICAL |
| 2.3 | OTP bypass in password reset: setting OTP=0 skips verification entirely | CRITICAL |
| 2.4 | SHA-256 password hashing WITHOUT salt — vulnerable to rainbow table attacks | CRITICAL |
| 2.5 | Hardcoded SMTP credentials: `partners@boulevardsuperapp.com` / `partners@123` (4 locations) | CRITICAL |
| 2.6 | Commented-out DB connection strings exposing production server IP, username, and password | CRITICAL |
| 2.7 | Hardcoded courier API key (`Jeebly123`) with direct string comparison (timing attack vulnerable) | CRITICAL |
| 2.8 | Android keystore password `123456` stored in `key.properties` | CRITICAL |
| 2.9 | Password field included in API login response — sent to mobile app | CRITICAL |
| 2.10 | Passwords stored in unencrypted `SharedPreferences` on mobile device | CRITICAL |
| 2.11 | No authorization headers sent in any API request from mobile app | CRITICAL |
| 2.12 | Stored XSS: `ValidateInput(false)` on 14 admin actions + `Html.Raw` renders user content unsanitized | CRITICAL |
| 2.13 | 12 N+1 query patterns causing exponential database load | CRITICAL |
| 2.14 | Missing database indexes on most-queried columns (Products, ProductCategories, ProductPrices, Brands, ServiceTypes, StockLogs) | CRITICAL |
| 2.15 | Zero caching in entire system — every request hits database directly | CRITICAL |
| 2.16 | 29 out of 30 admin listing endpoints load ALL records without pagination | CRITICAL |
| 2.17 | PaginatedList.CreateAsync calls `ToListAsync()` WITHOUT `Skip`/`Take` — loads all records | CRITICAL |
| 2.18 | `NotoficationService` creates 10 undisposed `BoulevardDbContext` instances | CRITICAL |
| 2.19 | Dashboard fires 22+ sequential database queries per page load | CRITICAL |
| 2.20 | Chart endpoints load ENTIRE orders table into memory 4 separate times | CRITICAL |
| 2.21 | ServiceAmenityAccess loads ENTIRE ServiceAmenity table N times inside a foreach loop | CRITICAL |
| 2.22 | Single API request can cascade into 10-20+ DbContext instances through `new XxxAccess()` chain | CRITICAL |
| 2.23 | Wrong variable returned in third-party login (returns empty object instead of registered member) | CRITICAL |
| 2.24 | Duplicate condition check in order validation: `model.MemberId == 0 || model.MemberId == 0` instead of checking `MemberAddressId` | CRITICAL |
| 2.25 | `GenericRepository.Get()` returns null when no `orderBy` is provided — silently discards all filter results | CRITICAL |

## Category 3: HIGH SEVERITY ISSUES (35 issues)

| # | Issue |
|---|-------|
| 3.1 | Destructive operations (member deletion, cart removal, order cancellation) accessible via GET requests |
| 3.2 | Only 4 out of ALL admin POST actions use `ValidateAntiForgeryToken` (CSRF protection) |
| 3.3 | Connection string injection in Excel bulk upload — password concatenated unsanitized |
| 3.4 | Missing auth headers in mobile app `api_service.dart` — no Bearer token sent |
| 3.5 | `IsInRole()` in `CustomPrincipal` throws `NotImplementedException` — role-based auth crashes |
| 3.6 | Mobile app broken exception handler (`error.message` on enum — crashes on error handling) |
| 3.7 | No request timeout on any mobile API call — UI freezes indefinitely on network issues |
| 3.8 | No retry logic on any API call in mobile app |
| 3.9 | Missing ProGuard/R8 configuration for Android — app can be trivially reverse-engineered |
| 3.10 | No code obfuscation in Flutter build |
| 3.11 | No root/jailbreak detection in mobile app |
| 3.12 | Deep link hijacking vulnerability — no intent filter validation |
| 3.13 | Missing Android `network_security_config.xml` |
| 3.14 | iOS `aps-environment` set to "development" for release builds |
| 3.15 | 10+ controllers have memory leaks — `TextEditingController`, `ScrollController` not disposed in `onClose` |
| 3.16 | Authentication cookie in admin: no `Secure` flag, no `HttpOnly` flag, no `SameSite` flag, 15-day expiration |
| 3.17 | Swagger UI publicly accessible without authentication — full API documentation exposed |
| 3.18 | `FakeController` and `DemoController` exist in production codebase |
| 3.19 | Images saved at 100% quality without compression — 4,235 images potentially 2-5 MB each |
| 3.20 | No thumbnail generation — full-size images served even for 150x150 list views |
| 3.21 | No static file caching or CDN configuration |
| 3.22 | Three (3) copies of jQuery loaded on every admin page |
| 3.23 | 16 CSS files + 28 JS files loaded on every admin page (44+ HTTP requests) |
| 3.24 | Admin header polls server every 5 seconds on EVERY open tab |
| 3.25 | All 35 DataTables instances use client-side processing — all data rendered in DOM |
| 3.26 | No HTTP compression configured in Web.config |
| 3.27 | No HTTPS enforcement or HSTS headers |
| 3.28 | No custom error pages — ASP.NET default pages expose internal paths and framework details |
| 3.29 | `debug="true"` in production Web.config — disables minification, exposes stack traces |
| 3.30 | `DbContext` instantiated directly in Razor view `_Header.cshtml` line 9 — connection leak |
| 3.31 | Layout settings queried 3 times per admin page load (identical query from 3 partial views) |
| 3.32 | IUnitOfWork does NOT extend IDisposable — DbContext never properly disposed |
| 3.33 | Repository `SaveChanges` called per Add/Edit/Remove — defeats Unit of Work pattern |
| 3.34 | No dependency injection container anywhere in the system |
| 3.35 | 30+ locations create raw `BoulevardDbContext` without `using` or `Dispose()` |

## Category 4: MEDIUM SEVERITY ISSUES (50 issues)

| # | Issue |
|---|-------|
| 4.1 | Status field is free-form string with inconsistent values ("Active", "Delete", "Deleted", "Finished", "Pending", "Success") |
| 4.2 | Some code uses `.ToLower() == "Active"` which will NEVER match (lowercased "Active" is "active") |
| 4.3 | `.ToLower()` in LINQ queries prevents SQL index usage (appears in 10+ service files) |
| 4.4 | Synchronous database calls (`Sum`, `Any`) blocking thread pool threads |
| 4.5 | Loading entire tables into memory then filtering in C# instead of SQL |
| 4.6 | Duplicate queries: dashboard metrics each checked with `AnyAsync` then re-queried with `CountAsync` |
| 4.7 | `CartServiceAccess.GetCartListProductsCount`: identical code in both if/else branches |
| 4.8 | Entity models used as ViewModels with 15+ `[NotMapped]` properties (Product model) |
| 4.9 | `MemberId` defined as `long` but cast to `int` via `Convert.ToInt32` throughout (truncation risk) |
| 4.10 | Missing navigation properties forcing manual queries (Product→ProductPrice, Product→ProductImage, etc.) |
| 4.11 | Misspelled properties: `IsTrenbding` (should be IsTrending), `PersoneQuantity` (should be PersonQuantity) |
| 4.12 | `Product.ProductPrice` silently clamps negative values to 0 via `Math.Max(0, value)` instead of validation |
| 4.13 | `OrderRequestService` does NOT inherit `BaseEntity` — missing all audit fields |
| 4.14 | No `[Key]` attribute on entity PKs — relying on EF convention |
| 4.15 | `ParentId` is `int` not `int?` — uses 0 as "no parent" instead of null; no FK constraint possible |
| 4.16 | Category model has lowercase `int label` property — violates C# naming conventions |
| 4.17 | `ServiceType` constructor creates empty `City` and `Country` objects even when not needed |
| 4.18 | String-based payment status fields with 100-char max when only a few values are used |
| 4.19 | `CreateBy = 1` hardcoded in every insert/update — audit trail always shows user 1 |
| 4.20 | Hardcoded feature category IDs (9, 11, 13) in conditional logic without named constants |
| 4.21 | Hardcoded phone prefix "+971" in member registration |
| 4.22 | Exception swallowing in 30+ service methods — catch all, log, return null |
| 4.23 | Repository method typo: `Addd` (triple 'd') on line 35 of GenericRepository |
| 4.24 | `GenericRepository.GetbyId`: inconsistent casing (should be `GetById`) |
| 4.25 | String-based `Include` in GenericRepository — breaks silently when property names change |
| 4.26 | 22+ methods with useless try-catch-throw patterns (catch Exception, rethrow) |
| 4.27 | Inconsistent API response formats across controllers |
| 4.28 | `BaseController.ErrorMessage` always returns HTTP 200 with embedded error code |
| 4.29 | Mixing static and instance method calls in NotificationController |
| 4.30 | Hardcoded base URL in mobile app (`urls.dart`) with no environment switching |
| 4.31 | Non-reactive internet connectivity check in mobile app |
| 4.32 | Only HTTP 200 status code handled in mobile app — all others treated as generic error |
| 4.33 | No HTTP interceptors for common concerns (auth, logging, error handling) |
| 4.34 | No consistent architecture pattern in mobile app — controllers directly call API classes |
| 4.35 | Incomplete dependency injection in mobile app — only 3 services registered with GetX |
| 4.36 | Controllers are God Objects — direct API calls, state management, navigation, error handling all in one |
| 4.37 | Direct API calls in view layer (bypassing controllers) — `MotorsApi().onAddRemoveService()` in widget build |
| 4.38 | Tight inter-module coupling — controllers reach across module boundaries |
| 4.39 | Inconsistent navigation pattern — mixed named routes and anonymous navigation |
| 4.40 | No global error handling in mobile app — no Crashlytics or Sentry |
| 4.41 | 20+ TODO comments in production code indicating incomplete features |
| 4.42 | All model `fromJson` methods — no null checks or type validation |
| 4.43 | Mixed localization approach — some screens use AppLocalizations, others have hardcoded English |
| 4.44 | No responsive design for tablets — fixed widths with flutter_screenutil |
| 4.45 | iOS permission descriptions have grammar errors ("We are used this library for profile picture change") |
| 4.46 | `flutter_html: any` version spec — accepts any version including breaking changes |
| 4.47 | Wrong app name in notifications: "Mirsal" instead of "Boulevard" |
| 4.48 | Hardcoded contact information (phone number) in 4+ widget files |
| 4.49 | Service layer contains presentation logic (URL prefixing with `HttpContext.Current.Request.Url`) |
| 4.50 | Service layer contains localization logic (language switching in data access methods) |

## Category 5: LOW SEVERITY ISSUES (34+ issues)

| # | Issue |
|---|-------|
| 5.1 | Public fields on GenericRepository (`_dbContext`, `_dbSet`) — should be private/protected |
| 5.2 | Public field on UnitOfWork (`_dbContext`) — breaks encapsulation |
| 5.3 | Missing repository properties in IUnitOfWork (MonthlyGoal, GolbalMemberCategory, TempProduct, etc.) |
| 5.4 | Duplicate region comments — three regions all named `#region notification` in UnitOfWork |
| 5.5 | Massive God-class UnitOfWork with 50+ repository properties (~1100 lines) |
| 5.6 | God-class service files: ServiceAccess.cs (1,804 lines), CategoryServiceAccess.cs (834 lines) |
| 5.7 | Hardcoded fake test data in production ServiceAccess (GetAllMotorServices, GetAllSalonServices) |
| 5.8 | Async methods performing no async work |
| 5.9 | Dead code throughout codebase |
| 5.10 | No unit tests — only single default widget_test.dart |
| 5.11 | No integration tests |
| 5.12 | DbContext initialized as field instead of in constructor (UnitOfWork line 13) |
| 5.13 | Service layer anti-pattern: `new BoulevardDbContext()` bypasses UnitOfWork entirely in 8+ methods |
| 5.14 | `UserAccess.GetUserByAuth` is async but uses sync `FirstOrDefault()` |
| 5.15 | 14 admin views load jQuery 1.7.1 over insecure HTTP |
| 5.16 | Unused libraries loaded on every admin page (fullcalendar, jvectormap, sparkline, d3) |
| 5.17 | Dashboard duplicates chartist from CDN on top of local copy |
| 5.18 | No `async`/`defer` on any script tag |
| 5.19 | Zero lazy loading on images across all 276 admin cshtml files |
| 5.20 | No AJAX debouncing on search inputs |
| 5.21 | Excessive ViewBag usage instead of strongly-typed ViewModels |
| 5.22 | Dropdown data loaded without limits — all records for select elements |
| 5.23 | No empty state messages in mobile app list views |
| 5.24 | No skeleton/shimmer loading states — BotToast global overlay blocks entire UI |
| 5.25 | Generic error messages ("Something went wrong") without context |
| 5.26 | No offline support in mobile app |
| 5.27 | No token refresh flow |
| 5.28 | Missing `const` constructors throughout Flutter widget code |
| 5.29 | Heavy widget trees (200+ line build methods) without extraction |
| 5.30 | `Column(children: list.map().toList())` instead of `ListView.builder` for long lists |
| 5.31 | Debug `print()` statements in build methods and production code |
| 5.32 | Missing `Obx` wrappers for reactive state in some views |
| 5.33 | No lazy loading for module resources — all modules loaded at app start |
| 5.34 | Notification polling every 10 seconds per admin tab |

---

# PART 5: BREACH OF PROFESSIONAL STANDARDS (Clause 7.1)

## 5.1 What Clause 7.1 Requires

The developer warranted it would perform services "in a professional and competent manner." Under UAE law and international software development standards, this means:

### OWASP Top 10 Compliance
The Open Web Application Security Project (OWASP) Top 10 is the internationally recognized minimum security standard. The delivered system violates ALL 10 categories:

| OWASP Category | Boulevard Violation |
|----------------|-------------------|
| A01: Broken Access Control | Zero authentication on 70 controllers; IDOR on all member endpoints |
| A02: Cryptographic Failures | SHA-256 without salt; plaintext credentials in source; passwords in API responses |
| A03: Injection | Connection string injection in Excel upload; stored XSS via `ValidateInput(false)` + `Html.Raw` |
| A04: Insecure Design | No threat modeling; Repository pattern defeats its own purpose; no dependency injection |
| A05: Security Misconfiguration | `debug="true"` in production; Swagger UI public; no HTTPS; no HSTS; no custom error pages |
| A06: Vulnerable Components | jQuery 1.7.1 (2011) loaded over HTTP; `flutter_html: any` version wildcard |
| A07: Identification/Authentication Failures | No auth enforcement; OTP bypass; weak hashing; 15-day auth cookies without flags |
| A08: Software/Data Integrity Failures | No ProGuard; no code obfuscation; no root detection; no integrity checks |
| A09: Security Logging/Monitoring Failures | Exception swallowing in 30+ locations; no audit trail (`CreateBy` always hardcoded to 1) |
| A10: Server-Side Request Forgery | SSL validation disabled globally in mobile app |

### Software Engineering Standards

| Standard | Requirement | Boulevard Status |
|----------|-------------|-----------------|
| Separation of Concerns | Each layer handles one responsibility | VIOLATED: Views query database directly; Service layer builds URLs; Controllers are God Objects |
| Unit of Work Pattern | Batch operations into transactions | VIOLATED: SaveChanges called per individual operation |
| Dependency Injection | Loose coupling via interfaces | ABSENT: Every class uses `new` keyword; zero DI container |
| Connection Management | DbContext properly scoped and disposed | VIOLATED: 30+ undisposed contexts; 10-20 per request |
| Input Validation | Validate all external input | ABSENT: No file type validation; no parameter validation; no model validation |
| Error Handling | Graceful degradation with proper error types | VIOLATED: Exception swallowing returns null; generic error messages |
| Testing | Automated test coverage | ABSENT: Zero tests (single default widget_test.dart) |
| Performance | Efficient data access patterns | VIOLATED: 12 N+1 patterns; zero caching; zero pagination |
| Code Quality | Clean, maintainable code | VIOLATED: 1,804-line God classes; typos in method names; dead code in production |

---

# PART 6: LEGAL BASIS FOR CLAIMS

## 6.1 Contractual Claims

### A. Material Breach of Contract (Clause 1 + Schedule A)
The developer failed to deliver 2 out of 4 contracted deliverables (Technician App and Website) and delivered the remaining 2 with significant missing features. This constitutes a material breach under Clause 10.1(a) of the agreement.

### B. Breach of Warranty (Clause 7.1)
The developer warranted "professional and competent" performance. Delivering a system with:
- Zero authentication
- Hardcoded passwords including `sa/123456`
- SSL validation disabled
- Remote Code Execution via file upload
- 147+ documented defects

...constitutes a clear breach of the professional competence warranty.

### C. Persistent Breach (Clause 10.1(c))
With 147+ defects spanning all system components, the breaches are persistent and pervasive, satisfying the "persistent breach" termination ground.

## 6.2 UAE Civil Code Claims

### A. Contract Rescission (Article 267)
UAE Civil Code Article 267 permits a court to rescind a bilateral contract when one party fails to perform its obligations. The client may seek rescission with restitution (return of payments) where the developer has fundamentally failed to perform.

### B. Compensation for Damages (Articles 282-292)
The client is entitled to compensation for:
- Direct damages: cost of the contract (40,950 AED)
- Consequential damages: cost of remediation by a competent developer
- Lost opportunity: delay in launching a functional platform

### C. Obligation to Perform in Good Faith (Article 246)
A party must perform its obligations in a manner consistent with good faith. Delivering a system with zero security and multiple missing deliverables while accepting full payment is arguably not performance in good faith.

## 6.3 Potential Criminal Liability

### UAE Cybercrime Law (Federal Decree-Law No. 34 of 2021)
The deliberate delivery of a system with:
- No authentication (allowing unauthorized access to user data)
- Hardcoded credentials in source code (facilitating unauthorized access)
- SSL validation disabled (enabling data interception)
- File upload with no restrictions (enabling server compromise)

...may constitute criminal negligence regarding data protection and cybersecurity under UAE law, particularly if user data has been compromised as a result.

## 6.4 Clause 8.1 Exception to Liability Limitations
The agreement states: "Nothing in this Agreement will limit or exclude any liability for fraud or fraudulent misrepresentation."

If the developer represented the system as complete and production-ready when it knew:
- Two deliverables were not built
- The system had zero authentication
- Credentials were hardcoded
- The payment API key was exposed

...this may constitute fraudulent misrepresentation, removing the liability cap entirely.

---

# PART 7: DAMAGE ASSESSMENT AND QUANTIFICATION

## 7.1 Direct Financial Loss

| Item | Amount (AED) |
|------|-------------|
| Total contract value paid | 40,950 |
| Value of undelivered Technician App (25% of deliverables) | 10,237.50 |
| Value of undelivered Website (25% of deliverables) | 10,237.50 |
| Subtotal — entirely undelivered | 20,475.00 |

## 7.2 Remediation Costs (Estimated)

Based on the 147+ defects documented across 6 independent audit reports:

| Remediation Area | Estimated Effort | Estimated Cost (AED) |
|-----------------|-----------------|---------------------|
| Security remediation (authentication, encryption, credential rotation, file upload security, CSRF, XSS, OTP fix) | 4-6 weeks | 30,000 - 45,000 |
| Architecture remediation (DI container, repository pattern fix, query optimization, caching layer) | 4-6 weeks | 30,000 - 45,000 |
| Missing feature implementation (Chat, Reports, Provider Mgmt, Technician Mgmt, Pagination, Breadcrumbs, Static Pages) | 4-6 weeks | 30,000 - 45,000 |
| Technician App development (from scratch) | 6-8 weeks | 40,000 - 60,000 |
| Website development (from scratch) | 1-2 weeks | 5,000 - 10,000 |
| Performance optimization (N+1 fixes, indexes, caching, frontend optimization) | 2-3 weeks | 15,000 - 25,000 |
| Testing (unit tests, integration tests, security testing) | 2-3 weeks | 15,000 - 25,000 |
| **Total estimated remediation** | **23-34 weeks** | **165,000 - 255,000** |

## 7.3 Data Breach Risk Assessment

The system in its current state poses ongoing risk due to:
- Any person can access all API endpoints without authentication
- Member data (names, addresses, phone numbers, orders) is publicly accessible
- The file upload endpoint could be used to deploy malware on the server
- SMTP credentials could be used to send phishing emails from the company domain
- Database credentials could be used for unauthorized database access

---

# PART 8: RECOMMENDED LEGAL ACTIONS

## 8.1 Immediate Actions

1. **Dispute Notice Under Clause 19.1:** Send written Dispute Notice to ROYEX Technologies LLC at their registered address (3203, The Citadel Tower, 32nd Floor, Marasi Dr, Business Bay, Dubai) via registered mail and email (info@royex.net). Include:
   - Reference to Agreement RX-23850
   - Summary of breaches (this report as attachment)
   - Demand for refund of full contract value (40,950 AED)
   - Demand for compensation for remediation costs
   - 30-day resolution deadline as required by Clause 19.1

2. **URGENT: Take system offline** — the delivered system has zero authentication and critical security vulnerabilities. If it is publicly accessible, all user data is at risk. Taking it offline protects against data breach liability.

3. **Credential rotation** — immediately change all credentials exposed in source code:
   - SMTP password for `partners@boulevardsuperapp.com`
   - Database password
   - MyFatoorah API key
   - Courier API key

## 8.2 If Amicable Resolution Fails (After 30 Days)

4. **DIAC Arbitration** under Clause 19.2:
   - File Request for Arbitration at Dubai International Arbitration Centre
   - One arbitrator, English language, Dubai seat
   - Claims: breach of contract, breach of warranty, rescission with restitution, damages

5. **Claims to pursue:**
   - Full refund: 40,950 AED
   - Remediation costs: 165,000 - 255,000 AED
   - Consequential damages: lost business opportunity, delay
   - Legal and arbitration costs

## 8.3 Evidence Preservation

6. **Preserve all evidence:**
   - All source code as delivered (create timestamped backup)
   - All communications with the developer
   - All payment records and invoices
   - This analysis report and all supporting audit reports
   - Screenshots of the system in its current state

---

# PART 9: EVIDENCE INDEX

## 9.1 Agreement Documents

| Document | Location |
|----------|----------|
| Full Agreement with Schedule A | `SourceCode of ADmin/Boulevard/Boulevard/agreement.md` |

## 9.2 Technical Audit Reports

| Report | Location | Issues Documented |
|--------|----------|-------------------|
| System Audit Report | `reports/BOULEVARD_SYSTEM_AUDIT_REPORT.md` | 60+ issues across 12 categories |
| Unified System Audit Report | `reports/BOULEVARD_UNIFIED_SYSTEM_AUDIT_REPORT.md` | 147+ issues across 18 categories |
| Deep Architecture Audit | `reports/DEEP_ARCHITECTURE_AUDIT.md` | Repository, UoW, Service layer deep analysis |
| Caching Analysis Report | `reports/CACHING_ANALYSIS_REPORT.md` | Zero caching documented with 19 findings |
| Frontend Performance Audit | `reports/FRONTEND_PERFORMANCE_AUDIT.md` | Admin pagination, views, assets — 20+ critical issues |
| Performance Report (Arabic) | `reports/PERFORMANCE_REPORT.md` | Comprehensive Arabic performance analysis |

## 9.3 Key Source Code Evidence Files

### Security Evidence

| Evidence | File Path |
|----------|-----------|
| Zero `[Authorize]` on all controllers | All files in `Controllers/` and `Areas/Admin/Controllers/` |
| SSL validation disabled | `Source Code (App)/boulevard-app/boulevard-app/lib/main.dart` |
| Hardcoded payment API key | `Source Code (App)/boulevard-app/boulevard-app/lib/core/constants/store_key_const.dart` |
| Hardcoded SMTP credentials | `Helper/EmailService.cs` (lines 42, 78, 112, 131) |
| Database credentials exposed | `Web.config` (commented-out connection strings) |
| Hardcoded courier API key | `Controllers/CourierController.cs` (line 44) |
| OTP bypass | `Controllers/MemberController.cs` (line 226) |
| SHA-256 without salt | `Helper/HashConfig.cs` |
| Unrestricted file upload | `Controllers/UploadController.cs` + `Helper/ImageProcess.cs` |
| Keystore password 123456 | `Source Code (App)/boulevard-app/boulevard-app/android/key.properties` |
| IsInRole throws NotImplementedException | `App_Start/CustomPrincipal.cs` (line 33-35) |
| No auth headers in API calls | `Source Code (App)/boulevard-app/boulevard-app/lib/core/services/api_service.dart` |
| Password in login response | `Source Code (App)/boulevard-app/boulevard-app/lib/modules/common/sign_in/models/login_response_model.dart` |

### Missing Deliverables Evidence

| Evidence | File Path |
|----------|-----------|
| Only ONE Flutter project exists (User App) | `Source Code (App)/boulevard-app/boulevard-app/` (no second app for technicians) |
| No website files | No standalone website project in workspace |
| Chat ENTIRELY commented out | `Source Code (App)/boulevard-app/boulevard-app/lib/modules/medical/views/chat_home_view.dart` |
| Twitter sign-in NOT implemented | Only icon asset at `assets_manager.dart`; no sign-in code |
| No provider management | Zero provider/vendor controllers in `Areas/Admin/Controllers/` |
| No report export (CSV/XLS) | Zero export code found; EPPlus only used for import (and that's commented out) |
| No technician management in admin | Zero technician management code in admin |
| Broken PaginatedList | `Areas/Admin/Pagination/PaginatedList.cs` (line 29: `ToListAsync()` without `Skip`/`Take`) |
| Wrong app name in notifications | `Source Code (App)/boulevard-app/boulevard-app/.../notification_service.dart` (line 44: "Mirsal") |

### Architecture Evidence

| Evidence | File Path |
|----------|-----------|
| No DI container | No IoC registration in any startup file |
| DbContext not IDisposable via interface | `BaseRepository/IUnitOfWork.cs` |
| SaveChanges per repo operation | `BaseRepository/GenericRepository.cs` (lines 26-29, 33-36, 55-58) |
| God class 1804 lines | `Service/ServiceAccess.cs` |
| Test data in production | `Service/ServiceAccess.cs` (lines 160-230: hardcoded fake services) |
| FakeController in production | `Controllers/FakeController.cs` |
| DemoController in production | `Controllers/DemoController.cs` |
| 3x layout DB calls per page | `Areas/Admin/Views/Shared/_Layout.cshtml`, `_Header.cshtml`, `_Scripts.cshtml` |
| DbContext in Razor view | `Areas/Admin/Views/Shared/_Header.cshtml` (line 9) |
| Zero tests | `Source Code (App)/boulevard-app/boulevard-app/test/widget_test.dart` (only file, single test) |

---

# APPENDIX A: ISSUE SEVERITY CLASSIFICATION

| Severity | Count | Description |
|----------|-------|-------------|
| CATASTROPHIC | 3 | Complete system-level security failures that make the system dangerous to operate |
| CRITICAL | 25 | Issues that will cause production failures, data leaks, or severe performance degradation |
| HIGH | 35 | Significant security or performance issues that degrade system quality |
| MEDIUM | 50 | Poor practices, missing features, and code quality issues that compound over time |
| LOW | 34+ | Maintainability, naming, documentation, and minor quality issues |
| **TOTAL** | **147+** | |

---

# APPENDIX B: DELIVERABLE COMPLIANCE SCORE

| Metric | Value |
|--------|-------|
| Total features specified in Schedule A | 88 |
| Features fully delivered | 27 (31%) |
| Features partially delivered | 28 (32%) |
| Features not delivered at all | 33 (37%) |
| Deliverables entirely missing | 2 out of 4 (50%) |
| Security vulnerabilities | 63 (catastrophic + critical + high) |
| Architecture defects | 84+ (medium + low) |
| Total documented defects | 147+ |

---

**END OF REPORT**

*This report is prepared for legal proceedings and is based on forensic analysis of the source code, configuration files, and documentation as delivered. All findings reference specific files, line numbers, and code samples that can be independently verified.*
