using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using System.Linq;


namespace Oceanarium.Pages.Admin.Tickets
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;


        [BindProperty]
        public Ticket changeTicket { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGetAsync(int? ticketId)
        {
            if (ticketId == null || ticketId == 0)
            {
                return NotFound();
            }

            changeTicket = await _db.Tickets.FindAsync(ticketId);
  

            if (changeTicket == null)
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

            var existingTicket = await _db.Tickets
        .Include(t => t.Event)
        .Include(t => t.Order)
            .ThenInclude(o => o.Tickets)
        .FirstOrDefaultAsync(t => t.Id == changeTicket.Id);

            if (existingTicket == null)
            {
                return NotFound();
            }

            //Changing tickets number
            var oldStatus = existingTicket.Status;
            var newStatus = changeTicket.Status;
            existingTicket.Status = changeTicket.Status;
            if (oldStatus == "Active" && (newStatus == "Cancelled" || newStatus == "Refunded"))
            {
                existingTicket.Event.MaxTickets++;
            }
            else if(newStatus == "Active" && (oldStatus == "Cancelled" || oldStatus == "Refunded"))
            {
                existingTicket.Event.MaxTickets--;
            }

            //Check if order is still active
            bool orderIsActive = existingTicket.Order.Tickets.Any(t => t.Status == "Active");

            existingTicket.Order.OrderStatus = orderIsActive
                ? "Active"
                : existingTicket.Order.Tickets.All(t => t.Status == "Refunded") ? "Refunded" : "Cancelled";


            await _db.SaveChangesAsync();
            TempData["success"] = "Order updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}
