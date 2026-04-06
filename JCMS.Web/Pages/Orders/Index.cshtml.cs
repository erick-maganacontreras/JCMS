using System.Collections.Generic;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet()
        {
            Orders = _cleaningOrderService.GetActiveOrders();
        }
    }
}
