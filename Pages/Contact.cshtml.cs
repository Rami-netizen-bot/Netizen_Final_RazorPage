using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authorization;

namespace RazorDemo.Pages
{
    [Authorize]
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
