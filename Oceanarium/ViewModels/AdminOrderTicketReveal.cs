using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class AdminOrderTicketReveal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Discount Type")]
        public string DiscountType { get; set; }

        public string Status { get; set; }
       
        public decimal TicketPrice { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
