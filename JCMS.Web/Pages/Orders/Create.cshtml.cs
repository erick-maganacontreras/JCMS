using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Orders
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly JewelryItemService _jewelryItemService;
        private readonly CleaningOrderService _cleaningOrderService;

        public CreateModel(
            CustomerService customerService,
            JewelryItemService jewelryItemService,
            CleaningOrderService cleaningOrderService)
        {
            _customerService = customerService;
            _jewelryItemService = jewelryItemService;
            _cleaningOrderService = cleaningOrderService;
        }

        [BindProperty(SupportsGet = true)]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }
        public List<JewelryItem> InventoryItems { get; set; } = new List<JewelryItem>();
        public List<JewelryItem> BraceletOptions { get; set; } = new List<JewelryItem>();

        [BindProperty]
        public List<int> SelectedItemIds { get; set; } = new List<int>();

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            public bool AddNewItemInline { get; set; }

            [Display(Name = "New Item Type")]
            public string? NewItemType { get; set; }

            [Display(Name = "New Item Description")]
            public string? NewItemDescription { get; set; }

            [Display(Name = "Parent Bracelet (for charm)")]
            public int? NewItemParentItemId { get; set; }
        }

        public IActionResult OnGet()
        {
            return LoadPage();
        }

        public IActionResult OnPost()
        {
            var loadResult = LoadPage();
            if (loadResult is NotFoundResult)
            {
                return loadResult;
            }

            var staffIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(staffIdClaim))
            {
                return RedirectToPage("/Account/Login");
            }

            int staffId = int.Parse(staffIdClaim);

            var result = _cleaningOrderService.CreateOrder(
                CustomerId,
                staffId,
                SelectedItemIds,
                Input.AddNewItemInline,
                Input.NewItemType,
                Input.NewItemDescription,
                Input.NewItemParentItemId);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to create order.");
                return Page();
            }

            return RedirectToPage("Receipt", new { id = result.OrderId });
        }

        private IActionResult LoadPage()
        {
            Customer = _customerService.GetById(CustomerId);
            if (Customer == null)
            {
                return NotFound();
            }

            InventoryItems = _jewelryItemService.GetInventoryForCustomer(CustomerId).ToList();
            BraceletOptions = _jewelryItemService.GetBraceletsForCustomer(CustomerId).ToList();

            return Page();
        }
    }
}