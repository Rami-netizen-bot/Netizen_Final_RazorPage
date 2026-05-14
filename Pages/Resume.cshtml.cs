using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authorization;

namespace RazorDemo.Pages
{
    [Authorize]
    public class ResumeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
