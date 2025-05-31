using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Oceanarium.ViewModels;
using Oceanarium.Servises.Interfaces;

namespace Oceanarium.Pages
{
    public class EventsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IFilterEventService _filterEventService;
        public List<Event> EventsList { get; set; }

        public EventsModel(ApplicationDbContext db, IFilterEventService filterEventService)
        {
            _db = db;
            _filterEventService = filterEventService;
        }


        public async Task<IActionResult> OnGetAllEventsAsync(
            string? name,
            string? description,
            DateTime? startDate,
            DateTime? endDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            int? duration,
            int? minPrice,
            int? maxPrice,
            int? ticketsAvailable)
        {
            var filter = new EventFilterParams
            {
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                TicketsAvailable = ticketsAvailable,
            };

            var events = await _filterEventService.GetFilteredAsync(filter);
            return new JsonResult(new { objEventsList = events });
        }
    }
}
