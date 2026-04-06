using System.Collections.Generic;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
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
            new SelectListItem { Value = "Checked In", Text = "Checked In" },
            new SelectListItem { Value = "In Cleaning", Text = "In Cleaning" },
            new SelectListItem { Value = "Completed", Text = "Completed" },
            new SelectListItem { Value = "Picked Up", Text = "Picked Up" },
            new SelectListItem { Value = "Cancelled", Text = "Cancelled" }
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
