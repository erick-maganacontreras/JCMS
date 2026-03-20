using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Orders
{
    [Authorize]
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
