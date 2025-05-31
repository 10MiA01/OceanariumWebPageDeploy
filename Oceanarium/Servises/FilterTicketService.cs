using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;

namespace Oceanarium.Servises
{
    public class FilterTicketService : IFilterTicketService
    {
        private readonly ApplicationDbContext _db;

        public FilterTicketService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Ticket>> GetFilteredAsync(TicketFilterParams p)
        {
            var q = _db.Tickets.AsQueryable();

            if (p.Id.HasValue)
                { q = q.Where(t => t.Id == p.Id); }

            if (p.EventId.HasValue)
            { q = q.Where(t => t.EventId == p.EventId); }

            if (p.OrderId.HasValue)
            { q = q.Where(t => t.OrderId == p.OrderId); }

            if (!string.IsNullOrWhiteSpace(p.DiscountType))
                { q = q.Where(t => t.DiscountType == p.DiscountType); }

            if (p.PurchaseDate.HasValue)
                q = q.Where(t => t.PurchaseDate.Date == p.PurchaseDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(p.Status))
                { q = q.Where(t => t.Status == p.Status); }

            if (!string.IsNullOrWhiteSpace(p.TicketCode))
               { q = q.Where(t => t.TicketCode.Contains(p.TicketCode)); }

            return await q.ToListAsync();
        }
    }
}

