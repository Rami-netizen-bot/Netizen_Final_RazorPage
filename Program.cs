using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RazorDemo.Data;
using RazorDemo.Model;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup MySQL Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Register Cookie Authentication instead of Identity
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login"; // Redirects here if not logged in
        options.AccessDeniedPath = "/Index";
        options.Cookie.Name = "UserLoginCookie";
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();