using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Authorization;

namespace RazorDemo.Pages
{
    [Authorize]
    public class ResumeModel : PageModel
    {
        public string Name {get; set;} = string.Empty;
        public string Title {get; set; } =string.Empty;
        public string Email {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;
        public string Location {get; set;} = string.Empty;
        public string Summary {get; set;} = string.Empty;

        public List<string> AdditinalDetails {get; set;}  = new List<string>();

        public List<string> TechnicalSkill {get; set;} = new List<string>();
        public List<string> Softskill {get; set;} = new List<string>();

      
       
        public void OnGet ()
        {
                Name = "Ramy";
                Title = "Student of Teacher Symon";
                Email = "ramyman030805@gmail.com";
                PhoneNumber = "069910033";
                Location = "Phnom Penh Camobodia";
                Summary = "Lorem ipsum dolor sit amet consectetur adipisicing elit. " +
                          "Doloremque praesentium ratione sed in quis autem et placeat vel?";

                AdditinalDetails = new List<string>
                {
                    "Lorem ipsum dolor sit amet consectetur adipisicing elit.",
                    "Lorem ipsum dolor sit amet consectetur adipisicing elit.",
                    "Lorem ipsum dolor sit amet consectetur."
                };
                TechnicalSkill = new List<string>
                {
                    "Flutter", "Figma", "HTML", "CSS", "JQery","BootStrap","Git"
                };
                
                Softskill = new List<string>
                {
                    "Friendly", "Teamwork", "Honesty", "Discipline", "Responsibility", "Communication"
                };
            }
        }
    }

