# RazorDemo Code Process Documentation

This document explains how the project code runs from start to finish. It is focused on process flow rather than file-by-file explanation.

## 1. Application Startup Process

When the application starts, `Program.cs` runs first.

Process:

1. ASP.NET Core creates the app builder.
2. The app reads configuration from `appsettings.json` and `appsettings.Development.json`.
3. The MySQL connection string named `DefaultConnection` is loaded.
4. `ApplicationDbContext` is registered with Entity Framework Core.
5. Cookie authentication is registered with the scheme name `Cookies`.
6. Authorization services are registered.
7. Razor Pages are registered.
8. The app builds the middleware pipeline.
9. Static files, routing, authentication, and authorization middleware are enabled.
10. Razor Pages are mapped.
11. The web server starts and waits for browser requests.

Important middleware order:

```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
```

Authentication must run before authorization because authorization checks need to know who the current user is.

## 2. Browser Request Process

When a user opens a page in the browser:

1. The browser sends a request, for example `/Index`.
2. ASP.NET Core routing finds the matching Razor Page.
3. If the page has `[Authorize]`, the authentication cookie is checked.
4. If the user is not allowed, ASP.NET Core redirects them.
5. The page model handler runs, such as `OnGet()` or `OnGetAsync()`.
6. The `.cshtml` page renders HTML.
7. `_Layout.cshtml` wraps the page with the navbar, styles, and scripts.
8. The final HTML is sent back to the browser.

## 3. Home Page Process

Files:

- `Pages/Index.cshtml`
- `Pages/Index.cshtml.cs`

Process:

1. User visits `/Index`.
2. `IndexModel.OnGet()` runs.
3. No database work happens.
4. `Index.cshtml` renders the landing page.
5. `_Layout.cshtml` adds the navbar.
6. The user sees the public NETIZEN home page.

The home page is public, so login is not required.

## 4. Login Process

Files:

- `Pages/Login.cshtml`
- `Pages/Auth.cshtml.cs`
- `Controller/ApplicationDbContext.cs`
- `Model/AppUsers.cs`

Process:

1. User opens `/Login?action=login`.
2. `Login.cshtml` shows the login form.
3. User enters email and password.
4. The form posts with `asp-page-handler="Login"`.
5. Razor Pages calls `AuthModel.OnPostLoginAsync()`.
6. The code checks whether the form model is valid.
7. Entity Framework searches the `users` table:

```csharp
var user = await _db.AppUsers
    .FirstOrDefaultAsync(u => u.Email == Input.Email && u.PasswordHash == Input.Password);
```

8. If no user is found, an error message is shown.
9. If a user is found, the code creates claims:
   - Name
   - Email
   - Role
10. The claims are placed inside a `ClaimsIdentity`.
11. ASP.NET Core signs in the user with a cookie:

```csharp
await HttpContext.SignInAsync("Cookies", principal);
```

12. The browser receives the authentication cookie named `UserLoginCookie`.
13. The user is redirected to `/Index`.

After login, the layout can read:

```csharp
User.Identity.IsAuthenticated
User.Identity.Name
User.IsInRole("Admin")
```

## 5. Registration Process

Files:

- `Pages/Login.cshtml`
- `Pages/Auth.cshtml.cs`
- `Model/AppUsers.cs`

Process:

1. User opens `/Login?action=register`.
2. `Login.cshtml` shows the registration form.
3. User enters name, email, role, gender, password, and confirm password.
4. The form posts with `asp-page-handler="Register"`.
5. Razor Pages calls `AuthModel.OnPostRegisterAsync()`.
6. The code checks validation.
7. The code compares password and confirm password.
8. The database is checked to make sure the email is not already used.
9. A new `AppUsers` object is created.
10. If no role is selected, the role defaults to `Student`.
11. The new user is added to `_db.AppUsers`.
12. `_db.SaveChangesAsync()` saves the user into MySQL.
13. A success message is stored in `TempData`.
14. The user is redirected back to the login form.

Important note:

The current project stores the password text in `PasswordHash`. In a real production system, this should be replaced with password hashing.

## 6. Logout Process

Files:

- `Pages/Shared/_Layout.cshtml`
- `Pages/Auth.cshtml.cs`

Process:

1. Logged-in user clicks `LOGOUT` in the navbar.
2. The form posts to `/Login` with the logout handler.
3. Razor Pages calls `AuthModel.OnPostLogoutAsync()`.
4. The code removes the authentication cookie:

```csharp
await HttpContext.SignOutAsync("Cookies");
```

5. The user is redirected to `/Index`.
6. The layout now shows `LOGIN` and `REGISTER` again.

