using System.Collections.Generic;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly JewelryItemService _jewelryItemService;

        public HistoryModel(CustomerService customerService, JewelryItemService jewelryItemService)
        {
            _customerService = customerService;
            _jewelryItemService = jewelryItemService;
        }

        public Customer? Customer { get; set; } 
        public IEnumerable<CleaningHistory> History { get; set; } = new List<CleaningHistory>();

        public IActionResult OnGet(int id)
        {
            Customer = _customerService.GetById(id);
            if (Customer == null)
            {
                return NotFound();
            }

            History = _jewelryItemService.GetCleaningHistoryForCustomer(id);
            return Page();
        }
    }
}
