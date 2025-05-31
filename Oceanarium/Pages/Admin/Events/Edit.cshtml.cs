using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages.Admin.Events
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Event changeEvent { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGetAsync(int? eventId)
        {
            if (eventId == null || eventId == 0)
            {
                return NotFound();
            }

            changeEvent = await _db.Events.FindAsync(eventId);

            if (changeEvent == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (changeEvent.StartDate >= changeEvent.EndDate)
            {
                ModelState.AddModelError("StartDate", "Start date must be before end date.");
                return Page();
            }

            var existingEvent = await _db.Events
                .Include(e => e.Tickets)
                    .ThenInclude(t => t.Order)
                .FirstOrDefaultAsync(e => e.Id == changeEvent.Id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.Id = changeEvent.Id;
            existingEvent.Name = changeEvent.Name;
            existingEvent.Description = changeEvent.Description;
            existingEvent.StartDate = changeEvent.StartDate;
            existingEvent.EndDate = changeEvent.EndDate;
            existingEvent.Price = changeEvent.Price;
            existingEvent.MaxTicketsDefault = changeEvent.MaxTicketsDefault;
            existingEvent.MaxTickets = changeEvent.MaxTickets;
            existingEvent.Status = changeEvent.Status;
            existingEvent.Tickets = changeEvent.Tickets;


            foreach (var ticket in existingEvent.Tickets)
            {
                ticket.Status = changeEvent.Status;
            }
            var uniqueOrders = existingEvent.Tickets
                .Where(t => t.Order != null)
                .Select(t => t.Order)
                .Distinct();

            foreach (var order in uniqueOrders)
            {
                order.OrderStatus = changeEvent.Status;
            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Event updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}

