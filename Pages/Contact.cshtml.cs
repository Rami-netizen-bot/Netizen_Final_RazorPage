using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RazorDemo.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // CRITICAL: This must be the ONLY constructor in this class!
        public ContactModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var port = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var appPassword = _configuration["EmailSettings:AppPassword"];

                var mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, senderName);
                mail.To.Add("ramyman030805@gmail.com");
                mail.Subject = $"New Portfolio Message from {Name}";
                mail.Body = $"<h3>New Contact Form Submission</h3>" +
                            $"<p><strong>Name:</strong> {Name}</p>" +
                            $"<p><strong>Sender Email:</strong> {Email}</p>" +
                            $"<p><strong>Message:</strong><br/>{Message}</p>";
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(smtpServer, port))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }

                StatusMessage = "Your message has been sent successfully!";
                return RedirectToPage();
            }
            catch (System.Exception)
            {
                StatusMessage = "Error: System failed to send email. Check your configuration credentials.";
                return Page();
            }
        }
    }
}