# Boulevard System — Comprehensive Unified Audit Report (Admin Panel + Mobile App + Database)

**Date:** March 13, 2026
**Scope:** Full-system cross-platform audit — ASP.NET MVC 5 Admin Panel, Web API 2 Backend, Flutter Mobile Application (Android + iOS), SQL Server Database, all configuration files, all platform-specific build files, and all infrastructure settings
**Technology Stack:**
- **Backend:** ASP.NET MVC 5 + Web API 2 (.NET Framework 4.7.2), Entity Framework 6 (Code First), SQL Server 2022 Express
- **Mobile App:** Flutter (Dart SDK >=3.3.3 <4.0.0), GetX State Management, Firebase Core/Messaging/Auth
- **Android:** compileSdk 36, minSdkVersion 26, targetSdkVersion 36, Kotlin 2.1.0
- **iOS:** Minimum iOS 15.0, Sign in with Apple, UAE Pass integration
- **Payment Gateway:** MyFatoorah
- **Maps:** Google Maps (Android + iOS)
- **Social Auth:** Google Sign-In, Facebook Login, Apple Sign-In, UAE Pass

**Total Issues Found:** 147+, classified across 18 categories
**Audited Files:** 75 database tables, 70+ server-side service files, 37 controllers, 276 Razor views, 55+ Dart files, 12 configuration files, 4 platform build files, all entitlement and manifest files

---

## Table of Contents

