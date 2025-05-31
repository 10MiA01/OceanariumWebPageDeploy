using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Models;
using Oceanarium.Servises.Interfaces;
using Oceanarium.ViewModels;

namespace Oceanarium.Pages.Admin.Exibitions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IFilterExibitionService _filterExibitionService;
        public List<Exibition> _Exibitions;


        public IndexModel(ApplicationDbContext db, IFilterExibitionService filterExibitionService)
        {
            _db = db;
            _filterExibitionService = filterExibitionService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _Exibitions = await _db.Exibition.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetAllExibitionsAsync(
            int? id,
            string? name,
            string? description,
            bool? isPermanent,
            DateTime? startDate,
            DateTime? endDate)
        {
            var filter = new ExibitionFilterParams
            {
                Id = id,
                Name = name,
                Description = description,
                IsPermanent = isPermanent,
                StartDate = startDate,
                EndDate = endDate
            };

            var exibitions = await _filterExibitionService.GetFilteredAsync(filter);
            return new JsonResult(new { objExibitionsList = exibitions });
        }


        public async Task<IActionResult> OnGetDeleteExibitionAsync(int? exibitionId)
        {
            if (exibitionId == null)
            {
                return NotFound();
            }

            Exibition? toDelete = await _db.Exibition.FindAsync(exibitionId);
            if (toDelete == null)
            {
                return NotFound();
            }

            _db.Exibition.Remove(toDelete);
            await _db.SaveChangesAsync();
            TempData["success"] = "Exibition deleted successfully.";

            return RedirectToPage("./Index");
        }
    }
}
