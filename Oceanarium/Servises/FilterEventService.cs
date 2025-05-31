using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Oceanarium.Servises
{
    public class FilterEventService : IFilterEventService
    {
        private readonly ApplicationDbContext _db;

        public FilterEventService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Event>> GetFilteredAsync(EventFilterParams p)
        {
            var q = _db.Events.AsQueryable();

            if (p.Id.HasValue)
            { q = q.Where(t => t.Id == p.Id); }

            if (!string.IsNullOrWhiteSpace(p.Name))
            { q = q.Where(t => t.Name.Contains(p.Name)); }

            if (!string.IsNullOrWhiteSpace(p.Description))
            { q = q.Where(t => t.Description.Contains(p.Description)); }

            if (p.StartDate.HasValue)
                q = q.Where(t => t.StartDate.Date >= p.StartDate.Value.Date);

            if (p.EndDate.HasValue)
                q = q.Where(t => t.StartDate.Date <= p.EndDate.Value.Date);

            if (p.MinPrice.HasValue)
            { q = q.Where(t => t.Price >= p.MinPrice); }

            if (p.MaxPrice.HasValue)
            { q = q.Where(t => t.Price <= p.MaxPrice); }

            if (p.TicketsMaxMin.HasValue)
            { q = q.Where(t => t.MaxTicketsDefault >= p.TicketsMaxMin); }

            if (p.TicketsMaxMax.HasValue)
            { q = q.Where(t => t.MaxTicketsDefault <= p.TicketsMaxMax); }

            if (p.TicketsAvailable.HasValue)
            { q = q.Where(t => t.MaxTickets >= p.TicketsAvailable); }

            if (!string.IsNullOrWhiteSpace(p.Status))
            { q = q.Where(t => t.Status == p.Status); }

            var list = await q.ToListAsync();

            if (p.StartTime.HasValue)
            {
                list = list
                    .Where(e => e.StartDate.TimeOfDay >= p.StartTime.Value)
                    .ToList();
            }
            if (p.EndTime.HasValue)
            {
                list = list
                    .Where(e => e.EndDate.TimeOfDay <= p.EndTime.Value)
                    .ToList();
            }

            if (p.Duration.HasValue)
            {
                var minutes = p.Duration.Value * 60;
                list = list
                    .Where(t => (t.EndDate - t.StartDate).TotalMinutes == minutes)
                    .ToList();
            }

            return list;
        }

    }
}
