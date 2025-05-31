using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages
{
    public class ExpositionModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public List<Exibition> Exposition { get; set; }

        public ExpositionModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            Exposition = _db.Exibition.ToList();
        }
    }
}
