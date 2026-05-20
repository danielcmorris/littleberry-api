# Skipped/Redundant Endpoints

This document lists endpoints from the original PFSA API that were **not migrated** to LittleberryApi, along with the reasons.

## Skipped Controllers

### 1. ValuesController
**Reason:** Sample/template controller from default Web API project
- `GET /api/values` - Returns sample values
- `GET /api/values/{id}` - Returns single value
- `POST /api/values` - Creates value (empty stub)
- `PUT /api/values/{id}` - Updates value (empty stub)
- `DELETE /api/values/{id}` - Deletes value (empty stub)

### 2. _AccountController (Identity/OAuth Controller)
**Reason:** Uses ASP.NET Identity with OWIN which is incompatible with .NET 8. Auth0 handles authentication instead.
- `GET /UserInfo` - Get user info
- `POST /Logout` - Logout
- `GET /ManageInfo` - Get manage info
- `POST /ChangePassword` - Change password
- `POST /SetPassword` - Set password
- `POST /AddExternalLogin` - Add external login
- `POST /RemoveLogin` - Remove login
- `GET /ExternalLogin` - Get external login
- `GET /ExternalLogins` - Get external logins
- `POST /Register` - Register user
- `POST /RegisterExternal` - Register external user

**Note:** If password management is needed, implement custom endpoints or use Auth0's password reset flow.

### 3. HomeController (MVC Controller)
**Reason:** MVC view controller, not an API endpoint
- `GET /` - Index view
- This is for serving HTML views, not relevant for API migration

### 4. HelpController (API Documentation)
**Reason:** Auto-generated API help pages from ASP.NET Web API
- `GET /Help` - API documentation index
- `GET /Help/Api/{apiId}` - Individual API documentation
- `GET /Help/ResourceModel/{modelName}` - Resource model documentation

**Note:** Swagger/OpenAPI is now used instead (available at `/swagger`)

## Skipped External Integration

### DocuSign Integration
**Reason:** Per user request, DocuSign integration was explicitly skipped
- Any endpoints or services related to DocuSign e-signature were not migrated

## Stub Methods (Empty Implementations)

The following methods existed as empty stubs in the original API and remain as stubs:

| Controller | Method | Route | Notes |
|------------|--------|-------|-------|
| CatalogController | DELETE | `/api/catalog/{id}` | Not implemented |
| SubjectController | DELETE | `/api/subject/{id}` | Not implemented |
| AccountController | DELETE | `/api/account/{id}` | Not implemented |
| FestaRequestController | GET | `/api/festarequest` | Returns empty |
| FestaRequestController | GET | `/api/festarequest/{id}` | Returns empty |
| FestaRequestController | PUT | `/api/festarequest/{id}` | Returns empty |
| FestaRequestController | DELETE | `/api/festarequest/{id}` | Returns empty |
| SimpleMailController | GET | `/api/simplemail` | Returns empty |
| SimpleMailController | GET | `/api/simplemail/{id}` | Returns empty |
| SimpleMailController | PUT | `/api/simplemail/{id}` | Returns empty |
| SimpleMailController | DELETE | `/api/simplemail/{id}` | Returns empty |

## CatalogSubject Route

The original API had a `CatalogSubject` controller referenced in routing but the actual controller file was not found. The functionality appears to be handled by `CatalogController` with the `/library/bysubject` route.

---

## Summary

| Category | Count |
|----------|-------|
| Controllers fully skipped | 4 |
| Endpoints skipped (Identity) | 11 |
| Stub methods retained | 10 |
| **Total skipped endpoints** | ~25 |
| **Total migrated endpoints** | ~55-60 |
