using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Oceanarium.Validations;

namespace Oceanarium.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Event")]
        public int EventId { get; set; }

        [ValidateNever]
        [InverseProperty("Tickets")]
        public Event Event { get; set; }


        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [ValidateNever]
        [InverseProperty("Tickets")]
        public Order Order { get; set; }

        [Required]
        [DiscountIsValid(ErrorMessage = "Invalid discount type")]
        public string DiscountType { get; set; } // "Adult", "Child", "Student", "Senior"


        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;


        [StatusIsValid]
        public string Status { get; set; } = "Active"; // "Active", "Cancelled", "Refunded"


        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N"); // for cancealling

    }
}
