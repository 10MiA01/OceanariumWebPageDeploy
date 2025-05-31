using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Identity;

namespace Oceanarium.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly UserManager<AdminUser> _userManager;

        public LogoutModel(SignInManager<AdminUser> signInManager,
            UserManager<AdminUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            await _signInManager.SignOutAsync();

            // AdminAccessCode - delete
            Response.Cookies.Delete("AdminAccessCode");

            // User - delete
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            TempData["success"] = "Your logout succeeded.";
            return RedirectToPage("/Index");
        }
    }
}
