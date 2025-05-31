using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oceanarium.Validations;

namespace Oceanarium.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Buyer email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string BuyerEmail { get; set; }


        public DateTime CreatedAt { get; set; } 


        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();


        [Required]
        public string OrderCode { get; set; } // for cancelling

        [StatusIsValid]
        public string OrderStatus { get; set; }  // "Active", "Cancelled", "Refunded", "Finished"
        
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        
        public string PaymentMethod {  get; set; }

    
    
    }
}
