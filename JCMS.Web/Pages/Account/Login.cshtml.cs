using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using JCMS.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _authService;

        public LoginModel(AuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        [TempData]
        public string? InfoMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Enter your username.")]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Enter your password.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Keep me signed in on this device")]
            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                InfoMessage = "You are already signed in.";
            }

            if (TempData.ContainsKey("SessionExpired"))
            {
                InfoMessage = "Your session expired due to 30 minutes of inactivity. Please sign in again.";
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _authService.Authenticate(Input.Username, Input.Password, out var errorMessage);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "The username or password is incorrect.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToPage("/Dashboard/Index");
        }
    }
}