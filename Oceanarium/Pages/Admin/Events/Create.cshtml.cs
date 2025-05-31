using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages.Admin.Events
{
    public class CreateModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Event newEvent { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (newEvent.StartDate >= newEvent.EndDate)
            {
                ModelState.AddModelError("StartDate", "Start date must be before end date.");
                return Page();
            }


            _db.Add(newEvent);
            await _db.SaveChangesAsync();
            TempData["success"] = "Event created successfully.";
            return RedirectToPage("./Index");
        }
    }
}