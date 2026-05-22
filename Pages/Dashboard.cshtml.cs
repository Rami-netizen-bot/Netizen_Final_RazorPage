using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Data;
using RazorDemo.Model;
using Microsoft.AspNetCore.Authorization;

namespace RazorDemo.Pages
{
    /// <summary>
    /// Administrative Dashboard - Restricted to Admin role only
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DashboardModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Properties to hold dashboard data
        public List<AppUsers> Users { get; set; } = new();
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int StudentCount { get; set; }

        /// <summary>
        /// Loads all users and calculates summary statistics
        /// </summary>
        public async Task OnGetAsync()
        {
            // Fetch all users from the database
            Users = await _db.AppUsers.ToListAsync();
            TotalUsers = Users.Count;

            // Calculate counts using case-insensitive role matching
            AdminCount = Users.Count(u => string.Equals(u.Role, "Admin", StringComparison.OrdinalIgnoreCase));
            
            // Treat Null or Empty roles as "Student" for fallback counting
            StudentCount = Users.Count(u => string.IsNullOrEmpty(u.Role) || string.Equals(u.Role, "Student", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Handles user deletion request from the dashboard table
        /// </summary>
        /// <param name="id">The ID of the user to remove</param>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _db.AppUsers.FindAsync(id);
            if (user != null)
            {
                _db.AppUsers.Remove(user);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
