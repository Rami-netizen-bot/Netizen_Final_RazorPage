using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; // Ensures List works

namespace MyApp.Namespace
{
    public class PorfolioModel : PageModel
    {
        // 1. Basic properties
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MyStory { get; set; } = string.Empty;
        public string CurrentFocus { get; set; } = string.Empty;
        //Propety

        public ContactDetails Contact { get; set; } = new ContactDetails();
       public List<SkillGroup> Skills { get; set; } = new List<SkillGroup>();
    
        public EducationDetails Education { get; set; } = new EducationDetails();
        public EducationDetails2 Education2 { get; set; } = new EducationDetails2();
        public EducationDetails3 Education3 { get; set; } = new EducationDetails3();
        public EducationDetail1 Education1 {get; set;} = new EducationDetail1();
        public List<LanguageDetail> Language {get; set;} = new List<LanguageDetail>();

        public void OnGet()
        {
            Name = "Ramy";
            Title = "Student of Computer Sciences";
            MyStory = "lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            CurrentFocus = "lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
          
            Contact = new ContactDetails
            {
                Email = "ramyman030805@gmail.com",
                Phone = "069 91 00 33",
                FacebookUrl = "#"
            };

       
            Skills = new List<SkillGroup>
            {
                new SkillGroup { Category = "Programming Languages", Technologies = "No Experience" },
                new SkillGroup { Category = "Web Development", Technologies = "No Experience" },
                new SkillGroup { Category = "Databases", Technologies = "No Experience" }
            };
           
            
              Education = new EducationDetails{
               
                Primary = "Phnom Toch Primary School",
                Year = "2012 - 2016",
            };
            Education2 = new EducationDetails2{
                HightSchool = "Ang Andet High School",
                Year = "2017 - 2023",
            };
            Education3 = new EducationDetails3{
                University = "Chenla University",
                Year = "2024 - Present",
            };
            Education1 = new EducationDetail1
            {
                Center = "TCEC Center",
                Year = "2009 - 2017",
            };
           Language = new List<LanguageDetail>
           {
               new LanguageDetail {Subject = "Khmer", Level = "Monder Toung"},
               new LanguageDetail {Subject = "English", Level = "Low"},
           };
       

        }
    }

    public class ContactDetails
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
    }

    public class SkillGroup
    {
        public string Category { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
    }
    public class EducationDetails
    {
        public string Degree { get; set; } = string.Empty;
        public string Primary { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
    public class EducationDetails2
    {
        public string Degree { get; set; } = string.Empty;
        public string HightSchool { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
    public class EducationDetails3
    {
        public string Degree { get; set; } = string.Empty;
        public string University { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
    public class EducationDetail1
    {
        public string Degree {get; set;} = string.Empty;
        public string Center {get; set; } = string.Empty;
        public string Year {get; set;} = string.Empty;
    }
    public class LanguageDetail
    {
        public string Subject {get; set;} = string.Empty;
        public string Level {get; set;} = string.Empty;
    }
}