using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
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
        public InputModel Input { get; set; } = new InputModel();

        public List<BraceletOption> BraceletOptions { get; set; } = new List<BraceletOption>();

        public class InputModel
        {
            [Required]
            public string ItemType { get; set; } = null!;

            [Required]
            [MaxLength(500)]
            public string Description { get; set; } = null!;

            [Display(Name = "Parent Bracelet (for charms)")]
            public int? ParentItemId { get; set; }
        }

        public class BraceletOption
        {
            public int Id { get; set; }
            public string Label { get; set; } = null!;
        }

        public IActionResult OnGet()
        {
            var customer = _customerService.GetById(CustomerId);
            if (customer == null)
            {
                return NotFound();
            }

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

            if (!ModelState.IsValid)
            {
                LoadBracelets();
                return Page();
            }

            // Only charms should have a parent bracelet
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
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to add jewelry item.");
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
