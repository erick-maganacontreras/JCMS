using System.Collections.Generic;
using System.Security.Claims;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StaffEntity = JCMS.Infrastructure.Entities.Staff;

namespace JCMS.Web.Pages.Admin.Staff
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly AuthService _authService;

        public IndexModel(AuthService authService)
        {
            _authService = authService;
        }

        public IEnumerable<StaffEntity> StaffUsers { get; set; } = new List<StaffEntity>();

        public void OnGet()
        {
            LoadUsers();
        }

        public IActionResult OnPostDeactivate(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = _authService.SetStaffActiveStatus(id, false, currentUserId.Value);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToPage();
            }

            TempData["SuccessMessage"] = "User deactivated successfully.";
            return RedirectToPage();
        }

        public IActionResult OnPostActivate(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = _authService.SetStaffActiveStatus(id, true, currentUserId.Value);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToPage();
            }

            TempData["SuccessMessage"] = "User activated successfully.";
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Forbid();
            }

            var result = _authService.DeleteStaffUser(id, currentUserId.Value);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToPage();
            }

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToPage();
        }

        private void LoadUsers()
        {
            StaffUsers = _authService.GetAllStaffUsers();
        }

        private int? GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(value, out var id))
            {
                return id;
            }

            return null;
        }
    }
}