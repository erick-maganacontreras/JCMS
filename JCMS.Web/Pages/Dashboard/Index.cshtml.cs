using System.Collections.Generic;
using System.Linq;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CleaningOrderService _cleaningOrderService;

        public IndexModel(CleaningOrderService cleaningOrderService)
        {
            _cleaningOrderService = cleaningOrderService;
        }

        public int ActiveOrderCount { get; set; }
        public int CheckedInCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }

        public IList<ActiveOrderViewModel> ActiveOrders { get; set; } = new List<ActiveOrderViewModel>();

        public class ActiveOrderViewModel
        {
            public int Id { get; set; }
            public string ConfirmationNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public System.DateTime CheckInAt { get; set; }
            public int ItemCount { get; set; }
        }

        public void OnGet()
        {
            var orders = _cleaningOrderService.GetActiveOrders().ToList();

            ActiveOrderCount = orders.Count;
            CheckedInCount = orders.Count(o => o.Status == "Checked In");
            InProgressCount = orders.Count(o => o.Status == "In Progress");
            CompletedCount = orders.Count(o => o.Status == "Completed");

            ActiveOrders = orders.Select(o => new ActiveOrderViewModel
            {
                Id = o.Id,
                ConfirmationNumber = o.ConfirmationNumber,
                CustomerName = $"{o.Customer.FirstName} {o.Customer.LastName}",
                CustomerPhone = o.Customer.Phone,
                Status = o.Status,
                CheckInAt = o.CheckInAt,
                ItemCount = o.OrderItems.Count
            }).ToList();
        }
    }
}