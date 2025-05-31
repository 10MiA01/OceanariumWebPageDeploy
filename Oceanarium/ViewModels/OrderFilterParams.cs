using Oceanarium.Models;

namespace Oceanarium.ViewModels
{
    public class OrderFilterParams
    {
        public int? Id { get; set; }
        public string? BuyerEmail { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? TicketQuantityFrom { get; set; }
        public int? TicketQuantityTo { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderStatus { get; set; }
        public decimal? TotalAmountFrom { get; set; }
        public decimal? TotalAmountTo { get; set; }
        public string? PaymentMethod { get; set; }


    }
}
