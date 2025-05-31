namespace Oceanarium.ViewModels
{
    public class EventsFilter
    {
        public DateTime? Date {  get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Duration { get; set; }
        public bool EnablePriceFilter { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? TicketsAvailable { get; set; }
        public string? DescriptionSearch { get; set; }
    }
}
