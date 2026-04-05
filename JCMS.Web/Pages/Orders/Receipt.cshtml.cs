using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Orders
{
    [Authorize]
    public class ReceiptModel : PageModel
    {
        private readonly CleaningOrderService _cleaningOrderService;

        public ReceiptModel(CleaningOrderService cleaningOrderService)
        {
            _cleaningOrderService = cleaningOrderService;
        }

        public CleaningOrder? Order { get; set; }

        public IActionResult OnGet(int id)
        {
            Order = _cleaningOrderService.GetById(id);
            if (Order == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
