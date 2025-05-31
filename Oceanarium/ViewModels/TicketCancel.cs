using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class TicketCancel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string DiscountType { get; set; }

        [Required]
        public string Status { get; set; }
        [Required]
        public decimal TicketPrice { get; set; }

    }

}

