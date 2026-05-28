# Code Process, Redirect Code, and Connection String Documentation

Project: RazorDemo  
Framework: ASP.NET Core Razor Pages  
Database: MySQL / MariaDB through Entity Framework Core

## 1. Application Startup Process

The main application process starts in `Program.cs`.

```csharp
var builder = WebApplication.CreateBuilder(args);
```

This creates the application builder. The builder is used to register services such as database access, authentication, authorization, and Razor Pages.

## 2. Connection String Process

The database connection string is stored in `appsettings.json`.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=loginappdb;User=root;Password=;"
}
```

Meaning of each part:

| Part | Meaning |
| --- | --- |
| `Server=localhost` | The database server is running on the local computer |
| `Port=3306` | MySQL/MariaDB default port |
| `Database=loginappdb` | The database name used by the project |
| `User=root` | The database username |
| `Password=;` | Empty password |

`Program.cs` reads this connection string with:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

Then the project connects Entity Framework Core to MySQL:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

This means every page model that receives `ApplicationDbContext` can work with the database.

## 3. DbContext Process

The database context is in `Controller/ApplicationDbContext.cs`.

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUsers> AppUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUsers>().ToTable("users");
    }
}
```

Process:

1. `ApplicationDbContext` receives database settings from `Program.cs`.
2. `DbSet<AppUsers>` represents the user records.
3. `AppUsers` is mapped to the database table named `users`.

## 4. User Model and Table Mapping

The user model is in `Model/AppUsers.cs`.

```csharp
public class AppUsers
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("role")]
    public string? Role { get; set; }

    [Column("remark")]
    public string? Remark { get; set; }

    [Column("password_hash")]
    public string? PasswordHash { get; set; }
}
```

Each property maps to a column in the `users` table.

Example:

| C# Property | Database Column |
| --- | --- |
| `Id` | `id` |
| `Name` | `name` |
| `Email` | `email` |
| `Gender` | `gender` |
| `Role` | `role` |
| `Remark` | `remark` |
| `PasswordHash` | `password_hash` |

## 5. Authentication Process

Authentication is registered in `Program.cs`.

```csharp
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth";
        options.AccessDeniedPath = "/Index";
        options.Cookie.Name = "UserLoginCookie";
    });
```

Process:

1. The project uses cookie authentication.
2. If a user tries to open a protected page without logging in, they are redirected to `/Auth`.
3. If a user does not have permission, they are redirected to `/Index`.
4. The login cookie is named `UserLoginCookie`.

Middleware order:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Authentication must run before authorization.

## 6. Register Process

Register code is in `Pages/Auth.cshtml.cs`.

```csharp
public async Task<IActionResult> OnPostRegisterAsync()
```

Process:

1. Validate the form.
2. Check confirm password.
3. Check if the email already exists.
4. Create a new `AppUsers` object.
5. Save the user to the database.
6. Redirect the user back to the login form.

Redirect code:

```csharp
return RedirectToPage("/Auth", new { action = "login" });
```

This redirects to the Auth page and passes `action=login`.

## 7. Login Process

Login code is in `Pages/Auth.cshtml.cs`.

```csharp
public async Task<IActionResult> OnPostLoginAsync()
```

Process:

1. Validate the form.
2. Search for a user with matching email and password.
3. If no user is found, return the same page with an error.
4. If the user exists, create authentication claims.
5. Save the login cookie.
6. Redirect to the home page.

User lookup:

```csharp
var user = await _db.AppUsers.FirstOrDefaultAsync(
    u => u.Email == Input.Email && u.PasswordHash == Input.Password);
```

