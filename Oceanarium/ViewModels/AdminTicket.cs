using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class AdminTicket
    {
        [Required]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; }

        public List<string> DiscountOptions { get; set; } = new()
        {
        "Adult", "Child", "Student", "Senior"
        };
        public decimal TicketPrice { get; set; }

        public int TicketQuantity { get; set; }
    }
}
