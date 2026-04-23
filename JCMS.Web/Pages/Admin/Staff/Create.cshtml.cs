using System.ComponentModel.DataAnnotations;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Admin.Staff
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly AuthService _authService;

        public CreateModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare(nameof(Password), ErrorMessage = "Password and confirm password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            public string Role { get; set; } = "Staff";

            [Display(Name = "Active")]
            public bool IsActive { get; set; } = true;
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

            var result = _authService.CreateStaffUser(
                Input.Username,
                Input.Password,
                Input.ConfirmPassword,
                Input.Role,
                Input.IsActive);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to create user.");
                return Page();
            }

            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToPage("/Admin/Staff/Index");
        }
    }
}