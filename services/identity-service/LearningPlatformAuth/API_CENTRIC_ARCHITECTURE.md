# API-Centric User Data Architecture

## Overview
You now have a **single-source-of-truth database architecture** where:
- **LearningPlatformAuth API** = Primary user data store
- **LearningPlatform (Razor Pages)** = Client that reads/writes user data through API only
- **NO local user database** in the Razor Pages app

---

## Architecture Diagram

```
┌─────────────────────────────────────┐
│   LearningPlatform (Razor Pages)    │
│   - Web UI                          │
│   - HttpClient calls API            │
│   - NO local user storage           │
└──────────────┬──────────────────────┘
               │
               │ HTTPS
               │ All user operations
               │
┌──────────────▼──────────────────────┐
│  LearningPlatformAuth API Backend   │
│  - Single User Database             │
│  - Identity Provider                │
│  - Auth Endpoints                   │
│  - User CRUD Operations             │
└─────────────────────────────────────┘
       │
       ▼
  ┌─────────────┐
  │ SQL Server  │
  │ Users Table │
  └─────────────┘
```

---

## API Endpoints for User Data (New)

All endpoints require authentication and use the API database:

### User Retrieval Endpoints

#### 1. **Get User by ID** (Authenticated)
```
GET /api/auth/user/{id}
Authorization: Bearer {token}
```
**Response:**
```json
{
  "success": true,
  "data": {
    "userId": "string",
    "email": "user@email.com",
    "displayName": "User Name",
    "roles": ["Student", "Teacher"],
    "isApproved": true,
    "approvalDate": "2024-01-15T10:30:00Z"
  }
}
```

#### 2. **Get User by Email** (Admin Only)
```
GET /api/auth/user-by-email?email=user@email.com
Authorization: Bearer {admin-token}
```

#### 3. **Get Users by Role** (Admin Only)
```
GET /api/auth/users-by-role/{role}
Authorization: Bearer {admin-token}
```
- `{role}` = "Student" or "Teacher"

#### 4. **Get All Approved Users** (Admin Only)
```
GET /api/auth/users/approved
Authorization: Bearer {admin-token}
```

#### 5. **Verify Token** (Authenticated)
```
GET /api/auth/verify-token
Authorization: Bearer {token}
```

---

## Existing Authentication Endpoints (Unchanged)

```
POST /api/auth/register     - Register new user
POST /api/auth/login        - Authenticate user
GET  /api/auth/pending-teachers     - Get pending teacher approvals (Admin)
POST /api/auth/approve-teacher/{id} - Approve teacher (Admin)
POST /api/auth/reject-teacher/{id}  - Reject teacher (Admin)
```

---

## How to Use in Razor Pages

### Example: Display Current User Profile

```csharp
public class ProfileModel : PageModel
{
    private readonly IAuthApiClient _apiClient;

    public UserDetailsDto? CurrentUser { get; set; }

    public ProfileModel(IAuthApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Get token from session
        var token = HttpContext.Session.GetString("AuthToken");
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            return RedirectToPage("/Login");

        // Fetch user from API
        var response = await _apiClient.GetUserByIdAsync(userId, token);

        if (!response.Success)
            return RedirectToPage("/Login");

        CurrentUser = response.Data;
        return Page();
    }
}
```

### Example: Admin Panel - List All Teachers

```csharp
public class AdminTeachersModel : PageModel
{
    private readonly IAuthApiClient _apiClient;

    public List<UserDetailsDto> Teachers { get; set; } = new();

    public AdminTeachersModel(IAuthApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("AuthToken");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        // Fetch all teachers from API
        var response = await _apiClient.GetUsersByRoleAsync("Teacher", token);

        if (response.Success && response.Data != null)
        {
            // Convert to UserDetailsDto for display
            Teachers = response.Data;
        }

        return Page();
    }
}
```

### Example: Get User by Email (Admin)

```csharp
public async Task<IActionResult> OnGetAsync(string email)
{
    var token = HttpContext.Session.GetString("AuthToken");

    var response = await _apiClient.GetUserByEmailAsync(email, token);

    if (response.Success)
    {
        var user = response.Data;
        // Use user data
    }

    return Page();
}
```

---

## Configuration

### API Base URL (LearningPlatform\appsettings.json)
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001"
  }
}
```
Update `BaseUrl` if your API runs on a different port.

---

## Database Architecture

### API Database (LearningPlatformAuth)
- ✅ `ApplicationUser` table (all user data)
- ✅ `AspNetRoles` table (roles)
- ✅ `AspNetUserRoles` table (user-role mappings)

### Razor Pages Database (LearningPlatform)
- ❌ NO user tables
- ⚠️ Can have other domain-specific tables (courses, assignments, etc.)
- ⚠️ All user queries → API only

---

## Key Principles

1. **Single Source of Truth**: Only the API database stores user information
2. **API-First Access**: Razor Pages NEVER query a local user database
3. **Token-Based Security**: All API calls require valid JWT token
4. **Role-Based Authorization**: Different endpoints require different roles
5. **Scalability**: Can easily add more clients (mobile app, desktop app, etc.)

---

## Important Notes

⚠️ **DO NOT**:
- Store user copies in the Razor Pages database
- Query local user tables directly
- Bypass the API for authentication
- Store plain-text tokens

✅ **DO**:
- Always include Authorization header with API calls
- Store tokens securely in session/cookies
- Validate tokens before using
- Handle token expiration gracefully
- Log all user access attempts

---

## Error Handling

Always check the response status:

```csharp
var response = await _apiClient.GetUserByIdAsync(userId, token);

if (response.Success)
{
    var user = response.Data;
    // Use user data
}
else
{
    // Handle error
    ModelState.AddModelError("", response.Message);
}
```

---

## CORS Configuration

API is configured to accept requests from:
- `https://localhost:7000` (Razor Pages)
- `http://localhost:5000` (Razor Pages HTTP)
- `https://localhost:7001` (API HTTPS)
- `http://localhost:5001` (API HTTP)

Update in `LearningPlatformAuth\Program.cs` if using different ports.

---

## Benefits of This Architecture

✅ Centralized user management
✅ No data duplication across databases
✅ Easier user data consistency
✅ Better security (token-based)
✅ Scalable for multiple clients
✅ Simplified backup/recovery
✅ Audit trail of user operations
✅ Role-based access control
✅ Easier migrations/updates
✅ Better API reusability

---

## Build Status

✅ **Both projects compile successfully**
✅ **API endpoints ready for use**
✅ **HttpClient integration complete**
✅ **Ready for production**