## 7. Navigation Display Process

File:

- `Pages/Shared/_Layout.cshtml`

Process:

1. `_Layout.cshtml` checks whether the user is logged in.
2. If the user is not logged in:
   - Shows Home.
   - Shows Login.
   - Shows Register.
3. If the user is logged in:
   - Shows Home.
   - Shows About.
   - Shows Contact.
   - Shows Resume.
   - Shows Portfolio.
   - Shows Logout.
   - Shows greeting with the user's name.
4. If the user is logged in as Admin:
   - Shows Dashboard.

Code pattern:

```csharp
@if (User.Identity?.IsAuthenticated ?? false)
{
    // protected nav links
}

@if (User.IsInRole("Admin"))
{
    // dashboard link
}
```

## 8. Protected Page Process

Files:

- `Pages/About.cshtml.cs`
- `Pages/Resume.cshtml.cs`

Both pages use:

```csharp
[Authorize]
```

Process:

1. User requests `/About` or `/Resume`.
2. ASP.NET Core checks the authentication cookie.
3. If no valid cookie exists, the user is redirected to `/Login`.
4. If the cookie is valid, the page is shown.

These pages require login but do not require a specific role.

## 9. Admin Dashboard Process

Files:

- `Pages/Dashboard.cshtml`
- `Pages/Dashboard.cshtml.cs`
- `Model/AppUsers.cs`

The dashboard uses:

```csharp
[Authorize(Roles = "Admin")]
```

Process:

1. User requests `/Dashboard`.
2. ASP.NET Core checks whether the user is logged in.
3. ASP.NET Core checks whether the user's role claim is `Admin`.
4. If the user is not Admin, access is denied.
5. If the user is Admin, `DashboardModel.OnGetAsync()` runs.
6. The code loads all users from the database:

```csharp
Users = await _db.AppUsers.ToListAsync();
```

7. The code counts:
   - Total users.
   - Admin users.
   - Student users.
8. `Dashboard.cshtml` displays statistic cards.
9. `Dashboard.cshtml` loops through `Model.Users` and displays the user table.

## 10. Delete User Process

Files:

- `Pages/Dashboard.cshtml`
- `Pages/Dashboard.cshtml.cs`

Process:

1. Admin clicks the delete button in the dashboard table.
2. Browser shows a JavaScript confirmation message.
3. If confirmed, the form posts with `asp-page-handler="Delete"`.
4. The user id is sent through `asp-route-id`.
5. Razor Pages calls:

```csharp
OnPostDeleteAsync(int id)
```

6. The code searches the database by id.
7. If the user exists, EF Core removes the record.
8. `_db.SaveChangesAsync()` saves the deletion.
9. The admin is redirected back to the dashboard.
10. The dashboard reloads the updated user list.

## 11. Edit User Process

Files:

- `Pages/EditUser.cshtml`
- `Pages/EditUser.cshtml.cs`

Process for opening the edit page:

1. Admin clicks the edit button on the dashboard.
2. Browser navigates to `/EditUser/{id}`.
3. `EditUserModel.OnGetAsync(int id)` runs.
4. The code searches the database for that user id.
5. If no user exists, the admin is redirected back to dashboard.
6. If the user exists, the form is filled with user data.

Process for saving changes:

1. Admin edits name, email, gender, role, or remark.
2. Admin clicks `SAVE CHANGES`.
3. The form posts to `EditUserModel.OnPostAsync()`.
4. The code validates the form.
5. The current database record is loaded again.
6. The allowed fields are updated:
   - Name
   - Email
   - Gender
   - Role
   - Remark
7. Password is not changed.
8. `_db.SaveChangesAsync()` saves the update.
9. Success message is stored in `TempData`.
10. Admin is redirected to `/Dashboard`.

## 12. Contact Form Process

Files:

- `Pages/Contact.cshtml`
- `Pages/Contact.cshtml.cs`
- `appsettings.json`

Process:

1. User opens `/Contact`.
2. `Contact.cshtml` displays contact details and a message form.
3. User enters name, email, and message.
4. The form posts to `ContactModel.OnPostAsync()`.
5. The code checks validation attributes:
   - Name is required.
   - Email is required and must be valid.
   - Message is required.
6. SMTP settings are read from `appsettings.json`.
7. A `MailMessage` object is created.
8. The email body is built as HTML.
9. `SmtpClient` connects to Gmail SMTP.
10. The email is sent.
11. If successful, a success message is shown.
12. If sending fails, an error message is shown.

Important note:

