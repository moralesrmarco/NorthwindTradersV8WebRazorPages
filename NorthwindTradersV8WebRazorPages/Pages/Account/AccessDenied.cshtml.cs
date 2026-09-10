using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NorthwindTradersV8WebRazorPages.Pages.Account
{
    [AllowAnonymous]
    public class AccessDeniedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}