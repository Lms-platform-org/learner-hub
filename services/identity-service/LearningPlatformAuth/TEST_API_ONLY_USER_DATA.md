# API-Only User Data Flow - Test Guide

## Test Scenarios

### Scenario 1: User Registration

**Step 1: Start the API**
```powershell
cd LearningPlatformAuth
dotnet run
```
✅ API running on `https://localhost:7001`

**Step 2: Register via API (using Postman or curl)**
```bash
POST https://localhost:7001/api/auth/register
Content-Type: application/json

{
  "email": "student1@test.com",
  "password": "Password123!",
  "role": "Student",
  "displayName": "Student One"
}
```

**Expected Response:**
```json
{
  "success": true,
  "message": "You can login now",
  "data": {
    "email": "student1@test.com",
    "userId": "uuid-here",
    "name": "Student One",
    "role": "Student"
  }
}
```

**Verify in API Database:**
```powershell
# Connect to SQL Server
SELECT * FROM AspNetUsers WHERE Email = 'student1@test.com'
```
✅ User should exist in API database

---

### Scenario 2: User Login

**Request:**
```bash
POST https://localhost:7001/api/auth/login
Content-Type: application/json

{
  "email": "student1@test.com",
  "password": "Password123!"
}
```

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "uuid-here",
  "email": "student1@test.com",
  "role": "Student",
  "displayName": "Student One",
  "isApproved": true
}
```

**Store Token in Razor Pages Session:**
```csharp
HttpContext.Session.SetString("AuthToken", response.Data.Token);
HttpContext.Session.SetString("UserId", response.Data.UserId);
```

---

### Scenario 3: Get User Details via API

**From Razor Pages Page Model:**

```csharp
public class UserDetailModel : PageModel
{
    private readonly IAuthApiClient _authApiClient;
    public UserDetailsDto? User { get; set; }

    public UserDetailModel(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        var response = await _authApiClient.GetUserByIdAsync(userId, token);

        if (!response.Success)
            return RedirectToPage("/Login");

        User = response.Data;
        return Page();
    }
}
```

**API Request (behind the scenes):**
```
GET https://localhost:7001/api/auth/user/{userId}
Authorization: Bearer {token}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "userId": "uuid-here",
    "email": "student1@test.com",
    "displayName": "Student One",
    "roles": ["Student"],
    "isApproved": true,
    "approvalDate": null
  }
}
```

---

### Scenario 4: Admin Gets All Teachers

**From Admin Page:**

```csharp
public class AdminTeachersModel : PageModel
{
    private readonly IAuthApiClient _authApiClient;
    public List<UserDetailsDto> Teachers { get; set; } = new();

    public AdminTeachersModel(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var adminToken = HttpContext.Session.GetString("AuthToken");

        if (string.IsNullOrEmpty(adminToken))
            return RedirectToPage("/Login");

        var response = await _authApiClient.GetUsersByRoleAsync("Teacher", adminToken);

        if (response.Success && response.Data != null)
        {
            Teachers = response.Data;
        }

        return Page();
    }
}
```

**API Request:**
```
GET https://localhost:7001/api/auth/users-by-role/Teacher
Authorization: Bearer {admin-token}
```

**Expected Response:**
```json
{
  "success": true,
  "count": 2,
  "data": [
    {
      "userId": "uuid-1",
      "email": "teacher1@test.com",
      "displayName": "Teacher One",
      "roles": ["Teacher"],
      "isApproved": true,
      "approvalDate": "2024-01-15T10:30:00Z"
    },
    {
      "userId": "uuid-2",
      "email": "teacher2@test.com",
      "displayName": "Teacher Two",
      "roles": ["Teacher"],
      "isApproved": false,
      "approvalDate": null
    }
  ]
}
```

---

### Scenario 5: Verify No Local User Storage

**Check Razor Pages Database:**

```powershell
# Connect to LearningPlatform database
SELECT * FROM AspNetUsers
```

❌ **Expected:** No results (or old empty tables)

```powershell
# Connect to LearningPlatformAuth database
SELECT * FROM AspNetUsers
```

✅ **Expected:** All users here

---

## Test Checklist

### Registration Flow
- [ ] Can register via API
- [ ] User created in API database
- [ ] User NOT in Razor Pages database
- [ ] Receives authentication token
- [ ] Token valid and contains correct claims

### Authentication Flow
- [ ] Can login via API
- [ ] Receives valid JWT token
- [ ] Token stored in session
- [ ] Token expires correctly

### User Data Retrieval
- [ ] Can fetch own user details with token
- [ ] Admin can fetch any user by ID
- [ ] Admin can fetch users by role
- [ ] Admin can fetch approved users
- [ ] Cannot access without valid token
- [ ] Cannot access with invalid token

### Authorization
- [ ] Non-admin cannot access admin endpoints
- [ ] Teachers pending approval cannot login
- [ ] Approved teachers can login
- [ ] Students can login immediately

### Data Consistency
- [ ] User updated in API reflects in all requests
- [ ] No stale data from local cache
- [ ] All changes persist to API database

---

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized on API call | Token missing/expired | Check session, re-login |
| 403 Forbidden | User lacks permission | Check user role in API DB |
| User not found | Wrong user ID | Verify UUID format |
| CORS error | API URL mismatch | Check appsettings.json |
| No data returned | API query failed | Check API logs |
| Null reference exception | Token not set | Verify session before calling API |

---

## Postman Collection Template

```json
{
  "info": {
    "name": "LearningPlatform API Tests",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Register User",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\": \"test@test.com\",\n  \"password\": \"Password123!\",\n  \"role\": \"Student\",\n  \"displayName\": \"Test User\"\n}"
        },
        "url": {
          "raw": "https://localhost:7001/api/auth/register",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "register"]
        }
      }
    },
    {
      "name": "Login",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"email\": \"test@test.com\",\n  \"password\": \"Password123!\"\n}"
        },
        "url": {
          "raw": "https://localhost:7001/api/auth/login",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "login"]
        }
      }
    },
    {
      "name": "Get User by ID",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{token}}"
          }
        ],
        "url": {
          "raw": "https://localhost:7001/api/auth/user/{{userId}}",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "user", "{{userId}}"]
        }
      }
    },
    {
      "name": "Get Users by Role",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{adminToken}}"
          }
        ],
        "url": {
          "raw": "https://localhost:7001/api/auth/users-by-role/Teacher",
          "protocol": "https",
          "host": ["localhost"],
          "port": "7001",
          "path": ["api", "auth", "users-by-role", "Teacher"]
        }
      }
    }
  ],
  "variable": [
    {
      "key": "token",
      "value": ""
    },
    {
      "key": "userId",
      "value": ""
    },
    {
      "key": "adminToken",
      "value": ""
    }
  ]
}
```

---

## Final Verification

After all tests pass:

```powershell
# 1. Verify API database has all users
SELECT COUNT(*) FROM AspNetUsers;  -- Should have registered users

# 2. Verify Razor Pages DB has NO users
USE LearningPlatform;
SELECT COUNT(*) FROM AspNetUsers;  -- Should be 0 or empty

# 3. Build both projects
dotnet build  # From solution root

# 4. Run tests
dotnet test   # If you have unit tests

# 5. Check logs
Get-Content logs/authentication-*.txt | tail -50
```

---

## Success Criteria

✅ Users registered only in API database
✅ Razor Pages queries all user data from API
✅ No user data in Razor Pages local database
✅ All API endpoints respond correctly
✅ Tokens work for authentication
✅ Admin can query all user roles
✅ Build succeeds for both projects
✅ No compilation warnings or errors

**You now have API-centric user data architecture!** 🎉
