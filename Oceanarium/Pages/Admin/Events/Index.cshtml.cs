using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;

namespace Oceanarium.Pages.Admin.Events
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Event> _Events;
        private readonly IFilterEventService _filterEventService;

        public IndexModel(ApplicationDbContext db, IFilterEventService filterEventService)
        {
            _db = db;
            _filterEventService = filterEventService;
        }

        public async Task<IActionResult> OnGetAllEventsAsync(
            int? id,
            string? name,
            string? description,
            DateTime? startDate,
            DateTime? endDate,
            TimeSpan? startTime,
            TimeSpan? endTime,
            int? duration,
            int? minPrice,
            int? maxPrice,
            int? ticketsMaxMin,
            int? ticketsMaxMax,
            int? ticketsAvailable,
            string? status)
        {
            var filter = new EventFilterParams
            {
                Id = id,
                Name = name,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                TicketsMaxMin = ticketsMaxMin,
                TicketsMaxMax = ticketsMaxMax,
                TicketsAvailable = ticketsAvailable,
                Status = status
            };

            var events = await _filterEventService.GetFilteredAsync(filter);
            return new JsonResult(new { objEventsList = events });
        }


        public async Task<IActionResult> OnGetDeleteEventAsync(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            Event toDelete = await _db.Events.FindAsync(eventId);
            if (toDelete == null)
            {
                return NotFound();
            }
            if ((toDelete.Status != "Finished") || (toDelete.Status != "Refunded"))
            {
                TempData["warning"] = "You can delete only refunded or finished event.";
            }
            else
            {
                _db.Events.Remove(toDelete);
                await _db.SaveChangesAsync();
                TempData["success"] = "Event deleted successfully.";
            }

            return RedirectToPage("./Index");
        }

    }
}

