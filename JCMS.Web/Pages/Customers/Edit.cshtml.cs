using System.ComponentModel.DataAnnotations;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly CustomerService _customerService;

        public EditModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public class InputModel 
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            [Display(Name = "Phone (XXX) XXX-XXXX")]
            public string Phone { get; set; } = null!;
        }

        public IActionResult OnGet(int id)
        {
            var customer = _customerService.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
            };

            return Page();
        }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = _customerService.UpdateCustomer(
                Input.Id,
                Input.Email,
                Input.Phone);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to update customer.");
                return Page();
            }

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToPage("Index");
        }
    }
}
