using System.Collections.Generic;
using JCMS.Application.Services;
using JCMS.Infrastructure.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JCMS.Web.Pages.Customers
{
    public class IndexModel : PageModel
    {
        public readonly CustomerService _customerService;

        public IndexModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<Customer> Customers { get; set; } = new List<Customer>();
        public void OnGet()
        {
            Customers = new List<Customer>(_customerService.Search(SearchTerm));
        }
    }
}
