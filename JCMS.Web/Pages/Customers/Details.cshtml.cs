using System.Collections.Generic;
using System.Linq;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly JewelryItemService _jewelryItemService;

        public DetailsModel(CustomerService customerService, JewelryItemService jewelryItemService)
        {
            _customerService = customerService;
            _jewelryItemService = jewelryItemService;
        }

        public Customer? Customer { get; set; }
        public IList<JewelryItemViewModel> Items { get; set; } = new List<JewelryItemViewModel>();

        public class JewelryItemViewModel
        {
            public int Id { get; set; }
            public string ItemType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string LastCleaningDate { get; set; } = "Never cleaned";
        }

        public IActionResult OnGet(int id)
        {
            var customer = _customerService.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;

            var items = _jewelryItemService.GetInventoryForCustomer(id);
            Items = items.Select(i => new JewelryItemViewModel
            {
                Id = i.Id,
                ItemType = i.ItemType,
                Description = i.Description,
                LastCleaningDate = GetLastCleaningDate(i)
            }).ToList();

            return Page();
        }

        private string GetLastCleaningDate(JewelryItem item)
        {
            var latest = item.CleaningHistoryEntries?
                .OrderByDescending(h => h.CleaningDate)
                .FirstOrDefault();

            return latest == null ? "Never cleaned" : latest.CleaningDate.ToLocalTime().ToString("d");
        }
    }
}