namespace Oceanarium.ViewModels
{
    public class TicketFilterParams
    {
        public int? Id { get; set; }
        public int? EventId { get; set; }
        public int? OrderId { get; set; }
        public string? DiscountType { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Status { get; set; }
        public string? TicketCode { get; set; }
    }
}
