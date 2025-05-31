using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class AdminOrder
    {
        [Required]
        [Display(Name = "Select Event")]
        public int EventId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Your Email")]
        public string BuyerEmail { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public List<AdminTicket> Tickets { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