1. [Security Vulnerabilities — Server Side (ASP.NET)](#1-security-vulnerabilities--server-side-aspnet)
2. [Security Vulnerabilities — Mobile App (Flutter)](#2-security-vulnerabilities--mobile-app-flutter)
3. [Security Vulnerabilities — Platform Configuration (Android + iOS)](#3-security-vulnerabilities--platform-configuration-android--ios)
4. [Database Performance Issues](#4-database-performance-issues)
5. [Entity Framework and Query Patterns](#5-entity-framework-and-query-patterns)
6. [Dashboard Performance](#6-dashboard-performance)
7. [Caching Strategy (Complete Absence — Server and Client)](#7-caching-strategy-complete-absence--server-and-client)
8. [Image and File Processing](#8-image-and-file-processing)
9. [Frontend and Static Assets (Admin Panel)](#9-frontend-and-static-assets-admin-panel)
10. [Pagination (Missing Across the Entire System)](#10-pagination-missing-across-the-entire-system)
11. [API Layer Issues (Server Side)](#11-api-layer-issues-server-side)
12. [API Layer Issues (Mobile App Client Side)](#12-api-layer-issues-mobile-app-client-side)
13. [Architecture and Clean Code Violations (Server Side)](#13-architecture-and-clean-code-violations-server-side)
14. [Architecture and Clean Code Violations (Mobile App)](#14-architecture-and-clean-code-violations-mobile-app)
15. [Data Model and Structure Issues](#15-data-model-and-structure-issues)
16. [Mobile App Performance Issues](#16-mobile-app-performance-issues)
17. [Mobile App UI/UX and Localization Issues](#17-mobile-app-uiux-and-localization-issues)
18. [Template Compilation and Server Configuration](#18-template-compilation-and-server-configuration)

---

## 1. Security Vulnerabilities — Server Side (ASP.NET)

### 1.1 Complete Absence of Authentication on All API and Admin Controllers

**Problem:** There is no Authorize attribute on any controller or action method across the entire system. This includes all 32 Web API controllers and all Admin area controllers. There is no global authorization filter registered in FilterConfig or WebApiConfig. The FormsAuthentication cookie is created upon admin login, but no controller actually checks for it.

Every sensitive endpoint is publicly accessible to anonymous users. This means anyone who knows the URL structure can delete members, view order histories, modify payment statuses, upload files, access the full admin dashboard, create or modify products, and perform any administrative operation without any form of authentication.

**Affected Files:** All controllers in the Controllers folder (MemberController, ProductController, ServiceController, OrderRequestController, NotificationController, UploadController, and 26 others), all Admin area controllers (DashboardController, UserController, ProductController, CategoryController, BrandController, and all others).

**Solution:** Register a global authorization filter in both FilterConfig for MVC controllers and WebApiConfig for Web API controllers. This will require all requests to be authenticated by default. Then, apply the AllowAnonymous attribute only on endpoints that must remain public, such as the login page, the registration endpoint, and public-facing API endpoints like product listing or country lookup. For the admin area specifically, ensure FormsAuthentication is enforced, and for the Web API, implement JWT or token-based authentication so that mobile app users must present valid credentials.

---

### 1.2 Insecure Direct Object References (IDOR)

**Problem:** Every endpoint that accepts a member ID or user ID as a parameter trusts the client-supplied value without verifying that the currently authenticated user is the owner of that ID. This means any user can pass a different member ID to view, modify, or delete another user's data. Affected operations include viewing member details, deleting members, viewing member addresses, viewing cart contents, viewing order histories, reading notifications, and reading customer enquiries.

**Solution:** The member or user ID should never come from the request body or query string for operations on the current user's data. Instead, extract the authenticated user's ID from the authentication token or session. Before performing any operation, verify that the authenticated user owns the resource being accessed. For admin operations, verify that the user has an admin role before allowing access to other users' data.

---

### 1.3 Unrestricted File Upload Leading to Remote Code Execution

**Problem:** The file upload endpoint in UploadController accepts files of any type without validation. The upload handler in MediaHelper preserves the original file extension provided by the client. Files are saved directly inside the web root under the Content/Upload directory. There is no authentication on the upload endpoint, no file type whitelist, no content-type verification, no file size limits, and no magic byte validation. An attacker could upload a web shell file with an ASPX extension, and IIS would execute it, giving the attacker full control of the server.

**Solution:** Implement a strict whitelist of allowed file extensions, permitting only safe formats such as JPG, JPEG, PNG, GIF, PDF, DOC, DOCX, and XLSX. Validate both the file extension and the actual content-type header. Validate magic bytes (file signature) to ensure the file contents match the declared type. Save uploaded files outside the web root directory or in a location configured to not execute scripts. Add a file size limit both in Web.config and in the upload handler. Generate unique filenames using Guid to prevent filename-based attacks. Require authentication before allowing any file upload.

---

### 1.4 Hardcoded Credentials in Source Code

**Problem:** SMTP email credentials are written directly in the EmailService helper class in four separate locations. The email password "partners@123" for the account "partners@boulevardsuperapp.com" is visible in plain text. The Web.config file contains commented-out connection strings that expose the production database server IP address (109.203.124.192), the database username (BoulevardDb-user), and the database password (o50i!32qK). The CourierController contains a hardcoded API key comparison where the key "Jeebly123" is written directly in the source code.

**Solution:** Move all credentials to the appSettings section of Web.config or to environment variables. In production, use a secrets management solution such as Azure Key Vault or encrypted configuration sections. Remove all commented-out connection strings that contain credentials from Web.config. For the courier API key, store it in configuration and use a constant-time comparison function to prevent timing attacks.

---

### 1.5 OTP Bypass in Password Reset Flow

**Problem:** The password reset flow has a critical logic flaw. An attacker can skip the OTP verification step entirely by sending the email address and a new password with the OTP value set to zero. The system interprets this as a valid password reset request without requiring OTP validation. Furthermore, the OTP value is returned in the API response when the Member entity is serialized, meaning an attacker can read the OTP directly from the response without needing access to the user's phone or email.

**Solution:** Implement server-side state management for the password reset flow. After the OTP is verified, set a time-limited flag on the server (such as an OTPVerified boolean with an expiration timestamp) that must be checked before allowing the password change. Never return the OTP value in API responses. Exclude sensitive fields like OTPNumber, Password hash, and SecurityToken from all API response serialization.

---

### 1.6 Weak Password Hashing (SHA-256 Without Salt)

**Problem:** The system hashes passwords using SHA-256 without any salt, as seen in Helper/HashConfig.cs. This means two users with the same password will have identical hash values, making the system vulnerable to rainbow table attacks and precomputed hash lookups. SHA-256 is a general-purpose hash function that is extremely fast, allowing attackers to try billions of password combinations per second with modern hardware. The comparison in Service/UserAccess.cs also uses direct string comparison which is vulnerable to timing attacks.

**Solution:** Replace SHA-256 with a purpose-built password hashing algorithm such as bcrypt, scrypt, or PBKDF2. Each user's password must be hashed with a unique, randomly generated salt. The work factor should be set high enough that hashing takes at least 100 milliseconds. Use constant-time comparison for hash verification. When migrating, implement a strategy where existing passwords are re-hashed on next successful login.

---

### 1.7 Stored Cross-Site Scripting (XSS)

**Problem:** The ValidateInput attribute is set to false on 14 admin controller actions, which disables ASP.NET's built-in request validation. Combined with the use of Html.Raw in views to render user-provided content (such as product descriptions, membership benefits, user report comments, and delivery information), this creates stored XSS vulnerabilities. XSS is also present in inline JavaScript within views — Airport/Index.cshtml, Category/Index.cshtml, City/Index.cshtml, and Country/Index.cshtml all embed entity keys directly into onclick JavaScript handlers without encoding, allowing injection through crafted key values. Since the API endpoints have no authentication, an attacker can inject JavaScript code through the API, and it will execute in the admin user's browser when they view the data.

**Solution:** Remove ValidateInput(false) from all actions where it is not strictly necessary. Where rich HTML content must be accepted (such as product descriptions), use a server-side HTML sanitization library to strip dangerous tags and attributes before saving to the database. Replace Html.Raw with Html.Encode in views wherever the content does not require HTML rendering. Replace inline JavaScript onclick handlers with data attributes and unobtrusive JavaScript. For content that must be rendered as HTML, sanitize it both on input and on output.

---

### 1.8 Destructive Operations Accessible via GET Requests

**Problem:** Several destructive operations are marked as HTTP GET or have no HTTP method attribute (which defaults to GET). These include member deletion, address removal, cart removal, order cancellation, push notification sending, and payment status updates. GET requests can be triggered by simply clicking a link, loading an image tag, or being included in a phishing email, making these operations vulnerable to Cross-Site Request Forgery attacks.

**Solution:** Change all destructive operations to use HTTP POST, PUT, or DELETE methods with the appropriate attributes. Add ValidateAntiForgeryToken to all POST actions in the admin area. For API endpoints, CSRF protection comes from proper authentication tokens (such as JWT in headers rather than cookies).

---

### 1.9 Missing CSRF Protection on Admin Forms

**Problem:** Out of all the admin controller POST actions, only 4 use the ValidateAntiForgeryToken attribute. All other admin forms for creating users, categories, brands, countries, cities, web content, and most other operations have no CSRF protection. The login form in Areas/Admin/Views/Auth/Login.cshtml does not include the AntiForgeryToken helper and the AuthController Login action does not have the ValidateAntiForgeryToken attribute.

**Solution:** Add the ValidateAntiForgeryToken attribute to all POST actions in the admin area. Ensure that all admin forms include the AntiForgeryToken helper in their Razor views. Consider registering a global CSRF filter for the admin area to prevent any form submissions without a valid token.

---

### 1.10 Connection String Injection in Excel Upload

**Problem:** The Excel bulk upload feature in the Product controller takes a password value from the form request and concatenates it directly into an OleDb connection string without any sanitization. An attacker could inject additional connection string parameters to redirect the data source or modify the connection behavior.

**Solution:** Validate and sanitize the password parameter before including it in the connection string. Use a connection string builder that safely handles parameter values. Better yet, eliminate the need for password-protected Excel files or handle the password through a separate, validated channel.

---

### 1.11 Broken Role-Based Authorization

**Problem:** The CustomPrincipal class has an IsInRole method that throws NotImplementedException. This means any attempt to implement role-based authorization using the standard ASP.NET [Authorize(Roles="Admin")] attribute will crash the application with an unhandled exception. The entire role-based access control system is fundamentally broken.

**Solution:** Implement the IsInRole method properly by checking user roles from the database or claims. Create a roles table if one does not exist, assign roles to users, and verify roles through the CustomPrincipal implementation.

---

### 1.12 Silent Authentication Failure in Global.asax

**Problem:** The Global.asax.cs PostAuthenticateRequest handler wraps the authentication cookie parsing in a try-catch block that silently swallows all exceptions. If the authentication cookie is malformed, tampered with, or corrupted, the exception is caught and ignored, meaning the request proceeds as unauthenticated without any logging or redirect to the login page.

**Solution:** Log all authentication exceptions and fail gracefully by redirecting to the login page. Never silently swallow authentication-related errors.

---

### 1.13 Additional Server-Side Security Issues

The system runs with debug mode enabled in Web.config, which exposes detailed error pages and stack traces to users. There are no custom error pages configured, so ASP.NET default error pages reveal internal paths and framework details. HTTPS is not enforced anywhere in the application — there is no RequireHttps attribute, no HSTS headers, and no URL rewrite rules. The authentication cookie is set without the Secure, HttpOnly, or SameSite flags, and has an excessively long 15-day expiration. The Swagger UI is unconditionally enabled and publicly accessible, providing attackers a complete map of all API endpoints. Test controllers (FakeController and DemoController) are present in the production codebase with hardcoded fake data. Internal exception messages are returned to clients in the CourierController error handling. The DataAccessor helper class allows raw SQL input from callers, creating a potential SQL injection vector if callers concatenate user input into SQL strings.

**Solution:** Set debug to false in Web.config for production environments. Configure custom error pages to display user-friendly error messages while logging the details server-side. Enforce HTTPS with the RequireHttps attribute or URL rewrite rules, and add HSTS headers. Set the authentication cookie with Secure, HttpOnly, and SameSite flags, and reduce the expiration to a reasonable duration. Conditionally enable Swagger only in development environments. Implement the IsInRole method properly. Remove FakeController and DemoController from production code. Replace exception message exposure with generic error responses while logging the actual exception internally.

---

## 2. Security Vulnerabilities — Mobile App (Flutter)

### 2.1 SSL/TLS Certificate Validation Completely Disabled

**Problem:** The main.dart file contains a MyHttpOverrides class that overrides the global HttpClient to accept ALL SSL certificates regardless of validity. The badCertificateCallback is set to always return true. This means the app will happily connect to a server presenting a self-signed, expired, revoked, or fraudulently issued certificate. This is the single most critical vulnerability in the mobile application, as it enables man-in-the-middle attacks on every single API call the app makes, including login, payment, and personal data requests.

**Solution:** Delete the MyHttpOverrides class entirely from main.dart. Remove the HttpOverrides.global assignment. For development environments, use build flavors or compile-time flags to conditionally enable certificate bypass, never in production. Implement certificate pinning using a library such as http_certificate_pinning to ensure the app only connects to servers presenting the expected certificate.

---

### 2.2 Payment Gateway API Key Hardcoded in Source Code

**Problem:** The full MyFatoorah payment API key is hardcoded as a static variable in lib/core/utils/services/store_key_const.dart. This is an extremely long production API token that grants complete access to the payment gateway. Anyone who decompiles the APK or IPA (which is trivial with publicly available tools) can extract this key and use it to process unauthorized transactions, view transaction history, issue refunds, or perform any payment operation the key authorizes.

**Solution:** Immediately revoke this API key through the MyFatoorah dashboard and generate a new one. Never store payment API keys in client-side code. Move payment processing to the server side — the mobile app should send payment intent to the backend, and the backend should communicate with MyFatoorah using the API key stored securely in server configuration. If client-side payment must be used, retrieve a time-limited session token from the backend at the time of payment.

---

### 2.3 Firebase API Keys Exposed in Source Code

**Problem:** The lib/firebase_options.dart file contains hardcoded Firebase API keys for Web (AIzaSyAHp4U4rVj-zlfyBJGEiLRExoAnRQd3Yuw), Android (AIzaSyCybfzKcKQfFOsRSn9uPe12j2TOxd5K434), and iOS (AIzaSyBblDc1ZTpR2HM6IA5J0LMf0OyR6gupgDw), along with the Firebase project ID (boulevard-a50a0) and all messaging sender IDs and app IDs. While Firebase API keys have limited scope, combined with the complete absence of Firebase Security Rules configuration in the project, an attacker could potentially access Firestore data, Cloud Storage, or abuse Firebase services.

**Solution:** Review and tighten Firebase Security Rules for all Firebase services (Firestore, Realtime Database, Cloud Storage). Restrict API key usage by platform and bundle identifier in the Google Cloud Console. Enable App Check to verify that only your genuine app can interact with Firebase services. Consider using Firebase Remote Config for storing sensitive configuration values.

---

### 2.4 Google OAuth Client Credentials Hardcoded

**Problem:** The sign_in_controller.dart file contains hardcoded Google OAuth server client ID and client ID at two separate locations (lines 72-75 and duplicated at lines 283-285). These credentials (581120393197-6u40adc8d92qj7v4dll002if14up5q0i.apps.googleusercontent.com and com.googleusercontent.apps.581120393197-t2e9kmpu1c1t0iksj4g00r8af4hhf6ac) can be used by an attacker to create a phishing application that mimics the legitimate OAuth flow and steals user tokens.

**Solution:** Move OAuth client IDs to Firebase Remote Config or environment-based configuration using --dart-define. The server client ID should only be used on the backend server. Implement PKCE (Proof Key for Code Exchange) for the OAuth flow to prevent authorization code interception.

---

### 2.5 UAE Pass Credentials Hardcoded

**Problem:** The sign_in_controller.dart file contains hardcoded UAE Pass sandbox credentials (clientId: "sandbox_stage", clientSecret: "sandbox_stage") at lines 51-62. While these are sandbox credentials, the isProduction flag is set to false, meaning the production app is configured to use the sandbox environment. Additionally, a developer email (emmacharwatson60@gmail.com) is visible in a code comment.

**Solution:** Remove hardcoded credentials and use environment-based configuration. Set isProduction to true for release builds using build flavors. Store the client secret on the backend server only — the mobile app should proxy UAE Pass authentication through the backend. Remove developer email addresses from code comments.

---

### 2.6 Facebook App Credentials Exposed

**Problem:** The Facebook App ID (876083431755345) and Client Token (1d2d8701fcb8105801e71f429bd372b8) are exposed in both AndroidManifest.xml and Info.plist. While these are somewhat expected in manifest files, the client token provides elevated access beyond the public API key.

**Solution:** Restrict the Facebook App settings to only allow connections from the registered app bundle identifiers and signing certificates. Enable advanced access controls in the Facebook Developer dashboard. Monitor the app for unauthorized access patterns.

---

### 2.7 Google Maps API Key Exposed Without Restrictions

**Problem:** The Google Maps API key (AIzaSyCzFiSh8ZG6PJfBn8EhMq9288dNVrkPbjM) is hardcoded in AndroidManifest.xml and referenced via GIDClientID in the iOS Info.plist. There are no API key restrictions configured in the Google Cloud Console, meaning anyone who extracts the key can use it for unlimited Maps API calls, resulting in significant billing charges.

**Solution:** Restrict the Google Maps API key in the Google Cloud Console. Set application restrictions by Android package name and SHA-1 certificate fingerprint for Android, and by iOS bundle identifier for iOS. Set API restrictions to only allow the Maps SDK API. Create separate keys for Android and iOS with platform-specific restrictions.

---

### 2.8 Missing Authorization Header in API Service

**Problem:** The api_service.dart file's _getHeadersBearer method creates headers with only Content-Type and Accept, but never includes an Authorization Bearer token. This means all API requests from the mobile app are sent without authentication credentials, relying entirely on the fact that the server endpoints have no authentication either. This creates a complete authentication vacuum across the entire system.

**Solution:** Implement proper token management. After login, store the JWT token securely using flutter_secure_storage. Add the Authorization header with Bearer token to all API requests. Implement token refresh logic to handle token expiration. Add an HTTP interceptor that automatically attaches the token and handles 401 responses by refreshing the token or redirecting to login.

---

### 2.9 Passwords and Sensitive Data Stored Unencrypted

**Problem:** The local_storage_service.dart stores all user data including the complete login response (which contains the password field) in SharedPreferences, which is unencrypted plaintext storage. On Android, SharedPreferences data is stored in an XML file accessible via ADB on rooted devices or through backup extraction. On iOS, it is stored in a plist file accessible on jailbroken devices. The login response model (login_response_model.dart) includes a password field that should never be stored on the client.

**Solution:** Replace SharedPreferences with flutter_secure_storage for all sensitive data (tokens, user credentials, personal information). Remove the password field from the login response model entirely — the server should never return the user's password. If the server returns it, the app should strip it before storage. Use encrypted_shared_preferences as a minimum baseline for Android.

---

### 2.10 Sensitive Data Exposed in Debug Logs

**Problem:** Over 50 locations throughout the mobile app codebase use print() or logger.i() to output sensitive data to the system log. This includes: full API response bodies with user data (login_api.dart line 17, 31, 82), FCM tokens (sign_in_controller.dart line 131), API request bodies containing passwords (login_api.dart line 15, register_api.dart line 15), price and transaction data (booking_and_location_controller.dart line 54), all API response status codes and bodies (typing_api.dart 10+ instances, hotel_booking_api.dart 8+ instances, real_estate_api.dart 8+ instances, motors_api.dart 9+ instances). On Android, these logs are accessible through adb logcat. On iOS, they are visible through the Console app.

**Solution:** Remove all print() statements from production code. Use the already-configured logger package with proper log levels, and ensure that in release mode, logging is disabled or filters out sensitive data. Never log request bodies, response bodies, tokens, or personal information. Implement a logging filter that automatically redacts sensitive fields.

---

### 2.11 No Session Timeout or Token Expiration

**Problem:** The mobile app has no session timeout mechanism. Once a user logs in, the session persists indefinitely. There is no token expiration handling, no refresh token flow, and no automatic logout after a period of inactivity. If a user's device is stolen, the attacker has permanent access to the user's account.

**Solution:** Implement token expiration on the server side (JWT tokens with a 15-30 minute expiration). Implement refresh tokens with a longer but still limited expiration (7-30 days). Add an inactivity timeout on the mobile app (auto-logout after 15 minutes of inactivity). Provide users with a "log out of all devices" option. Handle 401 responses from the API by clearing local storage and redirecting to the login screen.

---

### 2.12 No Root/Jailbreak Detection

**Problem:** The mobile app performs no check for rooted (Android) or jailbroken (iOS) devices. On rooted/jailbroken devices, all security boundaries enforced by the operating system are broken, meaning other apps can read the app's storage, intercept network traffic, modify the app's behavior at runtime, and extract decrypted data from memory.

**Solution:** Implement root and jailbreak detection using a library such as flutter_jailbreak_detection or root_checker. When a compromised device is detected, inform the user and restrict sensitive operations (especially payments). For high-security requirements, prevent the app from running entirely on compromised devices.

---

### 2.13 No Code Obfuscation Configuration

**Problem:** While the Android build.gradle has minifyEnabled and shrinkResources set to true, the proguard-rules.pro file does not exist. This means ProGuard/R8 is configured to run but has no custom rules, potentially breaking the app at runtime by obfuscating classes that rely on reflection (such as JSON serialization). For Flutter/Dart, there is no --obfuscate flag configured in the build process, meaning Dart code is not obfuscated and can be easily reverse-engineered using tools like flutter_extract.

**Solution:** Create proguard-rules.pro with rules to keep all model classes, Firebase classes, and reflection-dependent code. Add the --obfuscate and --split-debug-info flags to the Flutter build command. Configure the CI/CD pipeline to always build with obfuscation enabled for release builds.

---

### 2.14 Missing Network Security Configuration (Android)

**Problem:** The Android app has no network_security_config.xml file. Without this configuration, the app relies on the default Android network security behavior which, while reasonably secure, does not implement certificate pinning or restrict which domains the app can communicate with. Combined with the disabled SSL certificate validation in main.dart, this creates a completely open network surface.

**Solution:** Create a res/xml/network_security_config.xml file that explicitly whitelists the app's backend domain, disables cleartext traffic for all domains, and implements certificate pinning for the production API endpoint. Reference this file in AndroidManifest.xml using the android:networkSecurityConfig attribute on the application element.

---

### 2.15 iOS Push Notification Environment Set to Development

**Problem:** The Runner.entitlements file has aps-environment set to "development" instead of "production". This means push notifications will not work when the app is distributed through the App Store or TestFlight, as Apple's push notification service routes messages to different servers based on this setting.

**Solution:** Change the aps-environment value to "production" in the Runner.entitlements file for release builds. Use separate entitlements files for debug and release configurations, or configure the build settings to use the correct entitlements per build scheme.

---

### 2.16 Unvalidated Deep Link URL Schemes

**Problem:** Both AndroidManifest.xml and Info.plist register the custom URL scheme "myappscheme" with handlers for "success" and "failure" hosts. This scheme is publicly registered and any app installed on the device can trigger these deep links. There is no validation of the source of the deep link, meaning a malicious app could craft a deep link to bypass payment verification or inject data into the app's navigation flow.

**Solution:** Implement App Links (Android) and Universal Links (iOS) instead of custom URL schemes, as these require domain verification and cannot be impersonated. If custom URL schemes must be used, validate the incoming URL parameters and source thoroughly before processing.

---

## 3. Security Vulnerabilities — Platform Configuration (Android + iOS)

### 3.1 Keystore Password "123456" in Source Control

**Problem:** The android/key.properties file is committed to source control with the keystore password and key password both set to "123456" — the most commonly used weak password. The keystore file (boulevard-keystore.jks) and its alias (boulevard) are also exposed. Anyone with access to the repository can sign release APKs that will be accepted by Google Play as legitimate updates, enabling malicious updates to be pushed to all users.

**Solution:** Immediately change the keystore passwords to strong, randomly generated values (minimum 20 characters). Remove key.properties from source control and add it to .gitignore. Store keystore credentials in a secure CI/CD secrets manager (GitHub Actions Secrets, Bitrise Secrets, or Codemagic Environment Variables). Rotate the keystore if there is any possibility of unauthorized access.

---

### 3.2 Missing ProGuard Rules File

**Problem:** The Android build.gradle references proguard-rules.pro in the release build type, but this file does not exist on disk. Since minifyEnabled and shrinkResources are both set to true, ProGuard/R8 will run during release builds but will have no custom rules. This can cause runtime crashes when R8 removes or renames classes that are accessed via reflection (JSON deserialization, Firebase, payment SDK classes).

**Solution:** Create android/app/proguard-rules.pro with keep rules for all model classes used in JSON serialization, Firebase messaging classes, MyFatoorah payment SDK classes, Google Maps classes, and any other reflection-dependent libraries. Test the release build thoroughly to catch obfuscation-related crashes.

---

### 3.3 Gradle Configuration Conflicts

**Problem:** The android/gradle.properties file sets android.compileSdk=34 while the android/app/build.gradle file sets compileSdk 36. The root build.gradle also forces compileSdkVersion 36 on all subprojects. These conflicting values can cause build failures or unexpected behavior in dependency resolution.

**Solution:** Align all SDK version references to a single value across all gradle files. Use the gradle.properties file as the single source of truth and reference the property in build.gradle files.

---

### 3.4 Google Services Plugin Commented Out

**Problem:** The root build.gradle has the Google Services Gradle plugin (com.google.gms:google-services) commented out. This plugin is required for Firebase to properly read the google-services.json configuration file at build time. Without it, Firebase may use fallback configuration which could lead to incorrect project binding or service failures.

**Solution:** Uncomment the Google Services plugin dependency and ensure the plugin is applied in the app-level build.gradle. Verify that google-services.json exists in the android/app directory with correct Firebase project configuration.

---

### 3.5 iOS Entitlements Not Split by Build Configuration

**Problem:** The Runner.entitlements file is used for both debug and release builds. The aps-environment is set to "development", which is correct for debug but incorrect for production. There are no separate entitlements files for different build configurations.

**Solution:** Create separate entitlements files: Runner.debug.entitlements with aps-environment set to development, and Runner.release.entitlements with aps-environment set to production. Configure the Xcode project to use the appropriate entitlements file per build scheme.

---

### 3.6 Version Number Mismatch Between Platforms

**Problem:** The pubspec.yaml has the Android version set to 1.0.1+2 while the iOS version is commented out as 1.0.0+3. This indicates that platform versions are managed separately and inconsistently, which can cause confusion about which features are included in each platform's release.

**Solution:** Use a single version number in pubspec.yaml for both platforms. Remove the commented-out iOS version. If platform-specific versioning is needed, manage it through build scripts rather than code comments.

---

### 3.7 Minimum SDK Version Considerations

**Problem:** The Android minSdkVersion is set to 26 (Android 8.0), which excludes approximately 5-10% of active Android devices globally. The iOS minimum version is set to 15.0, which provides good security features but excludes older devices.

**Solution:** Evaluate user base demographics before changing minimum SDK versions. If the target market (UAE) has high device refresh rates, the current values may be acceptable. If wider reach is needed, consider lowering the Android minSdkVersion to 24 (Android 7.0) with appropriate security checks for older OS versions.

---

### 3.8 Exposed Intent Filters Allowing External App Interaction

**Problem:** The AndroidManifest.xml registers an intent filter with FLUTTER_NOTIFICATION_CLICK action and BROWSABLE category. This combination allows any app on the device to trigger notification click events in the Boulevard app by crafting an appropriate intent. Similarly, the custom URL scheme (myappscheme://success and myappscheme://failure) can be invoked by any app.

**Solution:** Remove the BROWSABLE category from the notification intent filter if external URL handling is not needed. Validate all data received through intent filters before processing. Implement App Links verification for the custom URL scheme handlers to prevent URL scheme hijacking.

---

### 3.9 Permission Over-Request

**Problem:** The Android manifest requests RECORD_AUDIO permission globally even though audio recording (speech-to-text for search) is only used in specific screens. The iOS Info.plist usage descriptions for camera (NSCameraUsageDescription) and photo library (NSPhotoLibraryUsageDescription) have grammatically incorrect descriptions ("We are used this library") which are visible to users and appear unprofessional.

**Solution:** Request RECORD_AUDIO permission at runtime only when the user accesses the speech-to-text feature, not at install time. Fix all iOS permission usage descriptions to use proper English ("This app uses the camera to allow you to update your profile picture"). Keep permission descriptions specific and clear about why the permission is needed.

---

## 4. Database Performance Issues

### 4.1 Missing Indexes on Heavily Queried Columns

**Problem:** The largest tables in the database (ProductCategories with 15,535 rows, ProductPrices with 8,959 rows, Products with 4,369 rows) have only Primary Key Clustered Indexes. There are no non-clustered indexes on the columns most frequently used in WHERE clauses, including Products.Status, Products.FeatureCategoryId, Products.BrandId, ProductCategories.ProductId, ProductCategories.CategoryId, ProductCategories.Status, ProductPrices.ProductId, and ProductPrices.Status. This forces SQL Server to perform full table scans on every product-related query.

**Solution:** Create non-clustered indexes on the most-queried column combinations. The Products table needs a composite index on Status and FeatureCategoryId with includes for BrandId, ProductName, ProductPrice, and Image. The ProductCategories table needs indexes on both ProductId with Status, and CategoryId with Status including ProductId. The ProductPrices table needs an index on ProductId and Status including ProductStock, Price, and ProductQuantity. Similarly, indexes should be added to the Brands, ServiceTypes, and StockLogs tables on their most-queried columns.

---

### 4.2 Dashboard Fires 22+ Sequential Database Queries

**Problem:** The DashboardDataAccess.GetAll method fires more than 18 sequential database queries every time the admin dashboard page is loaded. Each statistic (total customers, total product orders, total service orders, total product sales, total service sales, and more) is fetched with a separate query. On top of that, a foreach loop iterates over each feature category, executing two additional queries per category (product count and service count), adding another 16 queries for 8 categories. Then, the DashboardController has four separate chart data endpoints, each of which loads the entire orders table into memory using ToListAsync, and then filters and processes the data in C# instead of SQL. This means the full orders table is loaded into memory 4 separate times.

**Solution:** Consolidate the dashboard statistics into a small number of efficient queries. Use SQL GROUP BY statements to calculate per-category counts in a single query instead of looping. Use SumAsync and CountAsync instead of loading entire tables. For the chart data, use SQL GROUP BY with MONTH to calculate monthly totals directly in the database rather than loading all records and processing them in C#. Consider caching the dashboard results for 5 to 10 minutes since dashboard statistics do not need to be real-time. All independent queries should run in parallel using Task.WhenAll rather than sequentially.

---

## 5. Entity Framework and Query Patterns

### 5.1 N+1 Query Patterns (12 Documented Instances)

**Problem:** The codebase contains at least 12 documented instances of the N+1 query pattern, where a loop iterates over a result set and fires one or more database queries per item. Notable instances include: the product listing page in ProductAccess.cs where a foreach loop queries ProductPrices per product to calculate stock (line 103); the product listing with upsell and crosssell checks firing 2 queries per product (100 products = 201 queries); the service listing where parent service names are fetched one by one; the service type loading inside a foreach loop; the service amenity loading which loads the entire ServiceAmenity table inside a loop for each service; the order request data access which fires 4 queries per order for member, address, payment method, and order status; and the dashboard category statistics loop. The DashboardController at lines 55-75 loads ALL orders into memory with ToListAsync and then filters by month in a C# for loop.

**Solution:** For each N+1 pattern, the fix follows the same principle: load all the required related data in a single batch query before the loop, then match results in memory. Use Include for eager loading instead of separate queries per entity. For order data, use Include or explicit joins to load member, address, and payment method data alongside orders in a single query. For the dashboard, use SQL GROUP BY Month to return monthly aggregates directly from the database.

---

### 5.2 While Loop Querying Database Per Iteration (Category Hierarchy)

**Problem:** When inserting a product into a category, the code walks up the category hierarchy using a while loop. Each iteration queries the database for the parent category and inserts a ProductCategory record. For a hierarchy 5 levels deep, this generates 5 separate queries and 5 separate insert-and-save operations.

**Solution:** Load the entire category hierarchy into memory once (or use the cached category tree), then traverse the in-memory tree to find all parent categories. Batch all the ProductCategory insert operations and save them in a single transaction at the end.

---

### 5.3 Multiple DbContext Instances Per Request (30+ Locations)

**Problem:** Instead of using the UnitOfWork consistently, more than 30 locations in the codebase create new BoulevardDbContext instances directly. The NotoficationService alone creates a new DbContext in 10 different methods, none of which use using statements or call Dispose. Other files including CommunitySetupAccess, MemberServiceAccess, TempServiceTypeDataAccess, ProductAccess, LayoutSettingAccess, and many others also create standalone contexts. Each new DbContext opens a new database connection, and since none are disposed, connections remain open until the garbage collector intervenes. A single HTTP request can easily result in 9 to 20 open database connections. The UnitOfWork class itself at line 5 creates a new BoulevardDbContext on every instantiation with no constructor injection support.

**Solution:** Implement dependency injection using a container such as Autofac or Unity, registering BoulevardDbContext with a per-request lifetime scope. This ensures a single DbContext is shared across all services within one HTTP request and is properly disposed at the end of the request. In the immediate term, wrap all standalone DbContext allocations in using statements to ensure proper disposal.

---

### 5.4 ToLower in LINQ Queries Prevents Index Usage

**Problem:** Many LINQ queries use .ToLower() on entity properties before comparison, which translates to the SQL LOWER() function. This function call prevents SQL Server from using indexes on those columns. This pattern appears in ProductAccess, CategoryAccess, CityAccess, CountryAccess, MemberAddressAccess, and several other service files. The comparison .Status.ToLower() == "Active" generates SQL that forces a full table scan regardless of any indexes present. Additionally, in the Status model, .ToLower() == "Active" will never match because the lowercase version of "Active" is "active", creating a logic bug.

**Solution:** SQL Server uses case-insensitive collation by default (SQL_Latin1_General_CP1_CI_AS), so the .ToLower() call is unnecessary. Remove all .ToLower() calls from LINQ queries and compare directly with the expected casing. Verify the database collation is case-insensitive, and if so, direct string comparisons will work correctly and allow index usage.

---

### 5.5 Synchronous Blocking Database Calls

**Problem:** Several database operations in the DashboardDataAccess use synchronous methods (Sum, Any) instead of their async counterparts (SumAsync, AnyAsync). When synchronous database calls execute in an ASP.NET request pipeline, they block a thread pool thread, reducing the server's ability to handle concurrent requests. Additionally, some dashboard metrics execute the same query twice — once with AnyAsync to check if data exists, then again with CountAsync or SumAsync to get the value.

**Solution:** Replace all synchronous LINQ methods with their async counterparts. Eliminate the redundant AnyAsync checks before CountAsync or SumAsync, since both return zero when no records match.

---

### 5.6 Loading Entire Tables Into Memory Then Filtering in C#

**Problem:** Multiple service methods load entire database tables into memory and then filter the data using C# LINQ-to-Objects instead of applying filters in the SQL query. The DashboardController loads all order records and then iterates over months in a C# loop. The NotoficationService loads all notifications matching a user and then processes them in memory. The ServiceAmenityAccess loads the entire amenities table inside a foreach loop. This is particularly catastrophic with large datasets — loading all orders for chart rendering can consume hundreds of megabytes of memory per request.

**Solution:** Move all filtering, grouping, and aggregation into the database query using Entity Framework LINQ expressions. Apply WHERE conditions, GROUP BY clauses, and aggregate functions at the database level so that only the required result set is transferred to the application.

---

### 5.7 Search Implementation Uses Inefficient LIKE Patterns

**Problem:** The search functionality in CategoryServiceAccess splits the search term into words and builds a LINQ expression that translates to SQL LIKE '%word%' for each word, combined with LOWER() function calls. This combination guarantees a full table scan on every search, as SQL Server cannot use any index for middle-of-string pattern matching with a leading wildcard.

**Solution:** For immediate improvement, replace Contains (which generates LIKE '%x%') with StartsWith (which generates LIKE 'x%') where the business logic allows, as prefix matching can use indexes. For a proper long-term solution, implement SQL Server Full-Text Search, which is purpose-built for text searching and supports word-based matching with proper indexing.

---

### 5.8 DbContext Missing OnModelCreating Configuration

**Problem:** The BoulevardDbContext in Contexts/BoulevardDbContext.cs only declares DbSet properties without any OnModelCreating override. There is no Fluent API configuration for composite keys, navigation properties, foreign key constraints, relationship mappings, or entity configurations. This means Entity Framework relies entirely on conventions, which can lead to incorrect relationship mappings and missing constraints.

**Solution:** Add an OnModelCreating override that explicitly configures all entity relationships, primary keys, foreign keys, and indexes. Use entity configuration classes for better organization. Define all relationships explicitly rather than relying on EF conventions.

---

## 6. Dashboard Performance

### 6.1 Dashboard Query Breakdown

**Problem:** A single dashboard page load results in the following database activity: 5 aggregate queries for top-level statistics (total customers, orders, sales), a foreach loop with 2 queries per feature category (16 queries for 8 categories), 8 more aggregate queries for weekly and monthly statistics (each with a redundant AnyAsync check, so effectively 16 queries), and 4 AJAX chart endpoints that each load the entire orders table into memory. The total is approximately 22+ sequential queries for the main page, plus 4 full table loads for the charts.

**Solution:** Combine all statistics into 2 to 3 optimized queries using GROUP BY. Replace the per-category loop with a single query that groups by FeatureCategoryId. Run independent queries in parallel using Task.WhenAll. For chart data, use SQL GROUP BY MONTH to return monthly aggregates directly from the database. Cache the entire dashboard result for 5 minutes.

---

## 7. Caching Strategy (Complete Absence — Server and Client)

### 7.1 Complete Absence of Server-Side Caching

**Problem:** The entire server-side system has zero caching of any kind. There is no use of MemoryCache, no OutputCache attributes, no HttpRuntime.Cache, no third-party caching libraries, and no Cache-Control headers on responses. Every single request hits the database directly, including data that changes rarely or never.

**Solution:** Introduce an in-memory caching layer using System.Runtime.Caching.MemoryCache. Create a centralized cache helper class that provides get-or-set semantics with configurable expiration times. Implement cache invalidation methods that are called when data is modified through CRUD operations.

---

### 7.2 Complete Absence of Client-Side Caching in Mobile App

**Problem:** The mobile app has no data caching layer whatsoever. Every screen load triggers a fresh API call to the server. There is no local database (sqflite is commented out in pubspec.yaml), no in-memory cache, and no HTTP response caching. This means the app is completely unusable without an internet connection and every navigation action requires a full data reload, creating poor user experience with visible loading delays.

**Solution:** Implement a multi-tier caching strategy: use an in-memory cache for frequently accessed data (categories, feature categories, user profile), use flutter_secure_storage or sqflite for persistent offline data, implement HTTP caching with ETag or Last-Modified support, and add cache invalidation logic when data is modified through user actions.

---

### 7.3 Layout Settings Queried 3 Times Per Page

**Problem:** On every admin page load, the LayoutSetting table is queried 3 times with identical queries — once from _Layout.cshtml, once from _Header.cshtml, and once from _Scripts.cshtml. Each call creates a new LayoutSettingAccess instance which creates a new UnitOfWork which creates a new DbContext.

**Solution:** Cache the layout settings in MemoryCache with a key like "LayoutSetting_Default" and a duration of 30 to 60 minutes. Invalidate the cache when the layout is updated through the admin panel.

---

### 7.4 Reference Data That Should Be Cached (Server and Client)

**Problem:** Static and slow-changing reference data is re-queried from the database on every request. FeatureCategories are queried in 19+ admin controller locations and in multiple mobile app API calls. Countries are queried in 12 server locations and every time the mobile app opens country pickers. Cities, Brands, Categories, ProductTypes, PaymentMethods, FAQs, and Airports are all queried fresh every time on both server and client.

**Solution:** Server side: implement caching for each type of reference data with appropriate durations. Countries, ProductTypes, and PaymentMethods can be cached for 24 hours. FeatureCategories, Cities, and Roles can be cached for 60 minutes. Client side: cache reference data locally and refresh based on Last-Modified headers or version flags.

---

### 7.5 API Endpoints Serving Static Data Need HTTP Caching

**Problem:** API endpoints for countries, cities, feature categories, brands, product types, payment methods, airports, and FAQs return data that changes infrequently, but provide no HTTP caching headers. Every mobile app or frontend client request generates a fresh database query.

**Solution:** For Web API controllers, implement a custom action filter that adds appropriate Cache-Control headers. Use cache durations of 1 hour for rarely-changing data and 15 to 30 minutes for occasionally-changing data.

---

### 7.6 Admin Notification Count AJAX Polling

**Problem:** The admin header polls for new notification counts every 5 seconds using AJAX. This generates a database query every 5 seconds per open admin browser tab. With 10 admin users, this creates 7,200 database queries per hour just for notification counts.

**Solution:** Cache the notification count per user for 30 to 60 seconds. Increase the polling interval to 30 or 60 seconds, or replace polling with SignalR for real-time push notifications.

---

## 8. Image and File Processing

### 8.1 Images Saved Without Compression

**Problem:** The MediaHelper image upload function saves images at default quality (100%) without any compression parameters. The ProductImages table contains 4,235 images. A single product image could be 2 to 5 MB in size. The mobile app downloads these full-size images for every product listing, consuming significant bandwidth and causing slow load times on mobile networks.

**Solution:** Apply JPEG compression with a quality level of 75 to 80 percent on the server. Resize images that exceed a maximum dimension (such as 1200 pixels wide) before saving. Consider converting to WebP format for better compression ratios.

---

### 8.2 No Thumbnail Generation

**Problem:** The upload process saves images at their original full resolution only. There is no automatic generation of thumbnails for list views. When the mobile app needs to display a 150-pixel-wide thumbnail in a product list, it loads the full 4000x3000 pixel original image, wasting bandwidth and memory.

**Solution:** Generate thumbnails at upload time in standard sizes (150x150 for lists, 300x300 for cards, 800x600 for detail views). Serve the appropriate size based on the display context. The mobile app API should return thumbnail URLs for list endpoints and full-size URLs for detail endpoints.

---

### 8.3 No Image Caching Configuration in Mobile App

**Problem:** The mobile app uses cached_network_image for image loading but has no configuration for cache size limits, expiration policies, or cache directory management. Over time, the image cache can grow unbounded, consuming excessive storage on the user's device.

**Solution:** Configure image cache limits (maximum cache size and maximum number of entries). Implement cache expiration policies. Use appropriately sized images through the thumbnail URLs mentioned in 8.2.

---

### 8.4 No Static File Caching or CDN

**Problem:** Images are served directly by IIS from the application directory with no Cache-Control headers. There is no Content Delivery Network configured. There is no compression enabled for static content. Every request for an image goes to the application server and reads the file from disk.

**Solution:** Add static content caching headers in Web.config to instruct browsers and mobile apps to cache images for 30 days. Enable IIS static and dynamic compression. For production, use a CDN to serve static files.

---

## 9. Frontend and Static Assets (Admin Panel)

### 9.1 Excessive Number of CSS and JS Files Per Page

**Problem:** Every admin page loads 16 CSS files and 28 JavaScript files individually, resulting in over 44 separate HTTP requests before any page content is loaded. Many of these libraries are only needed on specific pages — TinyMCE (approximately 500 KB), D3.js (approximately 280 KB), Chartist, C3.js, and FullCalendar CSS are loaded on every page even though they are only used on specific pages.

**Solution:** Use ASP.NET's bundling and minification feature to combine all commonly used CSS and JS files into bundles. Load page-specific libraries only on the pages that use them using section blocks in the layout.

---

### 9.2 Three Copies of jQuery Loaded Per Page

**Problem:** Every admin page loads jQuery three times from three different locations — once from the Head partial view, once from the Scripts partial view, and once from the Header partial view. Additionally, 14 individual view files load a fourth copy of jQuery version 1.7.1 (from 2011) over insecure HTTP. This results in approximately 300 KB of wasted bandwidth per page load and JavaScript version conflicts.

**Solution:** Load jQuery exactly once, in the Head partial view. Remove all other jQuery script tags. Remove all references to the outdated jQuery 1.7.1.

---

### 9.3 No Script Defer or Async Attributes

**Problem:** All script tags load synchronously without the async or defer attributes. jQuery is loaded in the head section, which blocks page rendering until the script is downloaded and executed.

**Solution:** Move all script tags to the bottom of the body, or add the defer attribute to prevent render blocking.

---

### 9.4 External CDN Dependencies Add Latency

**Problem:** Several resources are loaded from external CDNs (jsDelivr, CloudFlare, unpkg). Dashboard-specific views load duplicate copies of Chartist from a CDN in addition to the local copy already loaded globally.

**Solution:** Host all third-party libraries locally and bundle them. Remove duplicate CDN loads.

---

### 9.5 No HTTP Compression Configured

**Problem:** Web.config has no urlCompression or httpCompression settings. All HTML, CSS, JavaScript, and JSON responses are sent at full uncompressed size, typically 3 to 5 times larger than compressed.

**Solution:** Enable both static and dynamic compression in Web.config under the system.webServer section.

---

## 10. Pagination (Missing Across the Entire System)

### 10.1 29 Out of 30 Admin Listing Endpoints Load All Data

**Problem:** Across the entire admin area, 29 out of 30 listing endpoints load all records from the database without any server-side pagination. Only the MemberController has partial pagination support. The PaginatedList class has a critical flaw: its CreateAsync method calls ToListAsync on the full queryable without applying Skip and Take.

**Solution:** Implement proper server-side pagination with Skip and Take in all list queries. Fix the PaginatedList.CreateAsync method. Each listing endpoint should accept page number and page size parameters.

---

### 10.2 All DataTables Use Client-Side Processing

**Problem:** Approximately 35 DataTable instances across the admin views all use client-side processing mode. The server renders all rows into the HTML DOM. For tables with thousands of records, this causes significant delays and high browser memory consumption.

**Solution:** Switch DataTables to server-side processing mode with AJAX source URLs. Create server-side endpoints that accept DataTables parameters and return only the requested page of data.

---

### 10.3 Mobile App Lists Missing Pagination UI

**Problem:** While some mobile app API calls include count and size parameters suggesting pagination support, the UI does not implement pagination controls or infinite scroll for any list view. Real estate listings, service listings, hotel listings, and product listings all appear to load data in a single request without lazy loading or pagination feedback to the user.

**Solution:** Implement infinite scroll with a scroll listener that triggers loading the next page when the user reaches the bottom of the list. Show a loading indicator at the bottom while the next page is being fetched. Track the current page and total pages to avoid unnecessary API calls.

---

## 11. API Layer Issues (Server Side)

### 11.1 N+1 Query in Search Endpoint

**Problem:** The SearchAllProductAndService endpoint first loads all feature categories, then iterates over each category and fires two additional queries per category. For 10 categories, this results in 21 database queries per search request.

**Solution:** Restructure the search to execute product and service queries with WHERE IN clauses based on category IDs rather than per-category loops.

---

### 11.2 Useless Try-Catch-Throw Patterns

**Problem:** 22 or more methods across multiple controllers wrap their logic in try-catch blocks that catch Exception and immediately re-throw it with no additional handling, logging, or transformation.

**Solution:** Remove all try-catch blocks that only contain throw with no additional logic. Implement global exception handling through a filter or middleware.

---

### 11.3 Inconsistent Response Format

**Problem:** Different controllers use different response envelope formats. NotificationController uses different property names. UserReportController uses a completely different object structure. The BaseController's ErrorMessage method always returns HTTP status 200 with an internal error status code.

**Solution:** Standardize the API response envelope across all controllers. Return appropriate HTTP status codes (400, 401, 404, 500) instead of always returning 200.

---

### 11.4 Mixing Static and Instance Method Calls

**Problem:** The NotificationController inconsistently calls some notification service methods as static methods and others on new instances, indicating inconsistent design.

**Solution:** Choose one consistent approach for all service methods. Avoid mixing static and instance patterns in the same class.

---

### 11.5 Wrong Variable Returned in Third-Party Login

**Problem:** In the MemberController's third-party login handler (line 146-153), when a new member is registered via a third-party provider, the code sets ThirdPartyLogin to true on the newly registered member object but then returns a different variable (loginMember) which is an empty Member object with no data. This bug silently discards the registration result and returns empty data to the client.

**Solution:** Return the correct variable (registeredMember instead of loginMember) in the success response after third-party registration.

---

### 11.6 Duplicate Condition Check in Order Validation

**Problem:** The OrderRequestController at lines 24-25 checks the same condition twice: model.MemberId == 0 || model.MemberId == 0 instead of the intended model.MemberId == 0 || model.MemberAddressId == 0. This means the MemberAddressId is never validated and could be zero, causing downstream errors.

**Solution:** Fix the condition to check both MemberId and MemberAddressId correctly.

---

### 11.7 Repository Method Typo

**Problem:** The GenericRepository has a method named "Addd" (triple 'd') on line 35, which is a duplicate of the Add method. This creates confusion and the synchronous version calls SaveChanges immediately (blocking).

**Solution:** Remove the duplicate Addd method. Use the async Add method consistently.

---

## 12. API Layer Issues (Mobile App Client Side)

### 12.1 No Request Timeout Configuration

**Problem:** The api_service.dart file makes all HTTP requests without any timeout configuration. A network glitch or unresponsive server will cause the HTTP client to wait indefinitely, freezing the UI and consuming memory. The user has no way to cancel the operation.

**Solution:** Add a timeout to all HTTP requests: implement a default timeout of 30 seconds for standard requests and 60 seconds for file uploads. Use the http package's timeout extension or wrap calls with Future.timeout. Show a timeout error to the user when the timeout is exceeded.

---

### 12.2 No Retry Logic on Any API Call

**Problem:** Every API call in the mobile app makes exactly one attempt. If a transient network error occurs (brief disconnection, server momentary overload, DNS timeout), the request fails permanently and the user must manually retry by navigating away and back.

**Solution:** Implement automatic retry with exponential backoff for idempotent requests (GET, PUT, DELETE). Use 3 retries with delays of 1, 2, and 4 seconds. Do not retry POST requests automatically (risk of duplicate submissions). Use a retry library or implement a custom retry decorator for the API service.

---

### 12.3 No HTTP Interceptors for Common Concerns

**Problem:** The api_service.dart handles each HTTP method independently with no shared interceptor chain. Common concerns like adding authorization headers, logging requests, handling 401 responses, and transforming errors are handled inconsistently or not at all across the codebase.

**Solution:** Replace the raw http.Client with a more capable HTTP client like dio, which provides built-in interceptor support. Implement interceptors for: authentication token injection, 401 response handling with automatic token refresh, request/response logging (in debug mode only), error standardization, and request deduplication.

---

### 12.4 Only HTTP 200 Status Code Handled

**Problem:** Every API method in the mobile app checks only if res.statusCode == 200 and returns a generic AppError.httpError for all other status codes. There is no differentiation between 400 (Bad Request), 401 (Unauthorized), 403 (Forbidden), 404 (Not Found), 429 (Rate Limited), 500 (Server Error), or 503 (Service Unavailable). This makes debugging impossible and provides no useful feedback to the user.

**Solution:** Implement comprehensive status code handling. For 401, trigger token refresh or redirect to login. For 400, parse validation errors and display them. For 429, implement backoff and retry. For 500/503, show a server error message and suggest the user try again later. For 404, show a "not found" state.

---

### 12.5 Hardcoded Base URL With No Environment Switching

**Problem:** The urls.dart file has the base URL (https://boulevard.r-y-x.net) and API version (/api/v1) hardcoded as static strings. There is no mechanism to switch between development, staging, and production environments without recompiling the app.

**Solution:** Use Dart compile-time environment variables (--dart-define) to set the base URL per build environment. Create a configuration class that reads the environment and provides the appropriate base URL. Alternatively, use build flavors (Android) and schemes (iOS) to manage environment-specific configurations.

---

### 12.6 Internet Connectivity Check Is Non-Reactive

**Problem:** The InternetService connectivity check is called synchronously before each API request. If the device loses connectivity during a request, the failure is not handled gracefully. There is no global connectivity listener that updates the UI when the device goes offline or comes back online.

**Solution:** Implement a reactive connectivity listener using the connectivity_plus package's onConnectivityChanged stream. Show a global offline banner when connectivity is lost. Queue API requests when offline and execute them when connectivity is restored (for non-time-sensitive operations).

---

### 12.7 Broken Exception Handler

**Problem:** The exception_handlers.dart file at line 10 attempts to access error.message on an AppError enum, but Dart enums do not have a message property by default. This causes a crash when the app tries to handle an unauthorized error, creating a secondary failure on top of the original error.

**Solution:** Add a message extension or property to the AppError enum that returns appropriate user-facing error messages for each error type.

---

## 13. Architecture and Clean Code Violations (Server Side)

### 13.1 Repository Pattern Defeats Its Own Purpose

**Problem:** The GenericRepository's Add, Edit, and Remove methods each call SaveChanges immediately after the operation. This defeats the entire purpose of the Unit of Work pattern, which is to batch multiple operations into a single database transaction. The MultipleRemove method calls SaveChanges inside a foreach loop, resulting in N database roundtrips instead of one.

**Solution:** Remove SaveChanges calls from all individual repository methods. Add SaveChanges and SaveChangesAsync methods to the IUnitOfWork interface. Service methods should call SaveChanges once at the end of a logical transaction.

---

### 13.2 IUnitOfWork Missing Critical Methods

**Problem:** The IUnitOfWork interface does not extend IDisposable and does not declare SaveChanges or SaveChangesAsync methods. Code using the interface cannot dispose the underlying DbContext or commit batches of changes.

**Solution:** Extend IUnitOfWork to inherit from IDisposable. Add SaveChanges and SaveChangesAsync method declarations.

---

### 13.3 Public Fields on Repository and UnitOfWork

**Problem:** GenericRepository exposes its BoulevardDbContext and DbSet as public fields. UnitOfWork also exposes its DbContext as a public field, allowing bypass of the repository pattern.

**Solution:** Change all public fields to private (or protected where subclass access is needed). Add readonly modifiers.

---

### 13.4 No Dependency Injection Container

**Problem:** The entire server application has no IoC container. Every service class is instantiated using the new keyword in controller constructors. This makes unit testing impossible and results in cascading DbContext creation.

**Solution:** Introduce a dependency injection container (Autofac, Unity, or Simple Injector) and register all service interfaces with their implementations. Register BoulevardDbContext with per-request lifetime scope.

---

### 13.5 Cascading Service Instantiation Creates Excessive DbContexts

**Problem:** When a service method needs data from another domain, it creates a new instance of that service using the new keyword. Each creates its own UnitOfWork and DbContext. A single request can cascade into 10 to 20 DbContext instances. For example, processing orders involves OrderRequestServiceAccess creating ProductServiceAccess (new UnitOfWork), which creates OfferServiceAccess (another new UnitOfWork), which creates MemberServiceAccess (yet another new UnitOfWork).

**Solution:** With DI, all service classes receive the same DbContext instance per request. Without DI as an immediate fix, refactor service methods to accept an existing UnitOfWork as a parameter.

---

### 13.6 Service Layer Contains Presentation Logic

**Problem:** Multiple service classes contain URL construction logic using HttpContext.Current.Request.Url. Language switching logic is embedded directly in data access methods.

**Solution:** Move URL construction to a separate mapper or DTO factory. Move language switching to a localization service.

---

### 13.7 Massive Service Files (God Classes)

**Problem:** Several service files have grown extremely large: ServiceAccess.cs (1,804 lines), CategoryServiceAccess.cs (834 lines), ProductAccess.cs (807 lines), and OfferServiceAccess.cs (790 lines).

**Solution:** Break down large service classes into smaller, focused classes. ProductAccess could be split into ProductQueryService, ProductCommandService, and ProductStockService.

---

### 13.8 Dead and Test Code in Production

**Problem:** The ServiceAccess class contains methods with hardcoded fake test data. FakeController and DemoController exist in the production controller directory. Multiple service files have methods marked async but performing no asynchronous work.

**Solution:** Remove all test and mock data from production service classes. Remove FakeController and DemoController. If test doubles are needed, create them in a separate test project.

---

### 13.9 Hardcoded Magic Numbers

**Problem:** CreateBy = 1 is set in every insert and update, meaning the audit trail always shows user 1. Feature category IDs 9, 11, 13 are used in conditional logic without explanation. The phone prefix "+971" is hardcoded. Order ID prefixes and timeout durations are hardcoded.

**Solution:** Replace hardcoded IDs with named constants or database-driven configuration. For CreateBy, use the actual authenticated user's ID. For country-specific logic, store configuration in the database.

---

### 13.10 Exception Swallowing (30+ Locations)

**Problem:** Every service method follows a pattern of catching all exceptions, logging them with Log.Error, and returning null. Callers do not check for null, resulting in NullReferenceException further up the call stack. The actual error is hidden and null propagates silently. Found in BrandAccess, ProductAccess, MemberServiceAccess, ProductServiceAccess, and 26+ other files.

**Solution:** For expected failure cases, use result types or specific exceptions. For unexpected errors, allow the exception to propagate to the global exception handler. Where null is a valid return value (entity not found), distinguish it clearly from error cases.

---

## 14. Architecture and Clean Code Violations (Mobile App)

### 14.1 No Consistent Architecture Pattern

**Problem:** The mobile app uses GetX for state management but does not follow a consistent architectural pattern. Controllers directly call API classes (e.g., MotorsApi().getServiceList()), bypassing any repository or use case layer. There is no separation between data sources, repositories, domain logic, and presentation. The sign_in_controller.dart alone is 426 lines long, handling Google Sign-In, Facebook Login, Apple Sign-In, UAE Pass login, Firebase token management, and local storage — mixing 6 different concerns in a single file.

**Solution:** Implement a clean architecture pattern: View → Controller → UseCase → Repository → DataSource. Extract social authentication into separate handler classes. Create repository interfaces for each domain (AuthRepository, ProductRepository, OrderRepository). This enables unit testing, improves maintainability, and allows swapping data sources (API vs local cache) transparently.

---

### 14.2 Incomplete Dependency Injection

**Problem:** The app_dic.dart file registers only 3 services with GetX (SharedPreferences, LocalStorageService, TranslationRepository). All other services (ApiService, NotificationService, InternetService, and all module-specific APIs) are created inline with the new keyword wherever they are needed. This makes testing impossible and creates tight coupling between controllers and their dependencies.

**Solution:** Register all service interfaces in the AppDependencies class using GetX dependency injection. Use Get.find() instead of direct instantiation. Create interface abstractions for all services to enable testing with mocks.

---

### 14.3 Controllers Are God Objects

**Problem:** Multiple controllers handle too many responsibilities: direct API calls (should be in repositories), state management, business logic, UI navigation, error handling, and data transformation. The MotorsBeautyMedicalHomeController, RealEstateHomeController, and HotelBookingController all contain API call logic, state management, and navigation logic in the same class with no separation of concerns.

**Solution:** Extract API calls into Repository classes. Move business logic into UseCase or Interactor classes. Keep controllers focused on managing UI state and responding to user interactions. Each class should have a single reason to change.

---

### 14.4 Direct API Calls in View Layer

**Problem:** Some view widgets make direct API calls, completely bypassing the controller/state management layer. For example, motors/views/service_details_view.dart at line 51 calls MotorsApi().onAddRemoveService(body) directly in the widget's build method. This violates the fundamental principle of separating UI from business logic and data access.

**Solution:** Move all API calls to the associated controller or, better, to a repository class. Views should only call controller methods, which in turn coordinate data access and state updates. This ensures proper loading state management, error handling, and testability.

---

### 14.5 Tight Inter-Module Coupling

**Problem:** Controllers reach across module boundaries to access other controllers' state. For example, home_controller.dart at line 127 uses Get.find<HomeController>().isSubscribe.value to check subscription status from another module. This creates hidden dependencies and makes individual modules impossible to develop or test in isolation.

**Solution:** Implement a shared service or event bus for cross-module communication. Use abstract interfaces or dependency injection to decouple modules. Create a shared app state service that modules can subscribe to without knowing about each other.

---

### 14.6 Inconsistent Navigation Pattern

**Problem:** The app mixes named routes with anonymous navigation. Some screens are navigated using Get.toNamed('/route'), others use Get.to(()=>ScreenWidget()), and some use direct instantiation within navigation methods. Named routes provide type safety and centralized route management, while anonymous navigation scatters route definitions across the codebase. Route helpers create new widget instances on every navigation.

**Solution:** Standardize on named routes with a centralized route configuration. Define all routes in the routes directory. Pass parameters through route arguments rather than constructor parameters.

---

### 14.7 Missing Dispose/OnClose Methods (Memory Leaks)

**Problem:** Multiple controllers create TextEditingController, ScrollController, and AnimationController instances without disposing them in the onClose method. This causes memory leaks that accumulate as users navigate through the app. Affected controllers include: typing_home_controller.dart (6 TextEditingControllers + ScrollController with addListener), real_estate_list_controller.dart (3 TextEditingControllers + ScrollController), sign_up_controller.dart (2 TextEditingControllers), hotel_booking_controller.dart (2 TextEditingControllers), enquiry_controller.dart (3 TextEditingControllers), change_password_controller.dart (3 TextEditingControllers), hotel_search_controller.dart (6 TextEditingControllers), favourite_controller.dart (TextEditingController + ScrollController), home_search_controller.dart (TextEditingController + ScrollController), profile_controller.dart (1 TextEditingController).

**Solution:** Add onClose methods to all controllers that create disposable resources. Call dispose() on all TextEditingControllers, ScrollControllers, and AnimationControllers. Remove scroll listeners in onClose.

---

### 14.8 No Global Error Handling

**Problem:** The app has no global error handler for uncaught exceptions. If an unhandled exception occurs (which is likely given the missing null checks and type casting without validation), the app crashes with a platform-default error screen. There is no crash reporting service (Crashlytics, Sentry) configured.

**Solution:** Wrap the app in a FlutterError.onError handler and a PlatformDispatcher.instance.onError handler to catch all uncaught exceptions. Integrate Firebase Crashlytics or Sentry for remote crash reporting. Show a user-friendly error screen instead of the default red error screen.

---

### 14.9 Incomplete Implementations (TODOs in Production Code)

**Problem:** Multiple files contain TODO comments indicating unfinished implementations: typing_service_details_controller.dart line 21 has "TODO: implement onInit" with no implementation, sign_in_view.dart line 365 has "TODO: Uncomment otp" suggesting OTP is disabled, medical_controller.dart line 5 has a wrong module comment copied from the grocery module. There are 20+ TODO comments scattered across the modules indicating incomplete features.

**Solution:** Audit all TODO comments and either complete the implementation or create tracked tickets for each. Do not ship code with TODO comments that represent missing functionality.

---

### 14.10 Missing Model Validation and Parse Error Handling

**Problem:** All model classes in the mobile app parse JSON without any error handling. The fromJson factory methods directly access JSON keys without null checks or type validation. If the API returns an unexpected format, missing field, or wrong type, the app crashes with an unrecoverable exception. For example, login_response_model.dart directly accesses json["memberId"], json["email"], json["password"] without defaults or error handling. ServiceType models at line 143 use List.from() without validating the list item types.

**Solution:** Add try-catch blocks to all fromJson methods. Use null-aware operators and default values for all JSON field access. Log parsing errors and return partial models rather than crashing. Consider using a code generation library like json_serializable for type-safe JSON parsing.

---

## 15. Data Model and Structure Issues

### 15.1 Status Field Is a Free-Form String with Inconsistent Values

**Problem:** The Status field on the BaseEntity class is a string with a maximum length of 10 characters. Different parts of the codebase compare against different values — "Active", "Delete", "Deleted", "Finished", "Pending", "Success". Some code uses .ToLower() == "Active" which will never match. There is no enum, no constants, and any typo in a status value becomes a silent bug.

**Solution:** Define an enum for entity status values. While the database column can remain a string for backward compatibility, map it through a property that parses the string into the enum. Create a migration to normalize inconsistent values.

---

### 15.2 Entity Models Used as ViewModels (15+ NotMapped Properties)

**Problem:** The Product model serves as both a database entity and a view model with more than 15 NotMapped properties. This violates Single Responsibility Principle.

**Solution:** Create separate ViewModel and DTO classes for each presentation context. Map between entities and DTOs explicitly or with AutoMapper.

---

### 15.3 MemberId Type Mismatch (Long vs Int)

**Problem:** The Member model defines MemberId as a long (64-bit integer), but throughout the code, it is consistently cast to int using Convert.ToInt32. This creates a truncation risk.

**Solution:** Either change the model to use int or update all code to use long consistently.

---

### 15.4 Missing Navigation Properties

**Problem:** Several important entity relationships are marked as NotMapped or absent, forcing manual queries. Product to ProductPrice, Product to ProductImage, and Product to ProductCategory all lack real EF navigation properties. Without them, Entity Framework cannot use Include or eager loading.

**Solution:** Add proper navigation properties with appropriate foreign key attributes. Configure relationships using Fluent API.

---

### 15.5 Misspelled Properties in Models

**Problem:** The Category model has "IsTrenbding" (should be "IsTrending"). The ServiceType model has "PersoneQuantity" (should be "PersonQuantity"). These misspellings propagate to database columns and all referencing code.

**Solution:** Create a migration to rename the columns and update all code references.

---

### 15.6 Product Price Silently Clamps Negative Values

**Problem:** The Product model's ProductPrice property uses Math.Max(0, value), silently converting negative prices to zero. A pricing bug could result in products being listed for free.

**Solution:** Throw a validation exception for negative prices rather than silently clamping.

---

### 15.7 OrderRequestService Does Not Inherit BaseEntity

**Problem:** OrderRequestService is the only entity without BaseEntity inheritance, lacking standard audit fields (CreateBy, CreateDate, Status, ModifiedBy, ModifiedDate).

**Solution:** Refactor to inherit from BaseEntity and add a migration for the missing columns.

---

### 15.8 GenericRepository Get Method Returns Null When No OrderBy

**Problem:** The GenericRepository's Get method initializes result as null. If the orderBy parameter is not provided, the method returns null silently, discarding all applied filters.

**Solution:** Fix the method to return the filtered query regardless of whether ordering is applied.

---

### 15.9 Mobile App Password Field in Login Response Model

**Problem:** The LoginResponseModel in the mobile app includes a "password" field that stores and serializes the user's password. The server API returns the password in the login response, and the mobile app stores it locally in unencrypted SharedPreferences.

**Solution:** The server should never return the password in API responses. Remove the password field from LoginResponseModel. If the server currently returns it, the mobile app should strip it before storage.

---

## 16. Mobile App Performance Issues

### 16.1 Missing const Constructors

**Problem:** Many widget files across the mobile app create widget instances without the const keyword where const construction is possible. Widgets like Divider(), SizedBox(), Icon(), Text() with constant values, and EdgeInsets() are created without const in build methods. This causes unnecessary widget rebuilds because Flutter cannot determine at compile time that the widget is immutable.

**Solution:** Add the const keyword to all widget constructors where all parameters are compile-time constants. Enable the prefer_const_constructors lint rule in analysis_options.yaml. The Flutter analyzer will automatically flag all locations where const can be added.

---

### 16.2 Heavy Widget Trees Without Performance Optimization

**Problem:** Several view files have deeply nested widget trees that rebuild entirely on state changes. Hotel booking views, typing module views, and home views contain 200+ line build methods with no extraction into smaller sub-widgets. Debug print statements (print(fcmToken)) are embedded inside build methods in hotel_booking_view.dart at lines 1167-1182.

**Solution:** Extract large build methods into smaller widget classes that can rebuild independently. Use const constructors for static parts of the UI. Remove all print statements from build methods. Use RepaintBoundary to isolate expensive painting operations.

---

### 16.3 No ListView.builder for Long Lists

**Problem:** Multiple views load entire lists into memory using Column(children: list.map(...).toList()) instead of ListView.builder. This means all widgets are created and laid out simultaneously, even if they are offscreen. For category lists with 100+ items or product lists with pagination, this creates significant jank.

**Solution:** Replace Column with ListView.builder (or SliverList with delegate) for all dynamic lists. ListView.builder creates widgets lazily as they scroll into view, using minimal memory regardless of list size.

---

### 16.4 Scroll Listener Memory Leaks

**Problem:** The home_controller.dart at line 265 adds a scroll listener with addListener() that is never removed. Every time the controller is recreated (page navigation), a new listener is added to the scroll controller without removing the previous one. Over many navigations, dozens of listeners accumulate, each executing on every scroll event.

**Solution:** Store a reference to the listener function and remove it in the onClose method using removeListener(). Alternatively, use GetX's ever() or once() methods for reactive scroll position tracking.

---

### 16.5 Expensive Operations in Build Methods

**Problem:** Helper functions called during widget building perform unnecessary allocations. The getTimeByFormat and getDateByFormat methods in helper.dart create new DateFormat instances on every call. A print(date) statement at line 70 executes during every build cycle.

**Solution:** Cache DateFormat instances as static final fields (they are reusable and thread-safe). Remove all print statements from helper methods. Avoid any allocation that is not strictly necessary during build.

---

### 16.6 Missing Obx Wrappers for Reactive State

**Problem:** Some views access GetX observable values using .value directly without wrapping the widget in an Obx() builder. This means the widget does not rebuild when the observable value changes. Examples include vehicle_list_view.dart at lines 98-101 and real_estate_home_view.dart at line 50.

**Solution:** Wrap all widgets that display reactive state in Obx() or use GetBuilder for non-reactive rebuilds. Ensure every .value or .obs access that should trigger UI updates is inside an Obx scope.

---

### 16.7 No Lazy Loading for Module Resources

**Problem:** All modules and their dependencies are loaded when the app starts. There is no lazy loading of feature modules. Services for typing, hotels, real estate, motors, and flowers are initialized regardless of whether the user accesses those features.

**Solution:** Use GetX's lazy binding (Get.lazyPut with fenix: true) for all module-specific controllers and services. Only initialize module resources when the user navigates to the module for the first time.

---

## 17. Mobile App UI/UX and Localization Issues

### 17.1 Wrong App Name in Notifications

**Problem:** The notification_service.dart at line 44 displays a permission dialog with the text "Mirsal needs notification permissions to work properly." The app is named "Boulevard", not "Mirsal", indicating copy-pasted code from a different project.

**Solution:** Replace "Mirsal" with "Boulevard" using the localized app name. Use AppLocalizations for all user-facing strings.

---

### 17.2 Hardcoded Contact Information

**Problem:** The phone number +971565545115 is hardcoded in at least 4 widget files across different modules (typing, motors, hotel, home). Hardcoded app store URLs appear in helper.dart (iOS: apps.apple.com, Android: play.google.com). The developer email emmacharwatson60@gmail.com appears in a code comment.

**Solution:** Move all contact information and app store URLs to a configuration file or Firebase Remote Config so they can be updated without releasing a new version.

---

### 17.3 No Empty State Messages

**Problem:** List views show nothing when their data source is empty. There is no "No results found", "No items available", or any other empty state indication. The user sees a blank screen with no way to know if data is loading, unavailable, or simply empty.

**Solution:** Implement empty state widgets for all list views. Show a relevant message and, where applicable, a call-to-action button (e.g., "Browse categories" when the favorites list is empty).

---

### 17.4 No Skeleton/Shimmer Loading States

**Problem:** The app uses BotToast.showLoading() which shows a global loading overlay for all API calls. This blocks the entire UI, preventing the user from navigating or performing any other action. The shimmer package is included in pubspec.yaml but is not used consistently across loading states.

**Solution:** Replace global loading overlays with shimmer/skeleton screens that show the structure of the expected content. This provides a better perceived performance and does not block user interaction. Keep the loading overlay only for destructive actions (submit order, process payment) where UI blocking is intentional.

---

### 17.5 Generic Error Messages

**Problem:** When API calls fail, the app shows generic messages like "Something went wrong" without distinguishing between network errors, server errors, validation errors, or authentication errors. The user has no way to know what went wrong or how to fix it.

**Solution:** Create specific error messages for each error category: "No internet connection — please check your Wi-Fi or mobile data", "Server is temporarily unavailable — please try again later", "Session expired — please log in again", and specific validation error messages from the server.

---

### 17.6 Mixed Localization Approach

**Problem:** The app supports English and Arabic through l10n.yaml and AppLocalizations, but the implementation is inconsistent. Some screens use AppLocalizations.of(context) for text, while others have hardcoded English strings. Medical module controller has hardcoded issue names and descriptions. Motors home view has hardcoded service descriptions. Helper.dart contains hardcoded English strings for sharing ("Check out this amazing service app").

**Solution:** Move all user-facing strings to the localization files. Run a full audit to find every hardcoded string and replace with the appropriate localization key. Add lint rules to flag hardcoded strings in Dart files.

---

### 17.7 No Responsive Design for Tablets

**Problem:** The app uses fixed widths with flutter_screenutil (e.g., 367.w) which adapts to phone screen sizes but does not account for tablets or landscape orientation. On tablets, the UI will either appear stretched or have excessive whitespace. The iOS Info.plist supports both portrait and landscape orientations for iPad but the UI is not designed for landscape.

**Solution:** Implement responsive breakpoints that switch between phone and tablet layouts. Use LayoutBuilder and MediaQuery to detect the available width and render appropriate layouts. For landscape orientation, either provide a landscape-optimized layout or restrict the app to portrait-only.

---

### 17.8 iOS Permission Descriptions Have Grammar Errors

**Problem:** The NSCameraUsageDescription says "We are used this library for profile picture change" (grammatically incorrect). The NSMicrophoneUsageDescription says "This application used this plugin for speech to text covert in search bar" (should be "convert", and grammar is incorrect). These descriptions are visible to users during permission prompts and appear unprofessional.

**Solution:** Fix all permission descriptions: NSCameraUsageDescription should read "Boulevard needs access to your camera to let you take a profile photo." NSMicrophoneUsageDescription should read "Boulevard needs microphone access to enable voice search." Keep descriptions clear, specific, and professionally written.

---

### 17.9 Flutter HTML Dependency Using "any" Version

**Problem:** The pubspec.yaml specifies flutter_html: any which accepts any version including major updates with breaking changes. This can cause the app to break after running flutter pub upgrade if a new major version of flutter_html is published with API changes.

**Solution:** Pin the flutter_html dependency to a specific version range (e.g., ^3.0.0). Never use "any" for dependencies in production applications. Run flutter pub outdated regularly to identify available updates and test them before adopting.

---

### 17.10 Zero Test Coverage

**Problem:** The test directory contains a single widget_test.dart file with one basic Flutter widget test. There are no unit tests for API services, controllers, models, or utility functions. There are no integration tests. There are no golden/screenshot tests. The entire business logic is untested, meaning any code change could introduce regressions without detection.

**Solution:** Write unit tests for all API service classes (mock HTTP responses), all controllers (test state transitions), all model fromJson methods (test with valid, invalid, and partial JSON), and all helper/utility functions. Write integration tests for critical flows (login, registration, checkout). Aim for at least 70% code coverage for business logic. Set up CI/CD to run tests automatically on every commit.

---

## 18. Template Compilation and Server Configuration

### 18.1 Debug Mode Enabled in Production Configuration

**Problem:** The Web.config file has compilation debug set to true. This causes: Razor views to be compiled individually instead of batch compiled, bundle minification to be disabled, additional debug metadata generated, and less aggressive caching of compiled assemblies.

**Solution:** Set compilation debug to false. Add optimizeCompilations set to true. Add batch set to true with appropriate batchTimeout. Enable precompilation by adding MvcBuildViews to the project file.

---

### 18.2 Missing Custom Error Pages

**Problem:** No custom error pages are configured in Web.config. ASP.NET default error pages expose internal paths, framework version, and stack traces to users.

**Solution:** Configure customErrors with user-friendly error pages while logging details server-side.

---

### 18.3 Missing HTTP Compression

**Problem:** Web.config has no urlCompression or httpCompression settings. All responses are sent uncompressed.

**Solution:** Enable static and dynamic compression in Web.config under system.webServer.

---

### 18.4 No HTTPS Enforcement or HSTS Headers

**Problem:** There are no SSL/TLS enforcement mechanisms in the server configuration. Combined with the mobile app's disabled certificate validation, this creates a complete absence of transport security across the entire system.

**Solution:** Enable HTTPS redirect in Web.config using URL rewrite rules. Add HSTS headers with max-age of at least one year. Add the RequireHttps attribute to all controllers. Configure the authentication cookie with the Secure flag.

---

## Summary of Issue Counts by Severity

| Severity | Count | Primary Categories |
|----------|-------|--------------------|
| Catastrophic | 3 | Complete absence of authentication (server), SSL certificate validation disabled (mobile), payment API key exposed (mobile) |
| Critical | 25 | IDOR, RCE via file upload, OTP bypass, exposed credentials (server + mobile × 7), SHA-256 without salt, stored XSS, N+1 queries (12 instances), missing indexes, no caching, no pagination, keystore password "123456", passwords in logs, unencrypted storage |
| High | 35 | GET-based destructive operations, missing CSRF, connection string injection, missing auth headers, broken role auth, broken exception handler, no timeouts, no retry, missing ProGuard, deep link hijacking, missing network security config, missing dispose methods (10+ controllers), no root detection, no code obfuscation |
| Medium | 50 | Status string inconsistency, dead code, magic numbers, response inconsistency, model misspellings, ToLower preventing indexes, synchronous DB calls, hardcoded URLs, mixed localization, missing pagination UI, no offline support, no token refresh, wrong app name, grammar errors in permissions, generic error messages, version mismatches, missing empty states |
| Low | 34+ | Public fields, naming conventions, debug mode, Swagger exposure, cookie configuration, test controllers, missing const constructors, heavy widget trees, TODO comments, zero tests, flutter_html: any, tab layout |

**Total Unique Issues: 147+**

---

## Recommended Fix Order

### Phase 0 — Critical Security Emergency (Immediate — Before Anything Else)

**Server Side:**
1. Register global authorization filters for both MVC and Web API. Add AllowAnonymous only on login, registration, and truly public endpoints.
2. Implement file upload validation with a whitelist of allowed extensions and magic byte verification.
3. Fix the OTP bypass by enforcing server-side OTP verification state.
4. Move all hardcoded credentials to configuration. Remove commented-out connection strings with credentials.
5. Implement proper CSRF protection across all admin forms.
6. Replace SHA-256 password hashing with bcrypt.
7. Fix the broken IsInRole method in CustomPrincipal.
8. Implement proper HTTPS enforcement and HSTS headers.

**Mobile App (SAME DAY):**
9. DELETE the MyHttpOverrides class that disables SSL certificate validation.
10. REVOKE the MyFatoorah payment API key immediately and generate a new one. Move payment processing to the server side.
11. Remove all API keys from source code — restrict Firebase keys in Google Cloud Console, restrict Google Maps keys by platform.
12. Change the Android keystore password from "123456" to a strong password. Remove key.properties from source control.
13. Replace SharedPreferences with flutter_secure_storage for all sensitive data.
14. Remove the password field from LoginResponseModel.
15. Remove all print() statements that expose sensitive data.
16. Add Authorization Bearer headers to all API requests.
17. Fix the iOS aps-environment to "production" for release builds.

### Phase 1 — Database and Caching Foundation (Week 1-2)

1. Create all missing database indexes on Products, ProductCategories, ProductPrices, Brands, ServiceTypes, and StockLogs.
2. Introduce MemoryCache helper on the server and cache layout settings, reference data, and dashboard statistics.
3. Implement client-side caching in the mobile app for reference data (countries, cities, categories).
4. Set debug to false with optimizeCompilations enabled.
5. Enable HTTP compression in Web.config.
6. Add Cache-Control headers to static data API endpoints.
7. Create proguard-rules.pro for Android with proper keep rules.
8. Configure network_security_config.xml for Android.

### Phase 2 — Dashboard and Query Optimization (Week 2-3)

1. Consolidate dashboard queries using GROUP BY and Task.WhenAll.
2. Fix all 12 N+1 query patterns by pre-loading related data in batches.
3. Convert chart data endpoints to use SQL aggregation instead of in-memory processing.
4. Remove ToLower calls from LINQ queries.
5. Replace synchronous database calls with async counterparts.
6. Fix the GenericRepository Get method null return bug.

### Phase 3 — API Layer and Mobile App Architecture (Week 3-4)

1. Standardize API response format across all server controllers.
2. Fix the third-party login bug (wrong variable returned).
3. Fix the duplicate condition check in order validation.
4. Implement HTTP interceptors in the mobile app with proper auth, retry, and timeout handling.
5. Implement comprehensive status code handling (401, 400, 429, 500).
6. Add request timeouts to all mobile app API calls.
7. Implement token refresh flow.
8. Add root/jailbreak detection.
9. Implement Flutter code obfuscation (--obfuscate flag).

### Phase 4 — Frontend, Pagination, and Performance (Week 4-6)

1. Bundle and minify CSS and JS files in admin panel. Remove duplicate jQuery loads.
2. Load page-specific libraries only where needed.
3. Implement server-side DataTables processing.
4. Add server-side pagination to all listing endpoints. Fix PaginatedList class.
5. Apply image compression and thumbnail generation on the server.
6. Fix all missing dispose/onClose methods in mobile app controllers (10+ controllers).
7. Add const constructors to all eligible widgets.
8. Replace Column-based lists with ListView.builder.
9. Remove scroll listener memory leaks.
10. Implement shimmer loading states in the mobile app.

### Phase 5 — Architecture Refactoring (Week 6-8)

**Server Side:**
1. Introduce a dependency injection container. Register DbContext with per-request lifetime.
2. Fix IUnitOfWork to include IDisposable and SaveChanges.
3. Remove SaveChanges from individual repository methods.
4. Separate entity models from view models.
5. Eliminate all standalone DbContext creations.
6. Convert Status strings to enums.
7. Remove dead code and test controllers.
8. Break down god classes into focused services.

**Mobile App:**
9. Implement clean architecture (View → Controller → Repository → DataSource).
10. Register all services in AppDependencies with proper DI.
11. Extract social auth handlers into separate classes.
12. Standardize on named routes.
13. Fix all hardcoded strings with proper localization.
14. Fix grammar errors in iOS permission descriptions.
15. Fix the wrong app name ("Mirsal") in notification service.
16. Write unit tests (target: 70% coverage for business logic).
17. Configure Dart code obfuscation for release builds.
18. Implement crash reporting with Firebase Crashlytics.

---

## Cross-System Vulnerability Chain Analysis

The Boulevard system has a compounding vulnerability chain where weaknesses in one component amplify weaknesses in others:

**Chain 1 — Complete Authentication Bypass:** The server has no authentication on any endpoint (Section 1.1). The mobile app sends no authentication headers (Section 2.8). The mobile app disables SSL/TLS validation (Section 2.1). Combined, this means any attacker on the same network can intercept, read, and modify all traffic between the app and server, and can call any server endpoint directly without any credentials.

**Chain 2 — Payment Fraud:** The payment API key is hardcoded in the mobile app (Section 2.2). The app has no code obfuscation (Section 2.13). The Android keystore password is "123456" (Section 3.1). Combined, an attacker can decompile the app, extract the payment key, and process unauthorized transactions.

**Chain 3 — Account Takeover:** The OTP can be bypassed on the server (Section 1.5). Passwords are hashed without salt (Section 1.6). The OTP value is returned in the API response (Section 1.5). The mobile app stores passwords in cleartext (Section 2.9). Combined, an attacker can reset any user's password, extract stored credentials from compromised devices, and take over any account in the system.

**Chain 4 — Data Exfiltration:** No authorization on any endpoint (Section 1.1) + IDOR on all entity operations (Section 1.2) + no rate limiting + debug logs exposing data (Section 2.10). Combined, an attacker can enumerate and download all user data, order histories, and personal information from the system with no barriers.

---

*This comprehensive unified report was compiled after exhaustive analysis of the complete Boulevard system across all three platforms — ASP.NET MVC 5 Admin Panel (37 controllers, 70+ services, 276 views, all models, all configuration files), Flutter Mobile Application (55+ Dart files across 8 modules, all controllers, all API services, all models, all views), Android build configuration (build.gradle, AndroidManifest.xml, key.properties, gradle.properties), iOS build configuration (Info.plist, Podfile, Runner.entitlements), SQL Server Database (75 tables, all indexes, all stored procedures), and all supporting infrastructure files including Firebase, analysis options, and localization configuration. Total files inspected: 500+.*
