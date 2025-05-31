using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises
{
    public class FilterOrderService : IFilterOrderService
    {
        private readonly ApplicationDbContext _db;

        public FilterOrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Order>> GetFilteredAsync(OrderFilterParams p)
        {
            var q = _db.Orders.AsQueryable();

            if (p.Id.HasValue)
            { q = q.Where(t => t.Id == p.Id); }

            if (!string.IsNullOrWhiteSpace(p.BuyerEmail))
            { q = q.Where(t => t.BuyerEmail == p.BuyerEmail); }

            if (p.DateFrom.HasValue)
                q = q.Where(t => t.CreatedAt.Date >= p.DateFrom.Value.Date);

            if (p.DateTo.HasValue)
                q = q.Where(t => t.CreatedAt.Date <= p.DateTo.Value.Date);

            if (p.TicketQuantityFrom.HasValue)
            { q = q.Where(t => t.Tickets.Count >= p.TicketQuantityFrom); }

            if (p.TicketQuantityTo.HasValue)
            { q = q.Where(t => t.Tickets.Count <= p.TicketQuantityTo); }

            if (!string.IsNullOrWhiteSpace(p.OrderCode))
            { q = q.Where(t => t.OrderCode == p.OrderCode); }

            if (!string.IsNullOrWhiteSpace(p.OrderStatus))
            { q = q.Where(t => t.OrderStatus.Contains(p.OrderStatus)); }

            if (p.TotalAmountFrom.HasValue)
            { q = q.Where(t => t.TotalAmount >= p.TotalAmountFrom); }

            if (p.TotalAmountTo.HasValue)
            { q = q.Where(t => t.TotalAmount <= p.TotalAmountTo); }

            if (!string.IsNullOrWhiteSpace(p.PaymentMethod))
            { q = q.Where(t => t.PaymentMethod == p.PaymentMethod); }

            return await q.ToListAsync();
        }
    }
}
