namespace Oceanarium.ViewModels
{
    public class EventFilterParams
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? Duration { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? TicketsMaxMin { get; set; }
        public int? TicketsMaxMax { get; set; }
        public int? TicketsAvailable { get; set; }
        public string? Status { get; set; }

    }
}
