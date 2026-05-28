using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Data;
using RazorDemo.Model;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace RazorDemo.Pages
{
    public class AuthModel : PageModel
    {
        private const string AuthScheme = "Cookies";
        private readonly ApplicationDbContext _db;

        public AuthModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public string? Name { get; set; }
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            public string Password { get; set; } = string.Empty;
            public string? Gender { get; set; }
            public string? Role { get; set; }
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid) return Page();

            var confirmPassword = Request.Form["confirmPassword"].ToString();
            if (Input.Password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            var existingUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return Page();
            }

            var user = new AppUsers 
            { 
                Name = Input.Name, 
                Email = Input.Email,
                Gender = Input.Gender,
                Role = string.IsNullOrEmpty(Input.Role) ? "Student" : Input.Role,
                PasswordHash = Input.Password
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("/Auth", new { action = "login" });
        }

        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == Input.Email && u.PasswordHash == Input.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Account not found.");
                return Page();
            }

            // This saves a login cookie in the browser.
            // Name is used for "Hello! username"; Role is used for Admin pages.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? user.Email ?? "User"),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, AuthScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(AuthScheme, principal);
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(AuthScheme);
            return RedirectToPage("/Index");
        }
    }
}
