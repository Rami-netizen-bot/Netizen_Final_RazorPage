using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "AppCookie";
        options.DefaultChallengeScheme = "AppCookie";
        options.DefaultSignInScheme = "AppCookie";
    })
    .AddCookie("AppCookie", options =>
    {
        options.LoginPath = "/Auth";
        options.LogoutPath = "/Auth?handler=Logout";
    });

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.ExecuteSqlRaw("""
        IF OBJECT_ID(N'[AppUsers]', N'U') IS NULL
        BEGIN
            CREATE TABLE [AppUsers] (
                [Id] int NOT NULL IDENTITY,
                [Name] nvarchar(max) NOT NULL,
                [Email] nvarchar(max) NOT NULL,
                [Gender] nvarchar(max) NOT NULL,
                [Role] nvarchar(max) NOT NULL,
                [Remark] nvarchar(max) NULL,
                [PasswordHash] nvarchar(max) NOT NULL,
                CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
            );
        END
        """);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
