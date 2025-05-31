using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class ContactForm
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress] 
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
