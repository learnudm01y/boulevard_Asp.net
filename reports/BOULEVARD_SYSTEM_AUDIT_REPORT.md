# Boulevard System — Comprehensive Audit Report (Text Only)

**Date:** March 12, 2026  
**Scope:** Full system audit — 75 database tables, 70+ service files, 37 controllers, all views, all API endpoints, all data models, all configuration files  
**Technology Stack:** ASP.NET MVC 5 + Web API 2 (.NET Framework 4.7.2), Entity Framework 6 (Code First), SQL Server 2022 Express  
**Total Issues Found:** 60+, classified across 12 categories

---

## Table of Contents

1. [Security Vulnerabilities](#1-security-vulnerabilities)
2. [Database Performance Issues](#2-database-performance-issues)
3. [Entity Framework and Query Patterns](#3-entity-framework-and-query-patterns)
4. [Dashboard Performance](#4-dashboard-performance)
5. [Caching Strategy (Complete Absence)](#5-caching-strategy)
6. [Image and File Processing](#6-image-and-file-processing)
7. [Frontend and Static Assets](#7-frontend-and-static-assets)
8. [Pagination (Missing Across the System)](#8-pagination)
9. [API Layer Issues](#9-api-layer-issues)
10. [Architecture and Clean Code Violations](#10-architecture-and-clean-code-violations)
11. [Data Model and Structure Issues](#11-data-model-and-structure-issues)
12. [Template Compilation and Server Configuration](#12-template-compilation-and-server-configuration)

---

## 1. Security Vulnerabilities

### 1.1 Complete Absence of Authentication on All API and Admin Controllers

**Problem:** There is no Authorize attribute on any controller or action method across the entire system. This includes all 32 Web API controllers and all Admin area controllers. There is no global authorization filter registered in FilterConfig or WebApiConfig. The FormsAuthentication cookie is created upon admin login, but no controller actually checks for it.

Every sensitive endpoint is publicly accessible to anonymous users. This means anyone who knows the URL structure can delete members, view order histories, modify payment statuses, upload files, access the full admin dashboard, create or modify products, and perform any administrative operation without any form of authentication.

**Solution:** Register a global authorization filter in both FilterConfig for MVC controllers and WebApiConfig for Web API controllers. This will require all requests to be authenticated by default. Then, apply the AllowAnonymous attribute only on endpoints that must remain public, such as the login page, the registration endpoint, and public-facing API endpoints like product listing or country lookup. For the admin area specifically, ensure FormsAuthentication is enforced, and for the Web API, implement JWT or token-based authentication so that mobile app users must present valid credentials.

---

### 1.2 Insecure Direct Object References (IDOR)

**Problem:** Every endpoint that accepts a member ID or user ID as a parameter trusts the client-supplied value without verifying that the currently authenticated user is the owner of that ID. This means any user can pass a different member ID to view, modify, or delete another user's data. Affected operations include viewing member details, deleting members, viewing member addresses, viewing cart contents, viewing order histories, reading notifications, and reading customer enquiries.

**Solution:** The member or user ID should never come from the request body or query string for operations on the current user's data. Instead, extract the authenticated user's ID from the authentication token or session. Before performing any operation, verify that the authenticated user owns the resource being accessed. For admin operations, verify that the user has an admin role before allowing access to other users' data.

---

### 1.3 Unrestricted File Upload Leading to Remote Code Execution

**Problem:** The file upload endpoint accepts files of any type without validation. The upload handler in ImageProcess preserves the original file extension provided by the client. Files are saved directly inside the web root under the Content/Upload directory. There is no authentication on the upload endpoint, no file type whitelist, no content-type verification, and no file size limits. An attacker could upload a web shell file with an ASPX extension, and IIS would execute it, giving the attacker full control of the server.

**Solution:** Implement a strict whitelist of allowed file extensions, permitting only safe formats such as JPG, JPEG, PNG, GIF, PDF, DOC, DOCX, and XLSX. Validate both the file extension and the actual content-type header. Save uploaded files outside the web root directory or in a location configured to not execute scripts. Add a file size limit both in Web.config and in the upload handler. Require authentication before allowing any file upload.

---

### 1.4 Hardcoded Credentials in Source Code

**Problem:** SMTP email credentials are written directly in the EmailService helper class in four separate locations. The email password "partners@123" for the account "partners@boulevardsuperapp.com" is visible in plain text. The Web.config file contains commented-out connection strings that expose the production database server IP address, the database username, and the database password. The CourierController contains a hardcoded API key comparison where the key "Jeebly123" is written directly in the source code.

**Solution:** Move all credentials to the appSettings section of Web.config or to environment variables. In production, use a secrets management solution such as Azure Key Vault or encrypted configuration sections. Remove all commented-out connection strings that contain credentials from Web.config. For the courier API key, store it in configuration and use a constant-time comparison function to prevent timing attacks.

---

### 1.5 OTP Bypass in Password Reset Flow

**Problem:** The password reset flow has a critical logic flaw. An attacker can skip the OTP verification step entirely by sending the email address and a new password with the OTP value set to zero. The system interprets this as a valid password reset request without requiring OTP validation. Furthermore, the OTP value is returned in the API response when the Member entity is serialized, meaning an attacker can read the OTP directly from the response without needing access to the user's phone or email.

**Solution:** Implement server-side state management for the password reset flow. After the OTP is verified, set a time-limited flag on the server (such as an OTPVerified boolean with an expiration timestamp) that must be checked before allowing the password change. Never return the OTP value in API responses. Exclude sensitive fields like OTPNumber, Password hash, and SecurityToken from all API response serialization.

---

### 1.6 Weak Password Hashing (SHA-256 Without Salt)

**Problem:** The system hashes passwords using SHA-256 without any salt. This means two users with the same password will have identical hash values, making the system vulnerable to rainbow table attacks and precomputed hash lookups. SHA-256 is a general-purpose hash function that is extremely fast, allowing attackers to try billions of password combinations per second with modern hardware.

**Solution:** Replace SHA-256 with a purpose-built password hashing algorithm such as bcrypt, scrypt, or PBKDF2. Each user's password must be hashed with a unique, randomly generated salt. The work factor should be set high enough that hashing takes at least 100 milliseconds. When migrating, implement a strategy where existing passwords are re-hashed on next successful login.

---

### 1.7 Stored Cross-Site Scripting (XSS)

**Problem:** The ValidateInput attribute is set to false on 14 admin controller actions, which disables ASP.NET's built-in request validation. Combined with the use of Html.Raw in views to render user-provided content (such as product descriptions, membership benefits, user report comments, and delivery information), this creates stored XSS vulnerabilities. Since the API endpoints have no authentication, an attacker can inject JavaScript code through the API, and it will execute in the admin user's browser when they view the data.

**Solution:** Remove ValidateInput(false) from all actions where it is not strictly necessary. Where rich HTML content must be accepted (such as product descriptions), use a server-side HTML sanitization library to strip dangerous tags and attributes before saving to the database. Replace Html.Raw with Html.Encode in views wherever the content does not require HTML rendering. For content that must be rendered as HTML, sanitize it both on input and on output.

---

### 1.8 Destructive Operations Accessible via GET Requests

**Problem:** Several destructive operations are marked as HTTP GET or have no HTTP method attribute (which defaults to GET). These include member deletion, address removal, cart removal, order cancellation, push notification sending, and payment status updates. GET requests can be triggered by simply clicking a link, loading an image tag, or being included in a phishing email, making these operations vulnerable to Cross-Site Request Forgery attacks.

**Solution:** Change all destructive operations to use HTTP POST, PUT, or DELETE methods with the appropriate attributes. Add ValidateAntiForgeryToken to all POST actions in the admin area. For API endpoints, CSRF protection comes from proper authentication tokens (such as JWT in headers rather than cookies).

---

### 1.9 Missing CSRF Protection on Admin Forms

**Problem:** Out of all the admin controller POST actions, only 4 use the ValidateAntiForgeryToken attribute. All other admin forms for creating users, categories, brands, countries, cities, web content, and most other operations have no CSRF protection.

**Solution:** Add the ValidateAntiForgeryToken attribute to all POST actions in the admin area. Ensure that all admin forms include the AntiForgeryToken helper in their Razor views. Consider registering a global CSRF filter for the admin area to prevent any form submissions without a valid token.

---

### 1.10 Connection String Injection in Excel Upload

**Problem:** The Excel bulk upload feature in the Product controller takes a password value from the form request and concatenates it directly into an OleDb connection string without any sanitization. An attacker could inject additional connection string parameters to redirect the data source or modify the connection behavior.

**Solution:** Validate and sanitize the password parameter before including it in the connection string. Use a connection string builder that safely handles parameter values. Better yet, eliminate the need for password-protected Excel files or handle the password through a separate, validated channel.

---

### 1.11 Additional Security Issues

The system runs with debug mode enabled in Web.config, which exposes detailed error pages and stack traces to users. There are no custom error pages configured, so ASP.NET default error pages reveal internal paths and framework details. HTTPS is not enforced anywhere in the application — there is no RequireHttps attribute, no HSTS headers, and no URL rewrite rules. The authentication cookie is set without the Secure, HttpOnly, or SameSite flags, and has an excessively long 15-day expiration. The Swagger UI is unconditionally enabled and publicly accessible, providing attackers a complete map of all API endpoints. The IsInRole method in CustomPrincipal throws a NotImplementedException, which means any attempt to implement role-based authorization will crash the application. Test controllers (FakeController and DemoController) are present in the production codebase. Internal exception messages are returned to clients in the CourierController error handling.

**Solution:** Set debug to false in Web.config for production environments. Configure custom error pages to display user-friendly error messages while logging the details server-side. Enforce HTTPS with the RequireHttps attribute or URL rewrite rules, and add HSTS headers. Set the authentication cookie with Secure, HttpOnly, and SameSite flags, and reduce the expiration to a reasonable duration. Conditionally enable Swagger only in development environments. Implement the IsInRole method properly by checking user roles from the database or claims. Remove FakeController and DemoController from production code. Replace exception message exposure with generic error responses while logging the actual exception internally.

---

## 2. Database Performance Issues

### 2.1 Missing Indexes on Heavily Queried Columns

**Problem:** The largest tables in the database (ProductCategories with 15,535 rows, ProductPrices with 8,959 rows, Products with 4,369 rows) have only Primary Key Clustered Indexes. There are no non-clustered indexes on the columns most frequently used in WHERE clauses, including Products.Status, Products.FeatureCategoryId, Products.BrandId, ProductCategories.ProductId, ProductCategories.CategoryId, ProductCategories.Status, ProductPrices.ProductId, and ProductPrices.Status. This forces SQL Server to perform full table scans on every product-related query.

**Solution:** Create non-clustered indexes on the most-queried column combinations. The Products table needs a composite index on Status and FeatureCategoryId with includes for BrandId, ProductName, ProductPrice, and Image. The ProductCategories table needs indexes on both ProductId with Status, and CategoryId with Status including ProductId. The ProductPrices table needs an index on ProductId and Status including ProductStock, Price, and ProductQuantity. Similarly, indexes should be added to the Brands, ServiceTypes, and StockLogs tables on their most-queried columns. These indexes should be created directly on the SQL Server and can typically be added without downtime.

---

### 2.2 Dashboard Fires 22+ Sequential Database Queries

**Problem:** The DashboardDataAccess.GetAll method fires more than 18 sequential database queries every time the admin dashboard page is loaded. Each statistic (total customers, total product orders, total service orders, total product sales, total service sales, and more) is fetched with a separate query. On top of that, a foreach loop iterates over each feature category, executing two additional queries per category (product count and service count), adding another 16 queries for 8 categories. Then, the DashboardController has four separate chart data endpoints, each of which loads the entire orders table into memory using ToListAsync, and then filters and processes the data in C# instead of SQL. This means the full orders table is loaded into memory 4 separate times.

**Solution:** Consolidate the dashboard statistics into a small number of efficient queries. Use SQL GROUP BY statements to calculate per-category counts in a single query instead of looping. Use SumAsync and CountAsync instead of loading entire tables. For the chart data, use SQL GROUP BY with MONTH to calculate monthly totals directly in the database rather than loading all records and processing them in C#. Consider caching the dashboard results for 5 to 10 minutes since dashboard statistics do not need to be real-time. All independent queries should run in parallel using Task.WhenAll rather than sequentially.

---

## 3. Entity Framework and Query Patterns

### 3.1 N+1 Query Patterns (12 Documented Instances)

**Problem:** The codebase contains at least 12 documented instances of the N+1 query pattern, where a loop iterates over a result set and fires one or more database queries per item. Notable instances include the product listing page where upsell and crosssell checks fire 2 queries per product (100 products = 201 queries), the service listing where parent service names are fetched one by one, the service type loading inside a foreach loop, the product stock calculation querying ProductPrices per product, the service amenity loading which loads the entire ServiceAmenity table inside a loop for each service, the order request data access which fires 4 queries per order for member, address, payment method, and order status, and the dashboard category statistics loop. The most severe case is the ServiceAmenityAccess which loads the entire table N times inside a loop and then filters in memory.

**Solution:** For each N+1 pattern, the fix follows the same principle: load all the required related data in a single batch query before the loop, then match results in memory. For example, instead of querying upsell features per product, load all upsell feature IDs for the given product in one query, then check membership using a HashSet inside the loop. For order data, use Include or explicit joins to load member, address, and payment method data alongside orders in a single query. For the catastrophic ServiceAmenity loading, query only the amenities matching the list of service IDs once, rather than loading the entire table for each service.

---

### 3.2 While Loop Querying Database Per Iteration (Category Hierarchy)

**Problem:** When inserting a product into a category, the code walks up the category hierarchy using a while loop. Each iteration queries the database for the parent category and inserts a ProductCategory record. For a hierarchy 5 levels deep, this generates 5 separate queries and 5 separate insert-and-save operations.

**Solution:** Load the entire category hierarchy into memory once (or use the cached category tree), then traverse the in-memory tree to find all parent categories. Batch all the ProductCategory insert operations and save them in a single transaction at the end.

---

### 3.3 Multiple DbContext Instances Per Request (30+ Locations)

**Problem:** Instead of using the UnitOfWork consistently, more than 30 locations in the codebase create new BoulevardDbContext instances directly. The NotoficationService alone creates a new DbContext in 10 different methods, none of which use using statements or call Dispose. Other files including CommunitySetupAccess, MemberServiceAccess, TempServiceTypeDataAccess, ProductAccess, LayoutSettingAccess, and many others also create standalone contexts. Each new DbContext opens a new database connection, and since none are disposed, connections remain open until the garbage collector intervenes. A single HTTP request can easily result in 9 to 20 open database connections.

**Solution:** Implement dependency injection using a container such as Autofac or Unity, registering BoulevardDbContext with a per-request lifetime scope. This ensures a single DbContext is shared across all services within one HTTP request and is properly disposed at the end of the request. In the immediate term, wrap all standalone DbContext allocations in using statements to ensure proper disposal.

---

### 3.4 ToLower in LINQ Queries Prevents Index Usage

**Problem:** Many LINQ queries use .ToLower() on entity properties before comparison, which translates to the SQL LOWER() function. This function call prevents SQL Server from using indexes on those columns. This pattern appears in ProductAccess, CategoryAccess, CityAccess, CountryAccess, MemberAddressAccess, and several other service files. The comparison .Status.ToLower() == "active" generates SQL that forces a full table scan regardless of any indexes present.

**Solution:** SQL Server uses case-insensitive collation by default (SQL_Latin1_General_CP1_CI_AS), so the .ToLower() call is unnecessary. Remove all .ToLower() calls from LINQ queries and compare directly with the expected casing. Verify the database collation is case-insensitive, and if so, direct string comparisons will work correctly and allow index usage.

---

### 3.5 Synchronous Blocking Database Calls

**Problem:** Several database operations in the DashboardDataAccess use synchronous methods (Sum, Any) instead of their async counterparts (SumAsync, AnyAsync). When synchronous database calls execute in an ASP.NET request pipeline, they block a thread pool thread, reducing the server's ability to handle concurrent requests. Additionally, some dashboard metrics execute the same query twice — once with AnyAsync to check if data exists, then again with CountAsync or SumAsync to get the value. This doubles the number of database queries unnecessarily.

**Solution:** Replace all synchronous LINQ methods with their async counterparts. Eliminate the redundant AnyAsync checks before CountAsync or SumAsync, since both return zero when no records match, making the existence check unnecessary.

---

### 3.6 Loading Entire Tables Into Memory Then Filtering in C#

**Problem:** Multiple service methods load entire database tables into memory and then filter the data using C# LINQ-to-Objects instead of applying filters in the SQL query. The DashboardController loads all order records and then iterates over months in a C# loop. The NotoficationService loads all notifications matching a user and then processes them in memory. The ServiceAmenityAccess loads the entire amenities table inside a foreach loop.

**Solution:** Move all filtering, grouping, and aggregation into the database query using Entity Framework LINQ expressions. Apply WHERE conditions, GROUP BY clauses, and aggregate functions (COUNT, SUM) at the database level so that only the required result set is transferred to the application.

---

### 3.7 Search Implementation Uses Inefficient LIKE Patterns

**Problem:** The search functionality in CategoryServiceAccess splits the search term into words and builds a LINQ expression that translates to SQL LIKE '%word%' for each word, combined with LOWER() function calls. This combination guarantees a full table scan on every search, as SQL Server cannot use any index for middle-of-string pattern matching with a leading wildcard.

**Solution:** For immediate improvement, replace Contains (which generates LIKE '%x%') with StartsWith (which generates LIKE 'x%') where the business logic allows, as prefix matching can use indexes. For a proper long-term solution, implement SQL Server Full-Text Search, which is purpose-built for text searching and supports word-based matching with proper indexing.

---

## 4. Dashboard Performance

### 4.1 Dashboard Query Breakdown

**Problem:** A single dashboard page load results in the following database activity: 5 aggregate queries for top-level statistics (total customers, orders, sales), a foreach loop with 2 queries per feature category (16 queries for 8 categories), 8 more aggregate queries for weekly and monthly statistics (each with a redundant AnyAsync check, so effectively 16 queries), and 4 AJAX chart endpoints that each load the entire orders table into memory. The total is approximately 22+ sequential queries for the main page, plus 4 full table loads for the charts.

**Solution:** Combine all statistics into 2 to 3 optimized queries using GROUP BY. Replace the per-category loop with a single query that groups by FeatureCategoryId. Run independent queries in parallel using Task.WhenAll. For chart data, use SQL GROUP BY MONTH to return monthly aggregates directly from the database. Cache the entire dashboard result for 5 minutes, as these statistics do not need to be real-time.

---

## 5. Caching Strategy

### 5.1 Complete Absence of Caching

**Problem:** The entire system has zero caching of any kind. There is no use of MemoryCache, no OutputCache attributes, no HttpRuntime.Cache, no third-party caching libraries, and no Cache-Control headers on responses. Every single request hits the database directly, including data that changes rarely or never.

**Solution:** Introduce an in-memory caching layer using System.Runtime.Caching.MemoryCache. Create a centralized cache helper class that provides get-or-set semantics with configurable expiration times. Implement cache invalidation methods that are called when data is modified through CRUD operations.

---

### 5.2 Layout Settings Queried 3 Times Per Page

**Problem:** On every admin page load, the LayoutSetting table is queried 3 times with identical queries — once from _Layout.cshtml, once from _Header.cshtml, and once from _Scripts.cshtml. Each call creates a new LayoutSettingAccess instance which creates a new UnitOfWork which creates a new DbContext. This data (theme configuration like logo, header color, sidebar style) changes only when an admin explicitly toggles a theme.

**Solution:** Cache the layout settings in MemoryCache with a key like "LayoutSetting_Default" and a duration of 30 to 60 minutes. Invalidate the cache when the layout is updated through the admin panel. Pass the cached layout data through ViewBag from a base controller action filter so that all three partial views use the same cached value without any database query.

---

### 5.3 Reference Data That Should Be Cached

**Problem:** Static and slow-changing reference data is re-queried from the database on every request. FeatureCategories are queried in 19+ admin controller locations. Countries are queried in 12 locations across admin and API. Cities are queried in 14 locations. Brands, Categories (with expensive tree assembly), ProductTypes, PaymentMethods, FAQs, and Airports are all queried fresh every time.

**Solution:** Implement caching for each type of reference data with appropriate durations based on how frequently the data changes. Countries, ProductTypes, PaymentMethods, and Airports essentially never change and can be cached for 24 hours. FeatureCategories, Cities, and Roles change very rarely and can be cached for 60 minutes. Brands, category trees, FAQs, and CMS content can be cached for 15 to 30 minutes. Each cache entry should be invalidated when the corresponding data is modified through admin CRUD operations.

---

### 5.4 Dashboard Statistics Should Be Cached

**Problem:** The dashboard statistics involve 22+ database queries that produce aggregate numbers. These numbers do not need to be updated in real-time — a 5-minute-old count of total orders is perfectly acceptable for a dashboard view.

**Solution:** Cache the entire dashboard view model with a key like "Dashboard_Stats" and a 5 to 10 minute expiration. Cache each chart dataset separately with similar durations. This single change eliminates 22+ queries and 4 full table loads on every dashboard visit after the first.

---

### 5.5 API Endpoints Serving Static Data Need HTTP Caching

**Problem:** API endpoints for countries, cities, feature categories, brands, product types, payment methods, airports, and FAQs return data that changes infrequently, but provide no HTTP caching headers. Every mobile app or frontend client request generates a fresh database query.

**Solution:** For Web API controllers, implement a custom action filter that adds appropriate Cache-Control headers to responses for static data endpoints. Use cache durations of 1 hour for rarely-changing data (countries, product types) and 15 to 30 minutes for occasionally-changing data (brands, FAQs). Vary the cache by relevant parameters such as language and feature category ID.

---

### 5.6 Admin Notification Count Should Be Cached

**Problem:** The admin header polls for new notification counts every 5 seconds using AJAX. This generates a database query every 5 seconds per open admin browser tab. With 10 admin users each having one open tab, this creates 120 database queries per minute (7,200 per hour) just for notification counts.

**Solution:** Cache the notification count per user for 30 to 60 seconds. Increase the polling interval from 5 seconds to 30 or 60 seconds, or replace polling with a more efficient mechanism such as SignalR for real-time push notifications.

---

## 6. Image and File Processing

### 6.1 Images Saved Without Compression

**Problem:** The MediaHelper image upload function saves images at default quality (100%) without any compression parameters. The ProductImages table contains 4,235 images. A single product image could be 2 to 5 MB in size. A product listing page showing 20 products could require 40 to 100 MB of image data to be transferred.

**Solution:** Apply JPEG compression with a quality level of 75 to 80 percent, which reduces file size by 60 to 70 percent with no perceptible quality loss for web display. Use the JPEG encoder with an EncoderParameter for quality. Resize images that exceed a maximum dimension (such as 1200 pixels wide) before saving. Consider converting to WebP format for modern browsers, which provides even better compression ratios.

---

### 6.2 No Thumbnail Generation

**Problem:** The upload process saves images at their original full resolution. There is no automatic generation of thumbnails for list views. When a page needs to display a 150-pixel-wide thumbnail, it loads the full 4000x3000 pixel original image.

**Solution:** Generate thumbnails at the time of upload in two or three standard sizes (for example, 150x150 for lists, 300x300 for cards, and 800x600 for detail views). Serve the appropriate size based on the display context. Store thumbnails alongside the original with a naming convention that identifies the size.

---

### 6.3 No Static File Caching or CDN

**Problem:** Images are served directly by IIS from the application directory with no Cache-Control headers. There is no Content Delivery Network configured. There is no compression enabled for static content. Every request for an image goes to the application server and reads the file from disk.

**Solution:** Add static content caching headers in Web.config to instruct browsers to cache images for 30 days. Enable IIS static and dynamic compression. For production, consider using a CDN to serve static files, which reduces load on the application server and provides faster delivery to geographically distributed users.

---

## 7. Frontend and Static Assets

### 7.1 Excessive Number of CSS and JS Files Per Page

**Problem:** Every admin page loads 16 CSS files and 28 JavaScript files individually, resulting in over 44 separate HTTP requests before any page content is loaded. Many of these libraries are only needed on specific pages — for example, TinyMCE (a rich text editor, approximately 500 KB) is loaded on every page even though it is only used on create and edit forms. D3.js (approximately 280 KB), Chartist, and C3.js are loaded on every page but only used on the dashboard. FullCalendar CSS is loaded on every page but there are no calendar views in the application.

**Solution:** Use ASP.NET's bundling and minification feature (BundleConfig) to combine all commonly used CSS files into one or two bundles, and all commonly used JS files into a small number of bundles. Libraries that are only needed on specific pages (TinyMCE, D3, Chartist, FullCalendar, jVectorMap) should be loaded only on the pages that use them, not globally. Define section blocks in the layout for page-specific scripts and styles.

---

### 7.2 Three Copies of jQuery Loaded Per Page

**Problem:** Every admin page loads jQuery three times from three different locations — once from the Head partial view, once from the Scripts partial view, and once from the Header partial view. Additionally, 14 individual view files load a fourth copy of jQuery version 1.7.1 (an ancient version from 2011) over insecure HTTP. This results in approximately 300 KB of wasted bandwidth per page load and can cause JavaScript conflicts between different jQuery versions.

**Solution:** Load jQuery exactly once, in the Head partial view, and remove all other jQuery script tags from the Scripts partial, the Header partial, and individual views. Remove all references to the outdated jQuery 1.7.1 over HTTP. Use the single jQuery version provided by the BundleConfig.

---

### 7.3 No Script Defer or Async Attributes

**Problem:** All script tags load synchronously without the async or defer attributes. jQuery is loaded in the head section, which blocks page rendering until the script is downloaded and executed. This means the browser cannot begin rendering the page until all scripts in the head are fully loaded.

**Solution:** Move all script tags to the bottom of the body, or add the defer attribute to script tags that do not need to execute immediately. The jQuery in the head can use the defer attribute to prevent render blocking while maintaining execution order.

---

### 7.4 External CDN Dependencies Add Latency

**Problem:** Several resources are loaded from external CDNs (jsDelivr, CloudFlare, unpkg) which adds network latency for DNS resolution and connection setup to multiple external domains. If any CDN is down or slow, the page load is delayed. Some dashboard-specific views even load duplicate copies of Chartist from a CDN in addition to the local copy already loaded globally.

**Solution:** Host all third-party libraries locally and bundle them. This eliminates external DNS lookups, reduces connection overhead, and ensures the application works even when external CDNs are unreachable. Remove the duplicate Chartist CDN loads from the dashboard view since the library is already loaded globally.

---

### 7.5 No HTTP Compression Configured

**Problem:** Web.config has no urlCompression or httpCompression settings. This means all HTML, CSS, JavaScript, and JSON responses are sent to the browser at their full uncompressed size, typically 3 to 5 times larger than compressed.

**Solution:** Enable both static and dynamic compression in Web.config under the system.webServer section. Add urlCompression with doStaticCompression and doDynamicCompression set to true. Also add staticContent caching with a clientCache directive using a max-age of 30 days.

---

## 8. Pagination

### 8.1 29 Out of 30 Listing Endpoints Load All Data

**Problem:** Across the entire admin area, 29 out of 30 listing endpoints load all records from the database without any server-side pagination. Only the MemberController has partial pagination support. Every other controller — Products, Services, Brands, Categories, Orders, Users, Notifications, Offers, Packages, Countries, Cities, Airports, FAQs, and more — calls GetAll or equivalent methods that return the complete data set. The PaginatedList class that exists in the codebase has a critical flaw: its CreateAsync method calls ToListAsync on the full queryable without applying Skip and Take, meaning it loads all records into memory even when pagination metadata is calculated.

**Solution:** Implement proper server-side pagination in the repository and service layers. Add Skip and Take to all list queries based on page number and page size parameters. Fix the PaginatedList.CreateAsync method to apply Skip and Take to the IQueryable before materializing it. Each listing endpoint should accept page number and page size parameters and return only the requested page of data.

---

### 8.2 All DataTables Use Client-Side Processing

**Problem:** Approximately 35 DataTable instances across the admin views all use client-side processing mode. The server renders all rows into the HTML DOM, and DataTables handles sorting, searching, and pagination entirely in the browser. This means the server must generate HTML for all records, the browser must parse and render all rows, and the browser must hold all data in memory. For tables with thousands of records, this causes significant delays in page rendering and high browser memory consumption.

**Solution:** Switch DataTables to server-side processing mode by setting serverSide to true and configuring an AJAX source URL. Create server-side endpoints that accept DataTables parameters (start, length, search, order) and return only the requested page of data in the expected JSON format. This approach ensures that only 10 to 25 rows are transferred and rendered at a time, regardless of the total dataset size.

---

## 9. API Layer Issues

### 9.1 N+1 Query in Search Endpoint

**Problem:** The SearchAllProductAndService endpoint first loads all feature categories, then iterates over each category and fires two additional queries (one for products, one for services) per category. For 10 categories, this results in 21 database queries per search request.

**Solution:** Restructure the search to execute product and service queries with WHERE IN clauses based on the list of category IDs, rather than querying per category in a loop. Alternatively, use database joins to fetch all search results in a single query.

---

### 9.2 Useless Try-Catch-Throw Patterns

**Problem:** 22 or more methods across multiple controllers wrap their logic in try-catch blocks that catch Exception and immediately re-throw it with no additional handling, logging, or transformation. This adds code noise and stack depth without providing any value.

**Solution:** Remove all try-catch blocks that only contain throw with no additional logic. If global exception logging is needed, implement it through a global exception filter or middleware rather than repeating the same empty catch block in every method.

---

### 9.3 Inconsistent Response Format

**Problem:** Different controllers use different response envelope formats. Most controllers use the BaseController's SuccessMessage method which wraps the result, but NotificationController uses a different property name for the data. UserReportController uses a completely different object structure. The BaseController's ErrorMessage method always returns HTTP status 200 with an internal error status code, which violates REST conventions and makes it difficult for clients to detect errors from HTTP status codes alone.

**Solution:** Standardize the API response envelope across all controllers. Use a single response wrapper class with consistent property names. Return appropriate HTTP status codes (400 for bad requests, 401 for unauthorized, 404 for not found, 500 for server errors) instead of always returning 200 with an embedded error code.

---

### 9.4 Mixing Static and Instance Method Calls

**Problem:** The NotificationController inconsistently calls some notification service methods as static methods and others on new instances. This suggests the service class has an inconsistent design where some methods require instance state (a UnitOfWork) and others use their own separately created contexts.

**Solution:** Choose one consistent approach — either all methods are instance methods using a shared UnitOfWork, or distinct service classes are created for different concerns. Avoid mixing static and instance patterns in the same class.

---

### 9.5 Wrong Variable Returned in Third-Party Login

**Problem:** In the MemberController's third-party login handler, when a new member is registered via a third-party provider, the code sets ThirdPartyLogin to true on the newly registered member object but then returns a different variable (loginMember) which is an empty Member object with no data. This bug silently discards the registration result and returns empty data to the client.

**Solution:** Return the correct variable (registeredMember instead of loginMember) in the success response after third-party registration.

---

## 10. Architecture and Clean Code Violations

### 10.1 Repository Pattern Defeats Its Own Purpose

**Problem:** The GenericRepository's Add, Edit, and Remove methods each call SaveChanges immediately after the operation. This defeats the entire purpose of the Unit of Work pattern, which is to batch multiple operations into a single database transaction. When each operation saves independently, there is no transactional consistency across multiple related changes. The MultipleRemove method is especially problematic as it calls SaveChanges inside a foreach loop, resulting in N database roundtrips instead of one.

**Solution:** Remove SaveChanges calls from all individual repository methods. Add SaveChanges and SaveChangesAsync methods to the IUnitOfWork interface. Service methods should call SaveChanges once at the end of a logical transaction, after all related Add, Edit, and Remove operations have been queued.

---

### 10.2 IUnitOfWork Missing Critical Methods

**Problem:** The IUnitOfWork interface does not extend IDisposable and does not declare SaveChanges or SaveChangesAsync methods. This means callers using the interface cannot dispose the underlying DbContext, and there is no way to commit a batch of changes through the interface. The actual UnitOfWork class has a Dispose method, but since IUnitOfWork does not declare it, code that references the interface (which is how it should be used) cannot access it.

**Solution:** Extend IUnitOfWork to inherit from IDisposable. Add SaveChanges and SaveChangesAsync method declarations to the interface. Update all service classes to call SaveChangesAsync at the end of their operations and to either use using statements or delegate disposal to the dependency injection container.

---

### 10.3 Public Fields on Repository and UnitOfWork

**Problem:** GenericRepository exposes its BoulevardDbContext and DbSet as public fields rather than private or protected fields. UnitOfWork also exposes its DbContext as a public field. This breaks encapsulation and allows any code to bypass the repository pattern by accessing the context directly.

**Solution:** Change all public fields in GenericRepository and UnitOfWork to private (or protected where subclass access is needed). Add readonly modifiers where the field should not be reassigned after construction.

---

### 10.4 No Dependency Injection Container

**Problem:** The entire application has no IoC (Inversion of Control) container. Every service class is instantiated using the new keyword in controller constructors. Service classes that need other service classes also create them with new. This makes unit testing impossible because dependencies cannot be mocked or replaced. It also means each class manages its own lifecycle, resulting in too many DbContext instances and no shared per-request scope.

**Solution:** Introduce a dependency injection container (such as Autofac, Unity, or Simple Injector) and register all service interfaces with their implementations. Register BoulevardDbContext with per-request lifetime scope. Update all controllers and service classes to receive their dependencies through constructor injection. This enables unit testing, ensures a single DbContext per request, and eliminates the cascading new UnitOfWork problem.

---

### 10.5 Cascading Service Instantiation Creates Excessive DbContexts

**Problem:** When a service method needs data from another domain, it creates a new instance of that service class using the new keyword. Each new service creates its own UnitOfWork, which creates its own DbContext. A single request can cascade into 10 to 20 or more DbContext instances. For example, processing a member's orders involves OrderRequestServiceAccess creating ProductServiceAccess (new UnitOfWork), which creates OfferServiceAccess (another new UnitOfWork), which creates MemberServiceAccess (yet another new UnitOfWork). Each of these operates in isolation with no shared transaction scope.

**Solution:** With a dependency injection container in place, all service classes receive the same DbContext instance per request through their shared UnitOfWork. This eliminates the cascading creation issue entirely. Without DI as an immediate fix, refactor service methods to accept an existing UnitOfWork as a parameter rather than creating their own.

---

### 10.6 Service Layer Contains Presentation Logic

**Problem:** Multiple service classes in the WebAPI layer contain URL construction logic using HttpContext.Current.Request.Url to build full image URLs. This is presentation or API formatting logic that does not belong in the data access layer. Similarly, language switching logic is embedded directly in data access methods, mixing localization concerns with data retrieval.

**Solution:** Move URL construction to a separate mapper or DTO factory that runs in the controller or API layer. Move language switching to a separate localization service or handle it through resource files. The service layer should return raw data, and the presentation layer should format it.

---

### 10.7 Massive Service Files (God Classes)

**Problem:** Several service files have grown extremely large: ServiceAccess.cs has 1,804 lines, CategoryServiceAccess.cs has 834 lines, ProductAccess.cs has 807 lines, and OfferServiceAccess.cs has 790 lines. These files contain repeated and overlapping logic, making maintenance, optimization, and testing very difficult.

**Solution:** Break down large service classes into smaller, focused classes based on specific responsibilities. For example, ProductAccess could be split into ProductQueryService (for reads), ProductCommandService (for writes), and ProductStockService (for stock-related operations). Each should be small enough to understand and test in isolation.

---

### 10.8 Dead and Test Code in Production

**Problem:** The ServiceAccess class contains methods (GetAllMotorServices, GetAllSalonServices) with hardcoded fake test data — manually constructed service objects with hardcoded IDs and names. These methods are marked as async but perform no asynchronous work. FakeController and DemoController exist in the production controller directory.

**Solution:** Remove all test and mock data from production service classes. Remove FakeController and DemoController. If test doubles are needed, create them in a separate test project.

---

### 10.9 Hardcoded Magic Numbers

**Problem:** The codebase contains hardcoded IDs and values that are meaningful but not named or explained. The value CreateBy = 1 is set in every insert and update operation, meaning the audit trail always shows user 1 regardless of who is actually logged in. Feature category IDs 9, 11, and 13 are used in conditional logic with no explanation of what these categories represent. The phone number prefix "+971" is hardcoded in member registration. Order ID prefixes and timeout durations are hardcoded in service access classes.

**Solution:** Replace all hardcoded IDs with named constants that clearly describe their purpose, or better yet, replace them with database-driven configuration or feature flags. For CreateBy, use the actual authenticated user's ID from the current HTTP context. For country-specific logic like phone prefixes, store the configuration in the database and make it configurable per deployment.

---

### 10.10 Exception Swallowing

**Problem:** Every service method follows a pattern of catching all exceptions, logging them, and returning null. The callers of these methods often do not check for null, resulting in NullReferenceException further up the call stack. The actual error is hidden, and the null propagates silently until it causes an unrelated crash somewhere else.

**Solution:** For expected failure cases, use result types or specific exceptions that carry meaning. For unexpected errors, allow the exception to propagate to the global exception handler. Where null is a valid return value (such as "entity not found"), distinguish it clearly from error cases through the method's return type or documentation.

---

## 11. Data Model and Structure Issues

### 11.1 Status Field Is a Free-Form String with Inconsistent Values

**Problem:** The Status field on the BaseEntity class is a string with a maximum length of 10 characters. Different parts of the codebase compare against different values — "Active", "Delete", "Deleted", "Finished", "Pending", "Success". Some code uses .ToLower() == "Active" which will never match because the lowercase version of "Active" is "active". There is no enum, no constants, and any typo in a status value becomes a silent bug that filters out records.

**Solution:** Define an enum for entity status values with all valid states. While the database column can remain a string for backward compatibility, map it through a property that parses the string into the enum. Create a migration to normalize existing inconsistent values (converting "Delete" to "Deleted" wherever used). Replace all magic string comparisons with enum comparisons throughout the codebase.

---

### 11.2 Entity Models Used as ViewModels (15+ NotMapped Properties)

**Problem:** The Product model class serves as both a database entity and a view model. It contains more than 15 NotMapped properties for presentation purposes, including IsUpsellProduct, IsCrosssellProduct, ProductList, ProductImages, ProductPrices, CartId, CartQuantity, IsFavourite, CategoryName, BrandName, PageIndex, PageSize, and TotalProductCount. This violates the Single Responsibility Principle and makes the entity carry data it has no business carrying.

**Solution:** Create separate ViewModel and DTO classes for each presentation context. The Product entity should contain only database-mapped properties. ProductListItemDto would contain the display properties needed for listing. ProductDetailDto would contain the full detail view properties. Map between entities and DTOs using explicit mapping methods or a library like AutoMapper.

---

### 11.3 MemberId Type Mismatch (Long vs Int)

**Problem:** The Member model defines MemberId as a long (64-bit integer), but throughout the service and controller code, it is consistently cast to int using Convert.ToInt32. This creates a truncation risk when member IDs exceed the int maximum value of approximately 2.1 billion.

**Solution:** Either change the model to use int if the ID space will never exceed 2.1 billion, or update all code that references MemberId to use long consistently without casting.

---

### 11.4 Missing Navigation Properties

**Problem:** Several important entity relationships are marked as NotMapped or are entirely absent, forcing manual queries for related data. Product to ProductPrice, Product to ProductImage, and Product to ProductCategory all lack real EF navigation properties. OrderRequestProduct has no navigation collection for order details. Category has no self-referencing navigation for child categories. Without navigation properties, Entity Framework cannot use Include or eager loading, and all related data must be fetched with separate manual queries.

**Solution:** Add proper navigation properties with the appropriate foreign key attributes. Configure the relationships in the DbContext using Fluent API if necessary. This enables the use of Include for eager loading and eliminates many of the N+1 query patterns.

---

### 11.5 Misspelled Properties in Models

**Problem:** The Category model has a property "IsTrenbding" which is a misspelling of "IsTrending". The ServiceType model has a property "PersoneQuantity" which is a misspelling of "PersonQuantity". These misspellings propagate to the database column names and all code that references them.

**Solution:** Create a database migration to rename the columns to their correct spellings. Update all references in the codebase. Since this is a breaking change for the database, ensure the migration handles existing data correctly.

---

### 11.6 Product Price Silently Clamps Negative Values

**Problem:** The Product model has a private setter on ProductPrice that uses Math.Max(0, value), which silently converts negative prices to zero. If a pricing error or calculation bug produces a negative price, it is silently set to zero instead of raising an error. This could result in products being listed for free without anyone noticing.

**Solution:** Throw a validation exception when a negative price is supplied rather than silently clamping it. Price validation is a business rule that should be explicit and visible, not hidden in a property setter.

---

### 11.7 OrderRequestService Does Not Inherit BaseEntity

**Problem:** OrderRequestService is the only entity in the system that does not inherit from BaseEntity, so it lacks the standard audit fields (CreateBy, CreateDate, Status, ModifiedBy, ModifiedDate). This makes it impossible to track who created or modified service orders, and the entity cannot be soft-deleted using the standard Status pattern.

**Solution:** Refactor OrderRequestService to inherit from BaseEntity and add a database migration to add the missing columns with appropriate default values for existing records.

---

### 11.8 GenericRepository Get Method Returns Null When No OrderBy

**Problem:** The GenericRepository's Get method initializes a local result variable as null. If the optional orderBy parameter is not provided, the result variable is never assigned the filtered query, and the method returns null. This silently discards any filters and includes that were applied to the query.

**Solution:** Fix the Get method to return the filtered query directly rather than assigning it through the orderBy parameter. The orderBy should be applied after the base query is built, and the base query should always be returned regardless of whether ordering is applied.

---

## 12. Template Compilation and Server Configuration

### 12.1 Debug Mode Enabled in Production Configuration

**Problem:** The Web.config file has compilation debug set to true. This setting has multiple performance impacts for ASP.NET: Razor views are compiled one at a time instead of being batch compiled (all views in a folder compiled together). Bundle minification is disabled, so CSS and JS files are served at full size. Additional debug metadata is generated for every request. Compiled view assemblies are not cached as aggressively by the runtime. The optimizeCompilations setting is not configured, meaning any top-level file change can trigger recompilation of the entire application.

**Solution:** Set compilation debug to false in the production Web.config (Web.Release.config already handles this transform, but the base configuration should also be secure). Add optimizeCompilations set to true, which tells ASP.NET to only recompile views that have actually changed rather than triggering a full recompilation cascade. Add batch set to true with an appropriate batchTimeout to enable batch compilation of all views in a directory into a single assembly. For maximum performance, enable precompilation by adding MvcBuildViews set to true in the project file, which compiles all Razor views at build time and eliminates first-request compilation delays entirely.

---

### 12.2 ASP.NET View Compilation (Equivalent of PHP OPcache)

**Problem and Context:** The user asked about the equivalent of PHP's OPcache template caching in ASP.NET. ASP.NET MVC's Razor engine has a built-in compilation system that works as follows: When a Razor view (.cshtml file) is first requested, the engine generates C# code, compiles it using the Roslyn compiler into a .NET assembly (.dll), and caches the resulting DLL in the "Temporary ASP.NET Files" folder. Subsequent requests to the same view use the cached DLL directly, with no compilation overhead. Recompilation occurs only when the .cshtml file is modified (checked by file timestamp). In debug mode, views are compiled individually, which increases the number of compiled assemblies and reduces caching efficiency. In release mode with batch compilation enabled, all views in a directory are compiled into a single DLL, which is more memory-efficient and faster.

**Solution:** The immediate action is to set debug to false and add optimizeCompilations to true. For maximum production performance: enable batch compilation, increase the numRecompilesBeforeAppRestart threshold from the default 15 to at least 50 (to prevent excessive application domain recycling during active development or after deployments), and consider precompiling all views at build time by adding MvcBuildViews to the project file. Precompilation eliminates all first-request compilation penalties and catches view compilation errors at build time rather than at runtime.

---

## Summary of Issue Counts by Severity

| Severity | Count | Primary Categories |
|----------|-------|--------------------|
| Catastrophic | 1 | Complete absence of authentication across the entire system |
| Critical | 15 | IDOR, Remote Code Execution via file upload, OTP bypass, exposed credentials, SHA-256 without salt, stored XSS, N+1 queries (12 instances), missing indexes, no caching, no pagination |
| High | 20 | GET-based destructive operations, missing CSRF, connection string injection, layout settings triple-query, AJAX polling, jQuery triplication, DbContext leaks, multiple DbContexts per request, broken UnitOfWork, no DI |
| Medium | 16 | Status string inconsistency, dead code, magic numbers, response format inconsistency, model misspellings, ToLower preventing index usage, synchronous database calls, excessive ViewBag usage |
| Low | 8 | Public fields, naming conventions, debug mode, Swagger exposure, cookie configuration, test controllers |

---

## Recommended Fix Order

**Phase 0 — Security Emergency (Before Everything Else):** Register global authorization filters for both MVC and Web API. Add AllowAnonymous only on login, registration, and truly public endpoints. Implement file upload validation with a whitelist of allowed extensions. Fix the OTP bypass by enforcing server-side OTP verification state. Move all hardcoded credentials to configuration. Implement proper CSRF protection across admin forms.

**Phase 1 — Database and Caching Foundation:** Create all missing database indexes. Introduce the MemoryCache helper and cache layout settings, reference data (countries, cities, categories, brands, feature categories), and dashboard statistics. Set debug to false with optimizeCompilations enabled. Enable HTTP compression in Web.config.

**Phase 2 — Dashboard and Query Optimization:** Consolidate dashboard queries using GROUP BY and Task.WhenAll. Fix all 12 N+1 query patterns by pre-loading related data in batches. Convert chart data endpoints to use SQL aggregation instead of in-memory processing.

**Phase 3 — Frontend and Pagination:** Bundle and minify CSS and JS files. Remove duplicate jQuery loads. Load page-specific libraries only where needed. Implement server-side DataTables processing. Add server-side pagination to all listing endpoints. Fix the PaginatedList class. Apply image compression and thumbnail generation.

**Phase 4 — Architecture Refactoring:** Introduce a dependency injection container. Register DbContext with per-request lifetime. Fix IUnitOfWork to include IDisposable and SaveChanges. Remove SaveChanges from individual repository methods. Separate entity models from view models. Eliminate all standalone DbContext creations. Convert Status strings to enums. Remove dead code and test controllers.

---

*This report was compiled after thorough analysis of 4 detailed technical audit reports covering the entire Boulevard system — database layer, service layer, repository layer, all controllers, all views, all models, all helper classes, and all configuration files. A total of 75 database tables, 70+ service files, 37 controllers, 276 Razor views, and all supporting infrastructure files were examined.*
