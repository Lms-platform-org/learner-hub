# Checklist: Remove Local User Storage from LearningPlatform

## Why?
The Razor Pages app should ONLY use the API database for user data. This document helps you identify and remove any local user storage.

---

## Step 1: Review Current Database Schema

**File to check:** `LearningPlatform\Data\ApplicationDbContext.cs`

```csharp
// ❌ DON'T HAVE THESE:
dbModelBuilder.Entity<User>();
dbModelBuilder.Entity<ApplicationUser>();
dbModelBuilder.Entity<AspNetUser>();

// ✅ OK TO HAVE:
dbModelBuilder.Entity<Course>();
dbModelBuilder.Entity<Assignment>();
// Other domain-specific entities (not user-related)
```

**Action:** If you find user-related entities:
1. Remove them from `OnModelCreating()`
2. Delete the migration files (if they were just added)
3. Create a new migration to drop the user tables

---

## Step 2: Remove User Identity Models

**Files to check:**
- `LearningPlatform\Models\User.cs`
- `LearningPlatform\Data\ApplicationDbContext.cs` (User configurations)

**Action:**
```csharp
// ❌ REMOVE if present:
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    // ... other user properties
}

// Use UserDetailsDto instead for API responses
public class UserDetailsDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    // ... (already created in Models/Dtos.cs)
}
```

---

## Step 3: Update Identity Configuration

**File:** `LearningPlatform\Program.cs`

**Current State (DON'T USE THIS ANYMORE):**
```csharp
❌ builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
```

**Why Remove It?**
- Local Identity is no longer needed
- All auth happens via API
- The API handles user storage

**Action:**
You can either:
1. Keep it (it won't hurt if not used)
2. Remove it and the related NuGet packages

---

## Step 4: Update Razor Pages

**Files to check:** Any `.cshtml.cs` file that reads/writes user data

**Pattern to Find & Replace:**

### ❌ OLD (Don't Do This)
```csharp
public class ProfileModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        // Access local database
    }
}
```

### ✅ NEW (Do This)
```csharp
public class ProfileModel : PageModel
{
    private readonly IAuthApiClient _apiClient;

    public UserDetailsDto? CurrentUser { get; set; }

    public async Task OnGetAsync()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        var userId = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Login");

        var response = await _apiClient.GetUserByIdAsync(userId, token);

        if (response.Success)
            CurrentUser = response.Data;
    }
}
```

---

## Step 5: Update Register/Login Pages

**Files to check:**
- `Areas\Identity\Pages\Account\Register.cshtml.cs`
- `Areas\Identity\Pages\Account\Login.cshtml.cs`

**Current Pattern (if using ASP.NET Identity):**
```csharp
❌ var result = await _userManager.CreateAsync(user, password);
❌ var signInResult = await _signInManager.PasswordSignInAsync(...);
```

**New Pattern (using API):**
```csharp
✅ var response = await _authApiClient.RegisterAsync(registerRequest);
✅ var response = await _authApiClient.LoginAsync(loginRequest);
```

Example Register Page:
```csharp
public class RegisterModel : PageModel
{
    private readonly IAuthApiClient _authApiClient;

    public async Task<IActionResult> OnPostAsync(string email, string password, string role)
    {
        var request = new RegisterRequest 
        { 
            Email = email, 
            Password = password, 
            Role = role 
        };

        var response = await _authApiClient.RegisterAsync(request);

        if (response.Success)
        {
            // Store token
            HttpContext.Session.SetString("AuthToken", response.Data.Token);
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("", response.Message);
        return Page();
    }
}
```

---

## Step 6: Check Database Migrations

**File:** `LearningPlatform\Data\Migrations\`

**Action:**
1. Look for any migrations that create user tables
2. If they exist and are old, you can ignore them
3. If they're new (from your recent changes), consider removing them:

```powershell
# Remove last migration if it added user tables
dotnet ef migrations remove

# Apply migrations
dotnet ef database update
```

---

## Step 7: Clean Up NuGet Packages (Optional)

**If you're NOT using ASP.NET Identity locally:**

```powershell
# Remove these if not needed:
dotnet remove package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet remove package Microsoft.AspNetCore.Identity.UI
```

**But Keep:**
- `System.Net.Http.Json` (for API calls)
- `AutoMapper` (if used)
- `Serilog` (for logging)

---

## Step 8: Verify No Local User Queries

**Search for these patterns** in your codebase:

```csharp
❌ _userManager.FindByIdAsync()
❌ _userManager.FindByEmailAsync()
❌ _userManager.GetUserAsync()
❌ _userManager.CreateAsync()
❌ _userManager.UpdateAsync()
❌ _userManager.DeleteAsync()
❌ _signInManager.PasswordSignInAsync()

// If you find any, replace with API calls
✅ _authApiClient.GetUserByIdAsync()
✅ _authApiClient.GetUserByEmailAsync()
✅ _authApiClient.RegisterAsync()
✅ _authApiClient.LoginAsync()
```

---

## Step 9: Update Data Access Layer

**If you have a User repository or service:**

```csharp
❌ IUserRepository (local database)
❌ UserService (queries local DB)

✅ IAuthApiClient (queries API database)
```

---

## Step 10: Test Everything

```powershell
# 1. Build both projects
dotnet build

# 2. Start API
cd LearningPlatformAuth
dotnet run

# 3. Start Razor Pages (in another terminal)
cd ..\..\ProjectLP\LearningPlatformAuth\LearningPlatform\LearningPlatform
dotnet run

# 4. Test registration/login
# - Register a new user
# - Verify it appears in API database
# - Verify it DOES NOT appear in Razor Pages database
# - Login and verify profile loads from API
```

---

## Final Checklist

- [ ] Removed User entities from ApplicationDbContext
- [ ] Updated Razor Pages to use IAuthApiClient
- [ ] Updated Register/Login pages to use API
- [ ] Removed or disabled ASP.NET Identity if not needed
- [ ] No local user queries remain
- [ ] Build is successful
- [ ] Both projects run without errors
- [ ] User data is stored ONLY in API database
- [ ] Razor Pages queries all user data from API

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Cannot find User table" | This is OK! It means local user storage is removed |
| "UserManager not found" | Remove the DI or use IAuthApiClient instead |
| "Token expired" | Refresh token or redirect to login |
| "Unauthorized on API call" | Verify token is set in Authorization header |
| "CORS error" | Check API CORS policy includes Razor Pages URL |

---

## Database Query Flow

```
Razor Pages
    ↓
IAuthApiClient
    ↓
HttpClient (POST /api/auth/user/{id})
    ↓
LearningPlatformAuth API
    ↓
AuthService
    ↓
AuthRepository
    ↓
Entity Framework
    ↓
SQL Server (ApplicationDbContext)
    ↓
User Data
```

**Every user query must go through this chain!**

---

See `API_CENTRIC_ARCHITECTURE.md` for more details on using the API.
