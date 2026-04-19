using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace JCMS.Web.Pages.Customers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CustomerService _customerService;

        public IndexModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<Customer> Customers { get; set; } = new List<Customer>();

        public void OnGet()
        {
            Customers = _customerService.Search(SearchTerm).ToList();
        }
    }
}