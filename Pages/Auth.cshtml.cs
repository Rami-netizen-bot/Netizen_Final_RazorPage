using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Data;
using RazorDemo.Models;
using System.Security.Claims;

namespace RazorDemo.Pages
{
    public class AuthModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AuthModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("AppCookie");
            return RedirectToPage("/Home");
        }

        public async Task<IActionResult> OnPostLoginAsync(string email, string password)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "AppCookie");
            await HttpContext.SignInAsync("AppCookie", new ClaimsPrincipal(identity));

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostRegisterAsync(string fullName, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Password and confirm password do not match.");
                return Page();
            }

            if (await _context.AppUsers.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(string.Empty, "This email is already registered.");
                return Page();
            }

            var newUser = new User
            {
                Name = fullName,
                Email = email,
                PasswordHash = password,
                Gender = "Other",
                Role = "User"
            };

            _context.AppUsers.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful. Please log in.";
            return RedirectToPage("/Login");
        }
    }
}
