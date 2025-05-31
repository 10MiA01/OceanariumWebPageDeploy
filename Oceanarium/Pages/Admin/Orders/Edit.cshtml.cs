using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages.Admin.Orders
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Order changeOrder { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGetAsync(int? orderId)
        {
            if (orderId == null || orderId == 0)
            {
                return NotFound();
            }

            changeOrder = await _db.Orders
                .Include(o => o.Tickets)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (changeOrder == null)
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

            var existingOrder = await _db.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(o => o.Id == changeOrder.Id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.OrderStatus = changeOrder.OrderStatus;

            foreach (var ticket in existingOrder.Tickets)
            {
                var oldStatus = ticket.Status;
                var newStatus = changeOrder.OrderStatus;
                ticket.Status = newStatus;

                if ((oldStatus == "Cancelled" || oldStatus == "Refunded") && newStatus == "Active")
                {
                    ticket.Event.MaxTickets--;
                }
                else if (oldStatus == "Active" && (newStatus == "Cancelled" || newStatus == "Refunded"))
                {
                    ticket.Event.MaxTickets++;
                }

            }

            await _db.SaveChangesAsync();
            TempData["success"] = "Order updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}
