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
    public class IndexModel : PageModel
    {
        private readonly CleaningOrderService _cleaningOrderService;

        public IndexModel(CleaningOrderService cleaningOrderService)
        {
            _cleaningOrderService = cleaningOrderService;
        }

        public IEnumerable<CleaningOrder> Orders { get; set; } = new List<CleaningOrder>();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status {  get; set; }

        public List<SelectListItem> StatusOptions { get; set; } = new()
        {
            new SelectListItem { Value = "", Text = "All Statuses"},
            new SelectListItem { Value = OrderStatuses.CheckedIn, Text = OrderStatuses.CheckedIn },
            new SelectListItem { Value = OrderStatuses.InProgress, Text = OrderStatuses.InProgress },
            new SelectListItem { Value = OrderStatuses.Completed, Text = OrderStatuses.Completed },
            new SelectListItem { Value = OrderStatuses.PickedUp, Text = OrderStatuses.PickedUp },
            new SelectListItem { Value = OrderStatuses.Cancelled, Text = OrderStatuses.Cancelled }
        };

        public void OnGet()
        {
            Orders = _cleaningOrderService.Search(SearchTerm, Status);
        }
    }
}
