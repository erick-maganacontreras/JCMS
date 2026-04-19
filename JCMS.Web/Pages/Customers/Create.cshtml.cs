using System.ComponentModel.DataAnnotations;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly CustomerService _customerService;

        public CreateModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Enter the customer's first name.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter the customer's last name.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter the customer's email address.")]
            [EmailAddress(ErrorMessage = "Enter a valid email address.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter the customer's phone number.")]
            [Display(Name = "Phone")]
            public string Phone { get; set; } = string.Empty;
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