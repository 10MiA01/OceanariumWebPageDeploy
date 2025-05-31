using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Servises;
using Oceanarium.ViewModels;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Oceanarium.Pages.Admin.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IQrCodeCreator _qrCodeCreator;
        private readonly IDiscountService _discountService;
        private readonly IFilterOrderService _filterOrderService;

        public List<Order> _Orders { get; set; }

        public IndexModel(ApplicationDbContext db, IEmailSender emailSender, IQrCodeCreator qrCodeCreator, 
            IDiscountService discountService, IFilterOrderService filterOrderService)
        {
            _db = db;
            _emailSender = emailSender;
            _qrCodeCreator = qrCodeCreator;
            _discountService = discountService;
            _filterOrderService = filterOrderService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnGetAllOrdersAsync(
            int? id,
            string? buyerEmail,
            DateTime? dateFrom,
            DateTime? dateTo,
            int? ticketQuantityFrom,
            int? ticketQuantityTo,
            string? orderCode,
            string? orderStatus,
            int? totalAmounFrom,
            int? totalAmountTo,
            string? paymentMethod
            )
        {
            var filter = new OrderFilterParams
            {
                Id = id,
                BuyerEmail = buyerEmail,
                DateFrom = dateFrom,
                DateTo = dateTo,
                TicketQuantityFrom = ticketQuantityFrom,
                TicketQuantityTo = ticketQuantityTo,
                OrderCode = orderCode,
                OrderStatus = orderStatus,
                TotalAmountFrom = totalAmounFrom,
                TotalAmountTo = totalAmountTo,
                PaymentMethod = paymentMethod
            };

            var order = await _filterOrderService.GetFilteredAsync(filter);
            return new JsonResult(new { objOrderList = order });
        }

        public async Task<JsonResult> OnGetTicketsByOrderAsync(int orderId)
        {
            
            var order = await _db.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new JsonResult(Array.Empty<object>());

            var tickets = order.Tickets.Select(t => new
            {
                id = t.Id,
                discountType = t.DiscountType,
                status = t.Status,
                ticketPrice = _discountService.CalculateDiscountedPrice(t.Event.Price, t.DiscountType)
            });

            return new JsonResult(tickets);
        }

        public async Task<JsonResult> OnGetLoadPriceAsync(int objEv, string discountType)
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

        public async Task<IActionResult> OnGetSendEmailAsync(int? orderId)
        {

            if (orderId == null)
            {
                return NotFound();
            }

            var orderToSend = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

            if (orderToSend == null)
            {
                return NotFound();
            }

            //creating data for email

            byte[] qrCodeImage = _qrCodeCreator.GenerateQrCode(orderToSend.OrderCode);
            var cancelUrl = $"https://localhost:7102/OrderCancel?code={orderToSend.OrderCode}";

            //sending email

            await _emailSender.SendEmailAsync(
                orderToSend.BuyerEmail,
                "Order Confirmation",
                $"Your order has been placed.<br/>Total amount: {orderToSend.TotalAmount}<br/>" +
                $"You can manage your order here: <a href='{cancelUrl}'>{cancelUrl}</a><br/>" +
                $"This is your code for entrance:",
                qrCodeImage);

            
            TempData["success"] = "Order sent successfully.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetDeleteOrderAsync(int? orderId)
        {

            if (orderId == null)
            {
                return NotFound();
            }

            var toDelete = await _db.Orders
                .Include(o => o.Tickets)
                    .ThenInclude(t => t.Event)
                .FirstOrDefaultAsync(o => o.Id == orderId);


            if (toDelete == null)
            {
                return NotFound();
            }

            if (toDelete.OrderStatus != "Refunded")
            {
                TempData["warning"] = "You can delete only refunded order.";
            }
            else
            {
                foreach (var ticket in toDelete.Tickets)
                {
                    ticket.Event.MaxTickets++;
                }
                _db.Orders.Remove(toDelete);
                await _db.SaveChangesAsync();
                TempData["success"] = "Order deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}