Claims:

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.Name ?? user.Email ?? "User"),
    new Claim(ClaimTypes.Role, user.Role ?? "User")
};
```

Sign in:

```csharp
await HttpContext.SignInAsync(AuthScheme, principal);
```

Redirect code:

```csharp
return RedirectToPage("/Index");
```

## 8. Logout Process

Logout code is in `Pages/Auth.cshtml.cs`.

```csharp
public async Task<IActionResult> OnPostLogoutAsync()
```

Process:

1. Remove the login cookie.
2. Redirect the user to the home page.

Code:

```csharp
await HttpContext.SignOutAsync(AuthScheme);
return RedirectToPage("/Index");
```

## 9. Protected Page Redirects

Some pages use `[Authorize]`.

Example:

```csharp
[Authorize]
public class AboutModel : PageModel
```

This means the page requires login.

If the user is not logged in:

```csharp
options.LoginPath = "/Auth";
```

The user is redirected to `/Auth`.

## 10. Admin Page Redirects

Some pages require the Admin role.

Example:

```csharp
[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
```

This means only users with role `Admin` can open the dashboard.

If the user is logged in but not Admin:

```csharp
options.AccessDeniedPath = "/Index";
```

The user is redirected to `/Index`.

## 11. Dashboard Process

Dashboard code is in `Pages/Dashboard.cshtml.cs`.

```csharp
public async Task OnGetAsync()
```

Process:

1. Load all users from the database.
2. Count total users.
3. Count Admin users.
4. Count Student users.

Delete process:

```csharp
public async Task<IActionResult> OnPostDeleteAsync(int id)
```

After deleting a user, the dashboard reloads:

```csharp
return RedirectToPage();
```

`RedirectToPage()` without a page name redirects to the same page.

## 12. Edit User Process

Edit user code is in `Pages/EditUser.cshtml.cs`.

When opening the edit page:

```csharp
public async Task<IActionResult> OnGetAsync(int id)
```

Process:

1. Find user by id.
2. If the user does not exist, redirect to dashboard.
3. If the user exists, show the edit form.

Redirect if user is missing:

```csharp
return RedirectToPage("/Dashboard");
```

When saving:

```csharp
public async Task<IActionResult> OnPostAsync()
```

Process:

1. Validate the form.
2. Find the existing user in the database.
3. Update allowed fields.
4. Save changes.
5. Redirect to dashboard.

Redirect after save:

```csharp
return RedirectToPage("/Dashboard");
```

## 13. Layout Navigation Process

Navigation links are in `Pages/Shared/_Layout.cshtml`.

Authenticated user condition:

```csharp
@if (User.Identity?.IsAuthenticated ?? false)
```

If logged in, the user can see links such as:

```html
<a class="nav-link active-pill" asp-page="/About">ABOUT</a>
<a class="nav-link active-pill" asp-page="/Contact">CONTACT</a>
<a class="nav-link active-pill" asp-page="/Resume">RESUME</a>
<a class="nav-link active-pill" asp-page="/Porfolio">PORTFOLIO</a>
```

Admin-only navigation:

```csharp
@if (User.IsInRole("Admin"))
```

If the logged-in user is Admin, the dashboard link appears.

## 14. Full Request Flow Summary

Login flow:

```text
User opens /Auth
User submits login form
Auth.cshtml.cs checks database
If correct, SignInAsync creates cookie
RedirectToPage("/Index")
Navbar shows authenticated links
```

Register flow:

```text
User opens register form
User submits name/email/password
Auth.cshtml.cs checks duplicate email
New user is saved into users table
RedirectToPage("/Auth", action = "login")
```

Protected page flow:

```text
User opens /About, /Resume, or Admin page
Authorize checks login cookie
If not logged in, redirect to /Auth
If role is wrong, redirect to /Index
```

Database flow:

```text
appsettings.json stores connection string
Program.cs loads DefaultConnection
ApplicationDbContext connects to MySQL
Page models use _db to query and save data
```

## 15. Important Security Note

The current login code stores and checks the password directly:

```csharp
u.PasswordHash == Input.Password
```

For a real production system, passwords should be hashed with a secure password hasher instead of being stored as plain text.

Also, private secrets such as email app passwords should not be committed to public GitHub repositories.
