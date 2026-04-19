using System.ComponentModel.DataAnnotations;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Jewelry
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly JewelryItemService _jewelryItemService;

        public EditModel(JewelryItemService jewelryItemService)
        {
            _jewelryItemService = jewelryItemService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }

            [Display(Name = "Item Type")]
            public string ItemType { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter a description for the jewelry item.")]
            [MaxLength(500, ErrorMessage = "The description cannot be longer than 500 characters.")]
            [Display(Name = "Description")]
            public string Description { get; set; } = string.Empty;
        }

        public IActionResult OnGet(int id)
        {
            var item = _jewelryItemService.GetById(id);
            if (item == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = item.Id,
                CustomerId = item.CustomerId,
                ItemType = item.ItemType,
                Description = item.Description
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = _jewelryItemService.UpdateDescription(Input.Id, Input.Description);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to update item.");
                return Page();
            }

            TempData["SuccessMessage"] = "Jewelry item updated successfully.";
            return RedirectToPage("/Customers/Details", new { id = Input.CustomerId });
        }
    }
}