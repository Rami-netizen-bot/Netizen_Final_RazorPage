# RazorDemo / NETIZEN Project Documentation

This document explains the source code in this ASP.NET Core Razor Pages project. It focuses on the files that developers edit directly. Generated build output, third-party libraries, and binary backup files are described separately at the end.

## 1. Project Overview

`RazorDemo` is a .NET 9 Razor Pages web application for a portfolio and user-management site called NETIZEN. The application includes:

- Public home page.
- Cookie-based login, registration, and logout.
- MySQL database access through Entity Framework Core.
- Role-based authorization for protected pages.
- Admin-only dashboard for viewing, editing, and deleting users.
- Portfolio, about, contact, and resume pages.
- Custom dark/cyan visual theme using Bootstrap plus project CSS.

The main application flow is:

1. `Program.cs` starts the web app and configures services.
2. `ApplicationDbContext` connects EF Core to the MySQL `users` table.
3. Authentication pages create a cookie containing user claims.
4. Razor Pages use `[Authorize]` and `[Authorize(Roles = "Admin")]` to restrict access.
5. Shared layout renders navigation based on whether the user is logged in and whether the user has the `Admin` role.

## 2. Technology Stack

- Framework: ASP.NET Core Razor Pages, .NET 9.0
- ORM: Entity Framework Core
- Database provider: Pomelo Entity Framework Core MySQL
- Authentication: ASP.NET Core cookie authentication
- Authorization: ASP.NET Core claims and role authorization
- UI: Razor views, Bootstrap, Bootstrap Icons, Font Awesome, custom CSS
- JavaScript: small inline scripts for auth form switching and password visibility

## 3. Source Tree

Important source folders:

- `Program.cs`: application startup and middleware pipeline.
- `RazorDemo.csproj`: project target framework and NuGet packages.
- `appsettings.json`: runtime configuration, database connection, and email settings.
- `Controller/ApplicationDbContext.cs`: EF Core database context.
- `Model/`: database model classes.
- `Pages/`: Razor Pages and their C# page models.
- `Pages/Shared/`: layout and shared validation scripts.
- `wwwroot/css/site.css`: main project styling.
- `wwwroot/js/site.js`: placeholder for site JavaScript.
- `wwwroot/images/`: logo and profile/about images.
- `wwwroot/lib/`: client libraries such as Bootstrap and jQuery.

Generated or non-source folders:

- `bin/`: compiled application output.
- `obj/`: intermediate build output.
- `Backups/`: zipped backup.
- `Reports/`: existing report document.
- `App_Data/DataProtectionKeys/`: ASP.NET Core data protection key material used for encrypting/signing cookies and other protected data.

## 4. Startup Code

### `Program.cs`

This is the entry point for the web application.

Key responsibilities:

- Creates the web app builder with `WebApplication.CreateBuilder(args)`.
- Reads the MySQL connection string named `DefaultConnection` from configuration.
- Registers `ApplicationDbContext` with EF Core and Pomelo MySQL:
  - `UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))`
  - EF Core will use this context whenever pages request `ApplicationDbContext` through dependency injection.
- Configures cookie authentication with the scheme name `Cookies`.
  - `LoginPath = "/Login"` means protected pages redirect unauthenticated users to `/Login`.
  - `AccessDeniedPath = "/Index"` means authenticated users without permission are sent to the home page.
  - `Cookie.Name = "UserLoginCookie"` sets the browser cookie name.
- Registers authorization.
- Registers Razor Pages.
- Configures middleware:
  - Production exception handling and HSTS.
  - HTTPS redirection.
  - Static file serving from `wwwroot`.
  - Routing.
  - Authentication.
  - Authorization.
  - Razor page endpoint mapping.

Important order:

- `app.UseAuthentication()` must run before `app.UseAuthorization()` so role checks can read the authenticated user.

Note:

- `Microsoft.AspNetCore.Identity` is imported, and Identity packages are referenced, but this project uses custom cookie authentication instead of ASP.NET Core Identity user management.

## 5. Project File

### `RazorDemo.csproj`

Defines the project as an ASP.NET Core web app:

