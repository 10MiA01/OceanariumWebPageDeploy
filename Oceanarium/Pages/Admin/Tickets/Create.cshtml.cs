using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;
using Microsoft.EntityFrameworkCore;



namespace Oceanarium.Pages.Admin.Tickets
{
    public class CreateModel : PageModel
    {

        private readonly ApplicationDbContext _db;
        private readonly IDiscountService _discountService;

        [BindProperty]
        public Ticket newTicket { get; set; }

        public CreateModel(ApplicationDbContext db, IDiscountService discountService)
        {
            _db = db;
            _discountService = discountService;

        }
        public async Task OnGetAsync()
        {
            
        }

        public async Task<JsonResult> OnGetOrderInfoAsync(string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                return new JsonResult(new { success = false });

            // Search for order
            var order = await _db.Orders
            .Include(o => o.Tickets)
                .ThenInclude(t => t.Event) 
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order == null || !order.Tickets.Any())
                return new JsonResult(new { success = false });

            // Get Event
            var eventId = order.Tickets.First().EventId;
            var eventName = order.Tickets.First().Event.Name;

            return new JsonResult(new
            {
                success = true,
                orderId = order.Id,
                eventId = eventId,
                eventName = eventName
            });
        }

        public async Task<JsonResult> OnGetPriceInfoAsync(string ticketType, int? eventId)
        {
            if (string.IsNullOrWhiteSpace(ticketType) || eventId == null)
                return new JsonResult(new { success = false });

            // Search for 
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (ev == null)
                return new JsonResult(new { success = false, message = "Event not found" });


            var price = _discountService.CalculateDiscountedPrice(ev.Price, ticketType);

            return new JsonResult(new { success = true, price });
        }
   
        public async Task<IActionResult> OnPostCreateTicketAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //Order and event from db
            var eventEntity = await _db.Events.FindAsync(newTicket.EventId);
            var orderEntity = await _db.Orders.FindAsync(newTicket.OrderId);
            if (eventEntity == null || orderEntity == null)
            {
                ModelState.AddModelError("", "Event or Order not found.");
                return Page();
            }
            //Update all data
            _db.Tickets.Add(newTicket);
            eventEntity.MaxTickets--;
            orderEntity.TotalAmount += _discountService.CalculateDiscountedPrice(eventEntity.Price, newTicket.DiscountType);

            await _db.SaveChangesAsync();
            TempData["success"] = "Order created successfully.";
            return RedirectToPage("./Index");
        }


    }
}
