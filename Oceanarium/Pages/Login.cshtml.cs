using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Identity;

namespace Oceanarium.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly UserManager<AdminUser> _userManager;
        private readonly IAdminKeyService _adminKeyService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginModel(SignInManager<AdminUser> signInManager, 
            UserManager<AdminUser> userManager, 
            IAdminKeyService adminKeyService,
             RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _adminKeyService = adminKeyService;
            _roleManager = roleManager;
        }

        [BindProperty]
        public string AccessCode { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(AccessCode) || !_adminKeyService.IsValidKey(AccessCode))
            {
                ErrorMessage = "Wrong access code";
                return Page();
            }

            // Get current user if exists
            var user = await _userManager.GetUserAsync(User);

            // If user is not authorized-create user
            if (user == null)
            {
                user = new AdminUser { UserName = Guid.NewGuid().ToString() };
                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    ErrorMessage = "Error while creating user";
                    return Page();
                }
            }

            // Cheking if admin role exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                ErrorMessage = "Role 'Admin' not found.";
                return Page();
            }

            // Give the role, if without it
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    ErrorMessage = "Error while giving a role.";
                    return Page();
                }
            }

            // Sign in as a admin
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Add cookie for access
            Response.Cookies.Append("AdminAccessCode", AccessCode, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });

            TempData["success"] = "Your login succeeded.";
            return RedirectToPage("/Index");
        }
    }

}
