using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.ViewModels;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Servises;


namespace Oceanarium.Pages.Admin.Tickets
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IDiscountService _discountService;
        private readonly IFilterTicketService _filterTicketService;


        public List<Ticket> _Tickets { get; set; }


        public IndexModel(ApplicationDbContext db, IDiscountService discountService, IFilterTicketService filterTicketService)
        {
            _db = db;
            _discountService = discountService;
            _filterTicketService = filterTicketService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _Tickets = await _db.Tickets.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetAllTicketsAsync(
            int? id,
            int? eventId,
            int? orderId,
            string? discountType,
            DateTime? purchaseDate,
            string? status,
            string? ticketCode)
        {
            var filter = new TicketFilterParams
            {
                Id = id,
                EventId = eventId,
                OrderId = orderId,
                DiscountType = discountType,
                PurchaseDate = purchaseDate,
                Status = status,
                TicketCode = ticketCode
            };

            var tickets = await _filterTicketService.GetFilteredAsync(filter);
            return new JsonResult(new { objTicketsList = tickets });
        }


        public async Task<IActionResult> OnGetDeleteTicketAsync(int? ticketId)
        {

            if (ticketId == null)
            {
                return NotFound();
            }

            var toDelete = await _db.Tickets
                .Include(o => o.Order)
                .Include(t => t.Event)
                .FirstOrDefaultAsync(o => o.Id == ticketId);


            if (toDelete == null)
            {
                return NotFound();
            }

            if (toDelete.Status != "Refunded")
            {
                TempData["warning"] = "You can delete only refunded tickets.";
            }
            else
            {
                toDelete.Order.TotalAmount -= _discountService.CalculateDiscountedPrice(toDelete.Event.Price, toDelete.DiscountType);
                toDelete.Event.MaxTickets++;
                _db.Tickets.Remove(toDelete);
                await _db.SaveChangesAsync();
                TempData["success"] = "Ticket cancelled successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}

