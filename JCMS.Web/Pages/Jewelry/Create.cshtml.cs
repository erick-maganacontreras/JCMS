using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Jewelry
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly JewelryItemService _jewelryItemService;
        private readonly CustomerService _customerService;

        public CreateModel(
            JewelryItemService jewelryItemService,
            CustomerService customerService)
        {
            _jewelryItemService = jewelryItemService;
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public int CustomerId { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string CustomerName { get; set; } = string.Empty;

        public List<BraceletOption> BraceletOptions { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Select a jewelry item type.")]
            [Display(Name = "Item Type")]
            public string ItemType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter a description for the jewelry item.")]
            [MaxLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
            [Display(Name = "Description")]
            public string Description { get; set; } = string.Empty;

            [Display(Name = "Parent Bracelet (for charms)")]
            public int? ParentItemId { get; set; }
        }

        public class BraceletOption
        {
            public int Id { get; set; }
            public string Label { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            var customer = _customerService.GetById(CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            CustomerName = $"{customer.FirstName} {customer.LastName}";
            LoadBracelets();
            return Page();
        }

        public IActionResult OnPost()
        {
            var customer = _customerService.GetById(CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

            CustomerName = $"{customer.FirstName} {customer.LastName}";

            if (!ModelState.IsValid)
            {
                LoadBracelets();
                return Page();
            }

            if (!string.Equals(Input.ItemType, "Charm", System.StringComparison.OrdinalIgnoreCase))
            {
                Input.ParentItemId = null;
            }

            var result = _jewelryItemService.AddItem(
                CustomerId,
                Input.ItemType,
                Input.Description,
                Input.ParentItemId);

            if (!result.Success)
            {
                if (result.ErrorMessage == "A charm must be linked to a parent bracelet." || result.ErrorMessage == "Selected parent item is not a bracelet for this customer.")
                {
                    ModelState.AddModelError("Input.ParentItemId", result.ErrorMessage);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to add jewelry item.");
                }

                LoadBracelets();
                return Page();
            }

            TempData["SuccessMessage"] = "Jewelry item added successfully.";
            return RedirectToPage("/Customers/Details", new { id = CustomerId });
        }

        private void LoadBracelets()
        {
            BraceletOptions = _jewelryItemService.GetBraceletsForCustomer(CustomerId)
                .Select(b => new BraceletOption
                {
                    Id = b.Id,
                    Label = $"{b.Description} (Bracelet #{b.Id})"
                })
                .ToList();
        }
    }
}