The Contact page link is only shown after login, but the page model does not currently have `[Authorize]`. That means someone can still open `/Contact` directly.

## 13. Portfolio Page Process

Files:

- `Pages/Porfolio.cshtml`
- `Pages/Porfolio.cshtml.cs`

Process:

1. User opens `/Porfolio`.
2. `PorfolioModel.OnGet()` runs.
3. The page model fills portfolio data directly in C#:
   - Name
   - Title
   - Story
   - Current focus
   - Contact details
   - Skills
   - Education records
4. `Porfolio.cshtml` displays that data.
5. The skills section loops through `Model.Skills`.
6. The footer displays phone, email, and Facebook link.

Important note:

The route and file are spelled `Porfolio`, not `Portfolio`.

## 14. About Page Process

Files:

- `Pages/About.cshtml`
- `Pages/About.cshtml.cs`

Process:

1. Logged-in user opens `/About`.
2. `[Authorize]` checks login.
3. `AboutModel.OnGet()` runs.
4. Static profile content is rendered.
5. CSS from `site.css` creates the profile-card and details-card layout.

## 15. Resume Page Process

Files:

- `Pages/Resume.cshtml`
- `Pages/Resume.cshtml.cs`
- `wwwroot/css/site.css`

Process:

1. Logged-in user opens `/Resume`.
2. `[Authorize]` checks login.
3. `ResumeModel.OnGet()` runs.
4. Resume HTML is rendered.
5. User can click the download icon.
6. The page calls:

```javascript
window.print()
```

7. Browser print dialog opens.
8. Print CSS hides navigation and changes the resume to a print-friendly white page.

## 16. Error Page Process

Files:

- `Pages/Error.cshtml`
- `Pages/Error.cshtml.cs`
- `Program.cs`

Process:

1. In production, if an unhandled exception happens, `Program.cs` sends the user to `/Error`.
2. `ErrorModel.OnGet()` runs.
3. The request id is collected.
4. `Error.cshtml` displays the error page.
5. The request id can help debug the issue.

## 17. Database Process

Files:

- `Controller/ApplicationDbContext.cs`
- `Model/AppUsers.cs`

Process:

1. Page models request `ApplicationDbContext` through constructor injection.
2. ASP.NET Core creates the context using the MySQL connection string.
3. Code reads or writes through `_db.AppUsers`.
4. EF Core translates C# queries into SQL.
5. MySQL executes the query.
6. EF Core converts database rows into `AppUsers` objects.
7. When data changes, `_db.SaveChangesAsync()` writes changes back to MySQL.

Expected table:

```text
users
```

Expected columns:

```text
id
name
email
gender
role
remark
password_hash
```

## 18. CSS Loading Process

Files:

- `Pages/Shared/_Layout.cshtml`
- `wwwroot/css/site.css`
- `Pages/Shared/_Layout.cshtml.css`

Process:

1. `_Layout.cshtml` loads Bootstrap CSS.
2. `_Layout.cshtml` loads `site.css`.
3. `_Layout.cshtml` loads the scoped CSS bundle.
4. Page HTML uses CSS class names.
5. Browser applies Bootstrap styles first.
6. Browser applies project custom styles from `site.css`.
7. Page-specific inline styles apply where they exist.

Most visual design is controlled by `site.css`.

## 19. JavaScript Process

Files:

- `Pages/Login.cshtml`
- `Pages/Auth.cshtml`
- `wwwroot/js/site.js`

Current JavaScript behavior:

- Login/register form switching.
- Password show/hide buttons.
- Print button on resume page.
- Delete confirmation on dashboard.

`wwwroot/js/site.js` currently has no active custom JavaScript.

## 20. Complete User Journey

Normal student/user journey:

1. Open `/Index`.
2. Click Register.
3. Create account.
4. Redirect to login.
5. Login with email and password.
6. Access About, Contact, Resume, and Portfolio.
7. Logout from navbar.

Admin journey:

1. Open `/Login`.
2. Login with an account whose role is `Admin`.
3. Dashboard link appears in navbar.
4. Open `/Dashboard`.
5. View user statistics.
6. Edit users.
7. Delete users.
8. Logout.

## 21. Important Process Risks

- Passwords are stored as plain text even though the property is named `PasswordHash`.
- Email credentials are stored in `appsettings.json`.
- Contact page is not protected by `[Authorize]`.
- Dashboard search input is visual only and does not filter users yet.
- No migrations or database schema file are included.
- `Auth.cshtml` and `Login.cshtml` duplicate similar authentication UI.
- Some referenced packages are not needed for the current process, such as SQL Server and Identity UI.

