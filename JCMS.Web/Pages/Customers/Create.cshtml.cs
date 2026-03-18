using System.ComponentModel.DataAnnotations;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly CustomerService _customerService;

        public CreateModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel 
        {
            [Required]
            public string FirstName { get; set; } = null!;

            [Required]
            public string LastName { get; set; } = null!;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            [Display(Name = "Phone (XXX) XXX-XXXX")]
            public string Phone { get; set; } = null!;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = _customerService.CreateCustomer(
                Input.FirstName,
                Input.LastName,
                Input.Email,
                Input.Phone);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to create customer.");
                return Page();
            }

            TempData["SuccessMessage"] = "Customer created successfully.";
            return RedirectToPage("Index");
        }
    }
}
