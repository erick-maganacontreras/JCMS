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
        public InputModel Input { get; set; } = null!;

        public class InputModel
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }

            public string ItemType { get; set; } = null!;

            [Required]
            [MaxLength(500)]
            public string Description { get; set; } = null!;
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
