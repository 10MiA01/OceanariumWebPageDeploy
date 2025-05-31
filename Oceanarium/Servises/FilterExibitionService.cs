using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Oceanarium.Servises
{
    public class FilterExibitionService : IFilterExibitionService
    {
        private readonly ApplicationDbContext _db;

        public FilterExibitionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Exibition>> GetFilteredAsync(ExibitionFilterParams p)
        {
            var q = _db.Exibition.AsQueryable();

            if (p.Id.HasValue)
            { q = q.Where(t => t.Id == p.Id); }

            if (!string.IsNullOrWhiteSpace(p.Name))
            { q = q.Where(t => t.Name.Contains(p.Name)); }

            if (!string.IsNullOrWhiteSpace(p.Description))
            { q = q.Where(t => t.Description.Contains(p.Description)); }

            if (p.IsPermanent.HasValue)
            {
                q = q.Where(e => e.IsPermanent == p.IsPermanent.Value);

                if (p.IsPermanent == false)
                {
                    if (p.StartDate.HasValue)
                    {
                        q = q.Where(e => e.StartDate >= p.StartDate.Value);
                    }

                    if (p.EndDate.HasValue)
                    {
                        q = q.Where(e => e.EndDate <= p.EndDate.Value);
                    }
                }
            }

            return await q.ToListAsync();
        }
    }
}
