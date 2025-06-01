using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.ViewModels;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Servises;

namespace Oceanarium.Pages
{
    public class TicketsModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IDiscountService _discountService;
        private readonly IEmailSender _emailSender;
        private readonly IQrCodeCreator _qrCodeCreator;
        public List<Event> EventsList { get; set; }

        [BindProperty]
        public TicketsOrder TicketsOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EventId { get; set; }

        public TicketsModel(ApplicationDbContext db, IDiscountService discountService, IEmailSender emailSender, IQrCodeCreator qrCodeCreator)
        {
            _db = db;
            _discountService = discountService;
            _emailSender = emailSender;
            _qrCodeCreator = qrCodeCreator;
        }



        public void OnGet()
        {
            EventsList = _db.Events.ToList();





            TicketsOrder = new TicketsOrder
            {
                Tickets = new List<TicketImput>
                {
                new TicketImput() 
                }
            };
        }

        public async  Task<JsonResult> OnGetUpdatePrice(int objEv, string discountType)
        {

            var selectedEvent = await _db.Events.FirstOrDefaultAsync(u => u.Id == objEv);
            if(selectedEvent == null)
            {
                // If not found
                return new JsonResult(new { price = 0 });
            }

            decimal basePrice = selectedEvent.Price;
            decimal finalPrice = _discountService.CalculateDiscountedPrice(basePrice, discountType);

            return new JsonResult(new { price = finalPrice });
        }



        public async Task<IActionResult> OnPostCreateOrder()
        {

            Console.WriteLine($"EventId selected: {TicketsOrder.EventId}");


            EventsList = _db.Events.ToList();
            if (!ModelState.IsValid)
            {
                // if model is invalid, load events list
                EventsList = _db.Events.ToList();
                return Page();
            }


            var eventObj = _db.Events.FirstOrDefault(e => e.Id == TicketsOrder.EventId);
            if (eventObj == null)
            {
                ModelState.AddModelError("", "Event not found.");
                return Page();
            }
            else
            {
                Console.WriteLine($"Event found: {eventObj.Name}, MaxTickets: {eventObj.MaxTickets}");
            }

            if (eventObj.MaxTickets < TicketsOrder.Tickets.Count)
            {
                ModelState.AddModelError("", "Not enough tickets available for this event.");
                return Page();
            }

            // Total price 
            decimal totalAmount = 0;
            foreach (var ticketInput in TicketsOrder.Tickets)
            {
                var selectedEvent = _db.Events.FirstOrDefault(e => e.Id == TicketsOrder.EventId);
                decimal ticketPrice = _discountService.CalculateDiscountedPrice(selectedEvent.Price, ticketInput.DiscountType);

                totalAmount += ticketPrice;  
            }

            
            TicketsOrder.TotalAmount = totalAmount;

            //Create order
            var order = new Order
            {
                BuyerEmail = TicketsOrder.BuyerEmail,
                PaymentMethod = TicketsOrder.PaymentMethod,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.Now,
                OrderCode = Guid.NewGuid().ToString("N").Substring(0, 8),
                OrderStatus = "Active"
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var ticketImput in TicketsOrder.Tickets)
            {
                var ticket = new Ticket
                {
                    EventId = eventObj.Id,
                    DiscountType = ticketImput.DiscountType,
                    OrderId = order.Id
                };

                _db.Tickets.Add(ticket);
            }

            eventObj.MaxTickets -= TicketsOrder.Tickets.Count;
            await _db.SaveChangesAsync();

            //creating data for email

            byte[] qrCodeImage = _qrCodeCreator.GenerateQrCode(order.OrderCode);

            //sending email

            await _emailSender.SendEmailAsync(
                TicketsOrder.BuyerEmail,
                "Order Confirmation",
                $"Your order has been placed.<br/>Total amount: {TicketsOrder.TotalAmount}<br/>" +
                $"This is your code for entrance:",
                EmailMessageType.OrderConfirmation,
                qrCodeImage,
                order.OrderCode);

            TempData["success"] = "Your order has been placed.";
            EventsList = _db.Events.ToList();
            return RedirectToPage();

        }
    }
}
 