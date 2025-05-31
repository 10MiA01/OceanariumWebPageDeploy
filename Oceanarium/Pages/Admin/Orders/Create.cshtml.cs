using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;

namespace Oceanarium.Pages.Admin.Orders
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IDiscountService _discountService;
        private readonly IEmailSender _emailSender;
        private readonly IQrCodeCreator _qrCodeCreator;

        public List<Event> _EventsList { get; set; }

        [BindProperty]
        public AdminOrder _AdminOrder { get; set; }

        public CreateModel(ApplicationDbContext db, IDiscountService discountService, IEmailSender emailSender, IQrCodeCreator qrCodeCreator)
        {
            _db = db;
            _discountService = discountService;
            _emailSender = emailSender;
            _qrCodeCreator = qrCodeCreator;
        }
        public async Task OnGetAsync()
        {
            _EventsList = await _db.Events.Where(e => e.EndDate > DateTime.Now).ToListAsync();

            _AdminOrder = new AdminOrder
            {
                Tickets = new List<AdminTicket>
                {
                new AdminTicket()
                }
            };
        }

        public async Task<JsonResult> OnGetUpdatePriceAsync(int objEv, string discountType)
        {

            Console.WriteLine("Json fetch is working");
            var selectedEvent = await _db.Events.FirstOrDefaultAsync(u => u.Id == objEv);
            if (selectedEvent == null)
            {
                // If not found
                Console.WriteLine("Event not fount");
                return new JsonResult(new { price = 0 });
            }

            decimal basePrice = selectedEvent.Price;
            decimal finalPrice = _discountService.CalculateDiscountedPrice(basePrice, discountType);

            return new JsonResult(new { price = finalPrice });
        }

        public async Task<IActionResult> OnPostCreateOrderAsync()
        {
            _EventsList = await _db.Events.Where(e => e.EndDate > DateTime.Now).ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var eventObj = _db.Events.FirstOrDefault(e => e.Id == _AdminOrder.EventId);
            if (eventObj == null)
            {
                ModelState.AddModelError("", "Event not found.");
                return Page();
            }

            //not nessasary cause of UX, but let's check it anyway
            int ticketsSelected = 0;
            foreach (var ticket in _AdminOrder.Tickets)
            {
                Console.WriteLine($"{ticket.TicketQuantity} is added to counter");
                ticketsSelected += ticket.TicketQuantity;
            }
            if (eventObj.MaxTickets < ticketsSelected)
            {
                ModelState.AddModelError("", "Not enough tickets available for this event.");
                return Page();
            }

            //Create order
            var order = new Order
            {
                BuyerEmail = _AdminOrder.BuyerEmail,
                PaymentMethod = _AdminOrder.PaymentMethod,
                TotalAmount = _AdminOrder.TotalAmount,
                CreatedAt = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString("N").Substring(0, 8),
                OrderStatus = "Active"
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            //Add tickets
            foreach (var ticketImput in _AdminOrder.Tickets)
            {
                Console.WriteLine($"Creating tickets in DB");
                while (ticketImput.TicketQuantity > 0)
                {
                    Console.WriteLine($"{ticketImput.TicketQuantity} is creating tickets");
                    var ticket = new Ticket
                    {
                        EventId = eventObj.Id,
                        OrderId = order.Id,
                        DiscountType = ticketImput.DiscountType,
                    };

                    Console.WriteLine($"{ticket} is added ro db");
                    _db.Tickets.Add(ticket);
                    ticketImput.TicketQuantity--;
                }
            }

            eventObj.MaxTickets -= ticketsSelected;
            await _db.SaveChangesAsync();

            TempData["success"] = "Order created successfully.";
            _EventsList = _db.Events.ToList();
            return RedirectToPage("./Index");
        }

    }
}
