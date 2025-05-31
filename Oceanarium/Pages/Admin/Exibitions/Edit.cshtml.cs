using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Data;
using Oceanarium.Models;

namespace Oceanarium.Pages.Admin.Exibitions
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Exibition changeExibition { get; set; }

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> OnGetAsync(int? exibitionId)
        {
            if (exibitionId == null || exibitionId == 0)
            {
                return NotFound();
            }

            changeExibition = await _db.Exibition.FindAsync(exibitionId);

            if (changeExibition == null)
            {
                return NotFound();
            }
            Console.WriteLine(changeExibition.ImageUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (changeExibition.IsPermanent)
            {
                changeExibition.StartDate = null;
                changeExibition.EndDate = null;
            }
            else
            {
                if (changeExibition.StartDate == null || changeExibition.EndDate == null)
                {
                    ModelState.AddModelError("", "Please provide both start and end dates.");
                    return Page();
                }

                if (changeExibition.StartDate >= changeExibition.EndDate)
                {
                    ModelState.AddModelError("StartDate", "Start date must be before end date.");
                    return Page();
                }
            }

            if (file != null && file.Length > 0)
            {
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

                if (!string.IsNullOrEmpty(changeExibition.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", changeExibition.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                changeExibition.ImageUrl = "/images/animals/" + uniqueFileName;
            }
            else if (string.IsNullOrEmpty(changeExibition.ImageUrl))
            {
                Console.WriteLine($"{changeExibition.ImageUrl} not found");
                ModelState.AddModelError("ImageUrl", "Please upload an image.");
                return Page();
            }



            _db.Exibition.Update(changeExibition);
            await _db.SaveChangesAsync();
            TempData["success"] = "Exibition updated successfully.";
            return RedirectToPage("./Index");
        }
    }
}
