using System.Collections.Generic;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JCMS.Web.Pages.Orders
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly CleaningOrderService _cleaningOrderService;

        public DetailsModel(CleaningOrderService cleaningOrderService)
        {
            _cleaningOrderService = cleaningOrderService;
        }

        public CleaningOrder? Order { get; set; }

        [BindProperty]
        public string NewStatus { get; set; } = string.Empty;

        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = OrderStatuses.CheckedIn, Text = OrderStatuses.CheckedIn },
            new SelectListItem { Value = OrderStatuses.InProgress, Text = OrderStatuses.InProgress },
            new SelectListItem { Value = OrderStatuses.Completed, Text = OrderStatuses.Completed },
            new SelectListItem { Value = OrderStatuses.PickedUp, Text = OrderStatuses.PickedUp },
            new SelectListItem { Value = OrderStatuses.Cancelled, Text = OrderStatuses.Cancelled }
        };

        public IActionResult OnGet(int id)
        {
            Order = _cleaningOrderService.GetById(id);
            if (Order == null)
            {
                return NotFound();
            }

            NewStatus = Order.Status;
            return Page();
        }

        public IActionResult OnPostUpdateStatus(int id)
        {
            var result = _cleaningOrderService.UpdateStatus(id, NewStatus);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to update the order status.");
                Order = _cleaningOrderService.GetById(id);
                return Page();
            }

            return RedirectToPage(new { id });
        }
    }
}
