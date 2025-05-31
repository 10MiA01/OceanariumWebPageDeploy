using System.ComponentModel.DataAnnotations;

namespace Oceanarium.ViewModels
{
    public class OrderCancel
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string OrderCode { get; set; }
        [Required]
        public string OrderStatus { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public int EventId { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public List<TicketCancel> Tickets { get; set; }

    }
}