- SDK: `Microsoft.NET.Sdk.Web`
- Target framework: `net9.0`
- Nullable reference types: enabled
- Implicit usings: enabled

NuGet packages:

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` and `Microsoft.AspNetCore.Identity.UI`
  - Available but not actively used for login in the current code.
- `Microsoft.EntityFrameworkCore.SqlServer`
  - Referenced but the active database provider is MySQL.
- `Microsoft.EntityFrameworkCore.Tools`
  - Supports EF Core design-time tooling and migrations.
- `Pomelo.EntityFrameworkCore.MySql`
  - Active provider used by `Program.cs`.

## 6. Configuration

### `appsettings.json`

Contains:

- Logging level configuration.
- `ConnectionStrings:DefaultConnection`
  - Points to local MySQL database `loginappdb`.
  - Used by `Program.cs`.
- `AllowedHosts`
  - `*` allows any host header.
- `EmailSettings`
  - SMTP server, port, sender email, sender name, and app password.
  - Used by `ContactModel` to send emails.

Security note:

- The SMTP app password and database connection details should not be committed in a real public repository. Prefer user secrets, environment variables, or a secure secret manager.

### `appsettings.Development.json`

Development-only settings:

- Enables detailed Razor errors.
- Sets logging levels for development.

### `Properties/launchSettings.json`

Used by Visual Studio and `dotnet run` profiles. It normally defines local URLs, environment variables, and launch behavior. It does not affect deployed production behavior.

## 7. Database Layer

### `Controller/ApplicationDbContext.cs`

Namespace: `RazorDemo.Data`

This class inherits from `DbContext`, which is EF Core's main database session object.

Important members:

- Constructor:
  - Receives `DbContextOptions<ApplicationDbContext>` from dependency injection.
  - Passes options to the base `DbContext`.
- `DbSet<AppUsers> AppUsers`
  - Represents rows in the `users` table.
  - Used by authentication, dashboard, editing, and deletion code.
- `OnModelCreating(ModelBuilder modelBuilder)`
  - Calls the base implementation.
  - Maps `AppUsers` to the database table named `users`.

Folder note:

- The folder is named `Controller`, but this is not an MVC controller. It is the EF Core database context.

## 8. Model Classes

### `Model/AppUsers.cs`

Namespace: `RazorDemo.Model`

This is the active database entity used by the project.

Properties:

- `Id`
  - Primary key.
  - Mapped to database column `id`.
- `Name`
  - Mapped to `name`.
- `Email`
  - Mapped to `email`.
- `Gender`
  - Mapped to `gender`.
- `Role`
  - Mapped to `role`.
  - Used for authorization claims and dashboard display.
- `Remark`
  - Mapped to `remark`.
  - Admin notes field.
- `PasswordHash`
  - Mapped to `password_hash`.
  - Despite the name, the current authentication code stores and compares the plain password string.

Security note:

- Passwords should be hashed with a password hasher before storage. Current code is simple for project/demo purposes but is not safe for production.

### `Model/User.cs`

Namespace: `RazorDemo.Models`

This class defines a similar user shape:

- `Id`
- `Name`
- `Email`
- `Gender`
- `Role`
- `Remark`
- `PasswordHash`

Current status:

- This class does not appear to be used by the active pages.
- The active EF Core entity is `AppUsers`.
- It may be an earlier model or a placeholder.

## 9. Razor Page Basics

Each Razor Page usually has two files:

- `.cshtml`: HTML/Razor markup for the page.
- `.cshtml.cs`: C# page model containing request handlers and server-side logic.

Common Razor Page handler methods:

- `OnGet()`: runs for HTTP GET requests.
- `OnGetAsync()`: async version for GET requests.
- `OnPostAsync()`: runs for standard POST requests.
- `OnPostLoginAsync()`: runs when the form posts with `asp-page-handler="Login"`.
- `OnPostRegisterAsync()`: runs when the form posts with `asp-page-handler="Register"`.
- `OnPostDeleteAsync()`: runs when the form posts with `asp-page-handler="Delete"`.

## 10. Shared Razor Files

### `Pages/_ViewImports.cshtml`

Imports common namespaces and tag helpers for Razor pages:

- `@using RazorDemo`
- `@namespace RazorDemo.Pages`
- Adds MVC tag helpers with `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`

The tag helpers allow syntax such as:

- `asp-page="/Login"`
- `asp-for="Input.Email"`
- `asp-route-id="@user.Id"`
- `asp-validation-for="Email"`

### `Pages/_ViewStart.cshtml`

Sets the default layout:

- `Layout = "_Layout";`

This means normal pages render inside `Pages/Shared/_Layout.cshtml`.

### `Pages/Shared/_Layout.cshtml`

This is the global page shell.

Main parts:

- HTML document structure.
- `<head>` includes:
  - Bootstrap CSS.
  - Project CSS at `~/css/site.css`.
  - Scoped CSS bundle `~/RazorDemo.styles.css`.
  - Bootstrap Icons from CDN.
  - Favicon/logo.
- Header/nav:
  - Brand logo links to `/Index`.
  - Home link is always visible.
  - About, Contact, Resume, and Portfolio links are visible only when the user is authenticated.
  - Dashboard link is visible only when `User.IsInRole("Admin")` is true.
  - Login/Register buttons show when the user is not authenticated.
  - Hello message and Logout button show when authenticated.
- Main content:
  - `@RenderBody()` is where each page's content is inserted.
- Scripts:
  - jQuery.
  - Bootstrap bundle.
  - `~/js/site.js`.
  - Optional page-specific scripts through `@RenderSectionAsync("Scripts", required: false)`.

The logout form posts to `/Login` with the `Logout` page handler.

### `Pages/Shared/_Layout.cshtml.css`

Scoped CSS generated from the default template style. It contains base layout styling for navbar brand, links, buttons, borders, shadows, and footer behavior.

Current status:

- Most visible project styling is in `wwwroot/css/site.css`.
- This file still contains template support styles.

### `Pages/Shared/_ValidationScriptsPartial.cshtml`

Includes jQuery validation scripts:

- `jquery.validate.min.js`
- `jquery.validate.unobtrusive.min.js`

Pages can render this partial when client-side validation is needed.

## 11. Authentication Pages

### `Pages/Login.cshtml`

This is the primary login/register page because `Program.cs` sets `LoginPath` to `/Login`, and the layout links to `/Login`.

Model:

- Uses `RazorDemo.Pages.AuthModel`.
- There is no separate `Login.cshtml.cs`; instead, it reuses `Auth.cshtml.cs`.

Important behavior:

- Reads query string `action`.
  - `?action=login` shows login form.
  - `?action=register` shows registration form.
- Displays `TempData["SuccessMessage"]` after registration.
- Login form:
  - Posts with `asp-page-handler="Login"`.
  - Binds email and password to `Input.Email` and `Input.Password`.
- Registration form:
  - Posts with `asp-page-handler="Register"`.
  - Binds name, email, role, gender, password.
  - Sends confirm password through a normal form field named `confirmPassword`.
- Inline JavaScript:
  - `toggleAuth()` switches between login and register forms and updates the query string.
  - `togglePasswordVisibility(inputId, btn)` toggles password inputs between `password` and `text`.

### `Pages/Auth.cshtml`

This is a second authentication page that also uses `AuthModel`.

It contains:

- Login form.
- Registration form.
- Error message display from `ModelState`.
- "Remember me", "Forgot password", terms checkbox, and Google button UI elements.
- Inline JavaScript for switching forms and showing/hiding passwords.

Current status:

- It works as an alternative auth page if routed directly to `/Auth`.
- The main app flow uses `/Login`.

### `Pages/Auth.cshtml.cs`

Namespace: `RazorDemo.Pages`

This is the shared page model for both `/Auth` and `/Login`.

Dependencies:

- `ApplicationDbContext _db`
  - Injected by constructor.
  - Used to read and write users.

Bound property:

- `Input`
  - Instance of nested `InputModel`.
  - Filled automatically from posted form fields.

`InputModel` properties:

- `Name`
- `Email`
- `Password`
- `Gender`
- `Role`

Validation:

- `Email` is required.
- `Password` is required.

Handlers:

- `OnGet()`
  - Does nothing special; just renders the page.

- `OnPostRegisterAsync()`
  - Checks `ModelState`.
  - Reads `confirmPassword` from `Request.Form`.
  - Rejects registration when passwords do not match.
  - Checks whether email already exists in `_db.AppUsers`.
  - Creates a new `AppUsers` record.
  - Uses role from the form or defaults to `Student`.
  - Saves the user.
  - Sets a success message in `TempData`.
  - Redirects to `/Login?action=login`.

- `OnPostLoginAsync()`
  - Checks `ModelState`.
  - Finds a user where email and password match.
  - If found:
    - Creates claims for name, email, and role.
    - Creates a `ClaimsIdentity` using the `Cookies` scheme.
    - Signs the user in through `HttpContext.SignInAsync("Cookies", principal)`.
    - Redirects to `/Index`.
  - If not found:
    - Adds a model error.
    - Returns the page.

- `OnPostLogoutAsync()`
  - Calls `HttpContext.SignOutAsync("Cookies")`.
  - Redirects to `/Index`.

Authorization impact:

- The role claim created during login is what allows Admin users to access the dashboard.

## 12. Admin Dashboard

### `Pages/Dashboard.cshtml.cs`

Namespace: `RazorDemo.Pages`

Authorization:

- `[Authorize(Roles = "Admin")]`
- Only authenticated users with role claim `Admin` can access it.

Dependencies:

- `ApplicationDbContext _db`

Page properties:

- `Users`
  - List of all `AppUsers`.
- `TotalUsers`
  - Total number of records.
- `AdminCount`
  - Number of users whose role is `Admin`.
- `StudentCount`
  - Number of users whose role is empty/null or `Student`.

Handlers:

- `OnGetAsync()`
  - Loads all users with `_db.AppUsers.ToListAsync()`.
  - Calculates dashboard statistics.

- `OnPostDeleteAsync(int id)`
  - Finds a user by id.
  - Removes the user if found.
  - Saves changes.
  - Redirects back to the dashboard.

### `Pages/Dashboard.cshtml`

Renders the admin user-management UI.

Main sections:

- Header with `Admin Dashboard` title and `Admin Mode` badge.
- Three statistic cards:
  - Total users.
  - Administrators.
  - Students.
- User management table:
  - Loops through `Model.Users`.
  - Displays id, name, email, gender, and role.
  - Shows a colored role badge for Admin, Student, User, or unknown roles.
  - Delete form posts to `OnPostDeleteAsync`.
  - Edit link navigates to `/EditUser/{id}`.

Inline page styles:

- Customize table hover color.
- Set transparent table backgrounds.
- Style action button hover behavior.

## 13. Edit User Page

### `Pages/EditUser.cshtml.cs`

Namespace: `RazorDemo.Pages`

Authorization:

- `[Authorize(Roles = "Admin")]`

Dependencies:

- `ApplicationDbContext _db`

Bound property:

- `UserToEdit`
  - The user being edited.
  - Filled from database on GET.
  - Filled from form data on POST.

Handlers:

- `OnGetAsync(int id)`
  - Uses the route id to find the user.
  - Redirects to dashboard if the user does not exist.
  - Returns the page if found.

- `OnPostAsync()`
  - Validates the model state.
  - Loads the existing database record.
  - Returns `NotFound()` if missing.
  - Updates allowed fields:
    - Name
    - Email
    - Gender
    - Role
    - Remark
  - Does not update `PasswordHash`.
  - Saves changes.
  - Stores a success message in `TempData`.
  - Redirects to dashboard.

### `Pages/EditUser.cshtml`

Route:

- `@page "{id:int}"`
- URL looks like `/EditUser/5`.

Main UI:

- Back link to dashboard.
- Page title showing the user id.
- Form fields for:
  - Full name.
  - Email.
  - Gender.
  - Role.
  - Remark/notes.
- Hidden field for `UserToEdit.Id`.
- Save button posts to `OnPostAsync`.
- Cancel button returns to dashboard.

Inline page styles:

- Improve form focus appearance.
- Style placeholders.

## 14. Protected Content Pages

### `Pages/About.cshtml.cs`

Authorization:

- `[Authorize]`
- Any logged-in user can access it.

Handler:

- `OnGet()` renders the page.

### `Pages/About.cshtml`

Displays an about/profile page.

Content:

- Profile photo from `wwwroot/images/photo_2025-10-31_11-27-40.jpg`.
- Name and title.
- Skill tags.
- Email, phone, and Facebook placeholder.
- Story and focus text.
- Three skill tiles:
  - Frontend skills.
  - Backend skills.
  - Mobile app skills.

Styling comes mostly from `site.css` classes such as:

- `about-container`
- `portfolio-container`
- `profile-card`
- `details-card`
- `tags`
- `skills-grid`

### `Pages/Resume.cshtml.cs`

Authorization:

- `[Authorize]`
- Any logged-in user can access it.

Handler:

- `OnGet()` renders the resume page.

### `Pages/Resume.cshtml`

Displays a printable resume.

Features:

- Download/print button using `javascript:window.print()`.
- Resume header with name, subtitle, and contact info.
- Technical skills chips.
- Soft skills chips.
- Summary section.
- More/details list.

Print behavior:

- `site.css` contains `@media print` styles that hide navigation and make the resume print on a white background.

## 15. Contact Page

### `Pages/Contact.cshtml.cs`

Namespace: `RazorDemo.Pages`

Dependencies:

- `IConfiguration _configuration`
  - Reads SMTP settings from `appsettings.json`.

Bound form properties:

- `Name`
  - Required.
- `Email`
  - Required and must be a valid email.
- `Message`
  - Required.

TempData:

- `StatusMessage`
  - Shows success or error message after sending.

Handlers:

- `OnGet()`
  - Renders the contact form.

- `OnPostAsync()`
  - Validates form data.
  - Reads SMTP configuration:
    - Server
    - Port
    - Sender email
    - Sender name
    - App password
  - Builds an HTML email with submitted name, sender email, and message.
  - Sends the message with `SmtpClient`.
  - On success:
    - Sets success status.
    - Redirects back to the page.
  - On failure:
    - Sets error status.
    - Returns the page.

Implementation note:

- `SmtpClient` works for this project, but modern production apps often use a dedicated email service SDK or background email sender.

### `Pages/Contact.cshtml`

Displays:

- Contact information panel.
- Contact form with fields for name, email, and message.
- Validation messages using `asp-validation-for`.
- Submit button.
- Alert area for success or error status.

Uses Bootstrap grid classes and custom classes from `site.css`.

## 16. Home Pages

### `Pages/Index.cshtml.cs`

Default home page model.

Dependencies:

- `ILogger<IndexModel> _logger`

Handler:

- `OnGet()` renders the page.

The logger is injected but not currently used inside `OnGet()`.

### `Pages/Index.cshtml`

Primary public landing page.

Content:

- Hero badge: final exam project text.
- Hero title about building websites with C# and Razor Pages.
- Subtitle placeholder text.
- GitHub link.
- Follow Me placeholder link.
- Logo image from `wwwroot/images/logo.png`.

### `Pages/Home.cshtml.cs`

Simple page model with `OnGet()`.

Current status:

- No custom logic.

### `Pages/Home.cshtml`

Similar to `Index.cshtml`.

Current status:

- Appears to be an alternative or older home page.
- The main navbar uses `/Index`.

## 17. Portfolio Page

### `Pages/Porfolio.cshtml.cs`

Namespace: `MyApp.Namespace`

Important spelling:

- The file and page are named `Porfolio`, not `Portfolio`.
- The layout links to `/Porfolio`, so the current spelling is part of the route.

Model class:

- `PorfolioModel`

Properties:

- `Name`
- `Title`
- `MyStory`
- `CurrentFocus`
- `Contact`
- `Skills`
- `Education`
- `Education2`
- `Education3`

Nested/helper classes:

- `ContactDetails`
  - Email
  - Phone
  - FacebookUrl
- `SkillGroup`
  - Category
  - Technologies
- `EducationDetails`
  - Degree
  - Primary
  - Year
- `EducationDetails2`
  - Degree
  - HightSchool
  - Year
- `EducationDetails3`
  - Degree
  - University
  - Year

Handler:

- `OnGet()`
  - Populates all portfolio data in code.
  - Sets contact details.
  - Creates three skill groups.
  - Sets primary school, high school, and university details.

Current status:

- Portfolio data is hard-coded in the page model rather than stored in the database.

### `Pages/Porfolio.cshtml`

Displays a portfolio card.

Content:

- Profile image from `wwwroot/images/photo_2026-05-18_00-30-22.jpg`.
- Greeting and story using model data.
- Education timeline using `Education`, `Education2`, and `Education3`.
- Skills icon grid with C#, Mobile, Web, Flutter, and Dart.
- Experience timeline generated by looping over `Model.Skills`.
- Footer row with phone, email, and Facebook profile link.

External CSS:

- Loads Font Awesome from CDN for icons.
- Also links `~/css/site.css`, although the layout already includes it.

## 18. Static Informational Pages

### `Pages/Privacy.cshtml.cs`

Simple page model.

Dependencies:

- `ILogger<PrivacyModel> _logger`

Handler:

- `OnGet()` renders the page.

### `Pages/Privacy.cshtml`

Default privacy page placeholder.

Current status:

- Contains basic placeholder text.
- Not visible in the active navbar because the footer is commented out in the layout.

### `Pages/Error.cshtml.cs`

Default error page model.

Attributes:

- `[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]`
- `[IgnoreAntiforgeryToken]`

Dependencies:

- `ILogger<ErrorModel> _logger`

Properties:

- `RequestId`
  - Stores current request/activity id.
- `ShowRequestId`
  - Returns true when `RequestId` is not empty.

Handler:

- `OnGet()`
  - Reads the request id from `Activity.Current?.Id` or `HttpContext.TraceIdentifier`.

Used by:

- Production exception handler in `Program.cs` through `app.UseExceptionHandler("/Error")`.

### `Pages/Error.cshtml`

Default error page view.

Expected behavior:

- Displays an error message.
- Shows request id when available.

## 19. CSS and Frontend Styling

### `wwwroot/css/site.css`

This is the main stylesheet for the app.

Major sections:

- Base HTML/body styles.
  - Font size.
  - Body background.
  - Dark theme.
  - Fixed navbar spacing.
- CSS variables:
  - `--bg-dark`
  - `--card-bg`
  - `--card-border`
  - `--text-primary`
  - `--text-secondary`
  - `--accent-cyan`
  - `--focus-bg`
- Shared layout styles:
  - `main-wrapper`
  - `.container`
  - `.hero-title`
  - `.hero-subtitle`
  - `.text-blue`
  - `.badge-pill`
  - `.btn-cyan`
  - `.btn-outline`
  - `.learning-card-box`
  - `.icon-box`
  - `.stat-card`
- Navbar styles:
  - `.custom-navbar`
  - `.active-pill`
  - custom mobile navbar toggler
  - login/register button styling
- Authentication styles:
  - `.auth-body`
  - `.auth-card`
  - `.auth-wrapper`
  - `.auth-container`
  - `.auth-alert`
  - `.btn-cyan-auth`
  - `.btn-primary-auth`
  - `.btn-google-auth`
  - form inputs, autofill fixes, checkbox styles
  - `fadeIn` animation
- About page styles:
  - `.about-container`
  - `.portfolio-container`
  - `.profile-card`
  - `.details-card`
  - `.profile-img`
  - `.tags`
  - `.focus-box`
  - `.skills-grid`
- Contact page styles:
  - `.contact-wrapper`
  - `.contact-container`
  - `.contact-title`
  - `.contact-detail`
  - `.custom-input`
  - `.btn-gradient-submit`
- Resume styles:
  - `.resume-wrapper`
  - `.resume-document`
  - `.resume-download-btn`
  - `.resume-name`
  - `.resume-section-title`
  - `.skill-chip`
  - print-specific styling with `@media print`
- Home hero logo styles:
  - `.hero-logo-wrapper`
  - `.hero-logo`
  - `float` animation
- Portfolio page styles:
  - `.portfolio-card-frame`
  - `.portfolio-layout-grid`
  - `.profile-photo-container`
  - `.column-title-main`
  - `.section-heading-bold`
  - `.body-text-muted`
  - `.timeline-item`
  - `.skills-icon-grid`
  - `.skill-icon-box`
  - `.portfolio-footer-row`
- Responsive styles:
  - Adjusts layout for tablet and mobile widths.
  - Stacks portfolio columns.
  - Adjusts body padding and auth buttons on small screens.

### `wwwroot/js/site.js`

Current status:

- Contains only template comments.
- No active global JavaScript.
- Page-specific JavaScript is currently inline in `Auth.cshtml` and `Login.cshtml`.

## 20. Static Assets

### `wwwroot/images/logo.png`

Used by:

- Navbar brand.
- Home/Index hero image.
- Favicon link.

### `wwwroot/images/photo_2025-10-31_11-27-40.jpg`

Used by:

- `About.cshtml` profile image.

### `wwwroot/images/photo_2026-05-18_00-30-22.jpg`

Used by:

- `Porfolio.cshtml` profile image.

### `wwwroot/images/about_us_team.png`

Available image asset.

Current status:

- Not clearly referenced by the active pages.

## 21. Third-Party Client Libraries

### `wwwroot/lib/bootstrap`

Bootstrap CSS and JavaScript files.

Used by:

- Layout.
- Grid system.
- Buttons.
- Forms.
- Responsive utilities.
- Navbar behavior through Bootstrap bundle.

### `wwwroot/lib/jquery`

jQuery library.

Used by:

- Included globally in `_Layout.cshtml`.
- Required by jQuery validation plugins.

### `wwwroot/lib/jquery-validation`

Client-side validation library.

Used by:

- `_ValidationScriptsPartial.cshtml`.

### `wwwroot/lib/jquery-validation-unobtrusive`

Connects ASP.NET Core validation attributes to jQuery validation.

Used by:

- `_ValidationScriptsPartial.cshtml`.

## 22. Authorization Summary

Public pages:

- `/Index`
- `/Home`
- `/Login`
- `/Auth`
- `/Privacy`
- `/Error`

Logged-in user pages:

- `/About`
- `/Resume`

Admin-only pages:

- `/Dashboard`
- `/EditUser/{id}`

Contact page:

- `/Contact` does not have `[Authorize]` in its page model, but the layout only shows the Contact link after login. A user can still navigate directly to `/Contact` unless `[Authorize]` is added.

## 23. Database Table Expected by the Code

The active model expects a table named `users`.

Columns expected:

- `id`
- `name`
- `email`
- `gender`
- `role`
- `remark`
- `password_hash`

The code does not show migrations in the source tree, so the database table likely needs to exist already in MySQL.

## 24. Important Behavior Notes

- The project uses cookie authentication, not full ASP.NET Core Identity.
- The `Role` field in the database becomes the role claim in the authentication cookie.
- Admin access depends on the stored role being `Admin`.
- `Dashboard.cshtml` includes a search input, but no JavaScript or backend search logic is currently attached to it.
- The dashboard delete action immediately deletes after browser confirmation.
- The edit page preserves the password because `PasswordHash` update is commented out.
- `TempData` is used for cross-redirect messages.
- `ContactModel` sends email directly during the HTTP request.
- `Porfolio` is misspelled in file names and route, but it is consistently linked that way.

## 25. Maintenance Recommendations

These are not required for the current project to run, but they would improve quality and safety:

- Hash passwords instead of storing plain text.
- Move SMTP credentials and database passwords out of `appsettings.json`.
- Remove unused Identity and SQL Server packages if the app will continue using custom cookies and MySQL.
- Rename `Controller` folder to `Data` for clarity.
- Rename `Porfolio` to `Portfolio` if route compatibility is not important.
- Add `[Authorize]` to `ContactModel` if contact should truly require login.
- Add migrations or a SQL schema file for the `users` table.
- Add working search/filter behavior to the dashboard search box.
- Move inline JavaScript from pages into `wwwroot/js/site.js` if it becomes shared.
- Remove unused duplicate auth page if only `/Login` is needed.

## 26. Quick Run Notes

Typical local run:

```powershell
dotnet restore
dotnet run
```

Before running, MySQL should be available and the `loginappdb` database should contain the expected `users` table.

