using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace JCMS.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly JcmsDbContext _context;

        public CustomerRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public Customer? GetById(int id)
        {
            return _context.Customers
                .Include(c => c.JewelryItems)
                .FirstOrDefault(c => c.Id == id);
        }

        public Customer? GetByEmail(string email)
        {
            return _context.Customers
                .FirstOrDefault(c => c.Email == email);
        }

        public IEnumerable<Customer> Search(string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return _context.Customers
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToList();
            }

            term = term.Trim();

            return _context.Customers
                .Where(c =>
                    c.LastName.Contains(term) ||
                    c.Email.Contains(term) ||
                    c.Phone.Contains(term))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToList();
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
