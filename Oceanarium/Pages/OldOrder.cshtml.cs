using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Oceanarium.Pages
{
    public class OldOrderModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostRedirect()
        {
            return RedirectToPage("Contact");
        }
    }
}
