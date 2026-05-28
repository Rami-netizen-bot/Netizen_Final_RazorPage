using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace RazorDemo.Pages
{
    [Authorize]
    public class AboutModel : PageModel
    {
        public string ProfileName {get; set; } = string.Empty;
        public string ProfileImage{get;set;} = string.Empty;
        public string Subtitile {get;set;}= string.Empty;
        public List<string> MainTags {get;set;} = new List<string>();
        public string Email {get;set;} = string.Empty;
         public string PhoneNumber {get; set;} = string.Empty;
         public string FacebookUrl {get; set;} = string.Empty;
         public string Mystory {get;set;} = string.Empty;
         public string CurrentFocus {get; set;} = string.Empty;
         public List<SkillGroup> Skillgroups {get;set;} = new List<SkillGroup>();

        public void OnGet()
        {
            ProfileName = "Ramy Man";
            ProfileImage = "/images/photo_2025-10-31_11-27-40.jpg";
            Subtitile = "Student of Computer Sciences";
            MainTags = new List<string> {"Flutter" , "CSS", "JavaScript", "HTML"};
            Email = "ramyman0308005@gmail.com";
            PhoneNumber = "069910033";
            FacebookUrl = "https://facebook.com";
            Mystory = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            CurrentFocus = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

            Skillgroups = new List<SkillGroup>
            {
                new SkillGroup {Title = "Frontend Skills" , Teachnologies = "HTML , CSS , BotStrap, JQuery"},
                new SkillGroup {Title = "Backend Skills" , Teachnologies = "MySQL , PHP, .Net Core"},
                new SkillGroup {Title = "Mobile Skills" , Teachnologies = "Flutter/Flutter , Firebase"}
            };


        }
        public class SkillGroup
        {
            public string Title {get;set;} = string.Empty;
            public string Teachnologies {get; set;} = string.Empty;
        }
    }
}
