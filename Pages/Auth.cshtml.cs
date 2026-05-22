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
    /// <summary>
    /// Handles User Authentication (Login, Registration, Logout)
    /// </summary>
    public class AuthModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AuthModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        /// <summary>
        /// Data Transfer Object for authentication forms
        /// </summary>
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

        public void OnGet()
        {
        }

        /// <summary>
        /// Logic for creating a new user account
        /// </summary>
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Check if passwords match
            var confirmPassword = Request.Form["confirmPassword"].ToString();
            if (Input.Password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }

            // Ensure email uniqueness
            var existingUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email already exists.");
                return Page();
            }

            // Create new user object with default role if none provided
            var user = new AppUsers 
            { 
                Name = Input.Name, 
                Email = Input.Email,
                Gender = Input.Gender,
                Role = string.IsNullOrEmpty(Input.Role) ? "Student" : Input.Role,
                PasswordHash = Input.Password // Stored as plain text per project requirement
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToPage("/Login", new { action = "login" });
        }

        /// <summary>
        /// Logic for authenticating an existing user
        /// </summary>
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Find user with matching email and password
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == Input.Email && u.PasswordHash == Input.Password);
            
            if (user != null)
            {
                // Create identity claims (Name, Email, Role)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user using Cookie Authentication
                await HttpContext.SignInAsync("Cookies", principal);

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "This Account is not find .");
            return Page();
        }

        /// <summary>
        /// Logic for signing out the user
        /// </summary>
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToPage("/Index");
        }
    }
}