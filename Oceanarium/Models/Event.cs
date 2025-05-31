using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Net.Sockets;
using Oceanarium.Validations;

namespace Oceanarium.Models
{

    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate {  get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
        [MinValue(0, ErrorMessage = "Price cannot be less than 0")]
        public decimal Price { get; set; }

        [MinValue(0, ErrorMessage = "MaxTickets cannot be less than 0")]
        public int MaxTicketsDefault { get; set; }

        [MinValue(0, ErrorMessage = "MaxTickets cannot be less than 0")]
        public int MaxTickets { get; set; }

        [StatusIsValid]
        public string Status { get; set; } = "Active"; // "Active", "Cancelled", "Refunded", "Finished" 


        [InverseProperty("Event")]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
