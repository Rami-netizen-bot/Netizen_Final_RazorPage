using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorDemo.Pages
{
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> _logger;
        public string BadgeText{get; set;} =string.Empty;
        public string MainHeading{get; set;} = string.Empty;
        public string HightlightedText{get;set;} = string.Empty;
        public string Subtitle {get;set;} = string.Empty;
        public string GitHubUrl {get; set; } =string.Empty;
        public string TelegramUrl {get; set; } = string.Empty;

        public HomeModel (ILogger<HomeModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            BadgeText = "Final Exam Project Razor Page .Net C#";
            MainHeading = "Learn to build";
            HightlightedText = "real-world";
            Subtitle = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            GitHubUrl ="https://github.com/Rami-netizen-bot/Netizen_Final_RazorPage.git";
            TelegramUrl = "https://t.me/Ramy_Mann"; 
        }
    }
}
