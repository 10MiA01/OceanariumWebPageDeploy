using MailKit.Search;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.ViewModels;
using Oceanarium.Servises.Interfaces;

namespace Oceanarium.Pages
{
    public class OrderCancelModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IDiscountService _discountService;

        //From _db
        public Event? Event { get; set; }
        public Order? Order { get; set; }
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();

        //ViewModels

        [BindProperty]
        public OrderCancel OrderCancel { get; set; }

        public List<TicketCancel> TicketCancels { get; set; } = new List<TicketCancel>();

        public OrderCancelModel(ApplicationDbContext db, IDiscountService discountService)
        {
            _db = db;
            _discountService = discountService;

        }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFound();
            }
            var now = DateTime.UtcNow;

            var order = await _db.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Event)
                .Where(o => o.OrderCode == code && o.Tickets.Any(t => t.Event.EndDate >= now))
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return RedirectToPage("OldOrder");
            }
            

            Order = order;
            Tickets = Order.Tickets.ToList();
            Event = Tickets.FirstOrDefault()?.Event;


            if (Event == null)
            {
                return NotFound();
            }

            TicketCancels = Tickets.Select(t => new TicketCancel
            {
                Id = t.Id,
                DiscountType = t.DiscountType,
                Status = t.Status,
                TicketPrice = _discountService.CalculateDiscountedPrice(Event.Price, t.DiscountType),
            }).ToList();

            OrderCancel = new OrderCancel
            {
                OrderId = Order.Id,
                OrderCode = Order.OrderCode,
                OrderStatus = Order.OrderStatus,
                OrderDate = Order.CreatedAt,
                TotalAmount = Order.TotalAmount,
                EventId = Event.Id,
                EventName = Event.Name,
                StartDate = Event.StartDate,
                EndDate = Event.EndDate,
                Tickets = TicketCancels,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostCancelTicketAsync(int ticketId, string code)
        {
            var toCancel = await _db.Tickets
                .Include(t => t.Event)
                .Include(t => t.Order)
                    .ThenInclude(o => o.Tickets)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
            if (toCancel == null)
            {
                return NotFound();
            }
            toCancel.Status = "Cancelled";

            var EventToUpdate = toCancel.Event;
            EventToUpdate.MaxTickets++;


            await _db.SaveChangesAsync();


            var orderTickets = toCancel.Order.Tickets;
            if (orderTickets != null && orderTickets.All(t => t.Status == "Cancelled" || t.Status == "Refunded"))
            {
                toCancel.Order.OrderStatus = "Cancelled";
                await _db.SaveChangesAsync();
                TempData["success"] = "Order cancelled successfully. We will contact you within a week about the rufundation";

            }
            else
            {
                TempData["success"] = "Ticket cancelled successfully. We will contact you within a week about the rufundation";

            }
            return RedirectToPage("OrderCancel", new { code = code });

        }


        public async Task<IActionResult> OnPostCancelOrderAsync(int orderId, string code)
        {
            var toCancelOrder = await _db.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (toCancelOrder == null)
            {
                return NotFound();
            }
            foreach (var ticket in toCancelOrder.Tickets)
            {
                ticket.Status = "Cancelled";
                ticket.Event.MaxTickets++;
            }
            toCancelOrder.OrderStatus = "Cancelled";

            await _db.SaveChangesAsync();

            TempData["success"] = "Order cancelled successfully. We will contact you within a week about the rufundation";

            return RedirectToPage("OrderCancel", new { code = code });
        }
    }


}
