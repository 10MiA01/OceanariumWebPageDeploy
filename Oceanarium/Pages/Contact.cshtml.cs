using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Servises;
using Oceanarium.ViewModels;

namespace Oceanarium.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ContactForm contactForm { get; set; }

        public ContactModel(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var htmlContent = $"<p><strong>From: </strong></p> {contactForm.Name} <p><strong>Email adress: </strong></p> {contactForm.Email} " +
                $"<p><strong>Message:</strong><br>{contactForm.Message}</p>";

            await _emailSender.SendEmailAsync("workingmail@gmail.com", contactForm.Subject, htmlContent);

            TempData["success"] = "Your message has been sent successfully!";

            return RedirectToPage();
        }
    }
}
