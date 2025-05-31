using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages.Admin.Exibitions
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Exibition newExibition { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("ImageUrl", "Please upload an image.");
                return Page();
            }

            if (newExibition.StartDate >= newExibition.EndDate)
            {
                ModelState.AddModelError("StartDate", "Start date must be before end date.");
                return Page();
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageUrl", "Only .jpg, .jpeg and .png files are allowed.");
                return Page();
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "animals");
            Directory.CreateDirectory(uploadsFolder); // Create if not exists

            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            newExibition.ImageUrl = "/images/animals/" + uniqueFileName;

            _db.Add(newExibition);
            await _db.SaveChangesAsync();

            TempData["success"] = "Exibition created successfully.";
            return RedirectToPage("./Index");
        }
    }
}

