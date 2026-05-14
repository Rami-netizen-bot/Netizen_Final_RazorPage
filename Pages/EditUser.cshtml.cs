using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Data;
using RazorDemo.Model;
using Microsoft.AspNetCore.Authorization;

namespace RazorDemo.Pages
{
    /// <summary>
    /// Logic for editing an existing user profile (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EditUserModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditUserModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public AppUsers UserToEdit { get; set; }

        /// <summary>
        /// Retrieves the user data to populate the edit form
        /// </summary>
        /// <param name="id">The ID of the user to edit</param>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            UserToEdit = await _db.AppUsers.FindAsync(id);

            if (UserToEdit == null)
            {
                return RedirectToPage("/Dashboard");
            }

            return Page();
        }

        /// <summary>
        /// Saves the modified user data back to the database
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch current record from DB to ensure it exists
            var userInDb = await _db.AppUsers.FindAsync(UserToEdit.Id);

            if (userInDb == null)
            {
                return NotFound();
            }

            // Update only specific allowed fields
            userInDb.Name = UserToEdit.Name;
            userInDb.Email = UserToEdit.Email;
            userInDb.Gender = UserToEdit.Gender;
            userInDb.Role = UserToEdit.Role;
            userInDb.Remark = UserToEdit.Remark;

            // Preserve password if it's not being changed (or handle password update logic here)
            // userInDb.PasswordHash = UserToEdit.PasswordHash; 

            await _db.SaveChangesAsync();

            // Store success message for the next page load
            TempData["SuccessMessage"] = "User updated successfully!";

            return RedirectToPage("/Dashboard");
        }
    }
}
