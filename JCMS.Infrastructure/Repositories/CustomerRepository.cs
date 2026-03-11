using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Data;

namespace JCMS.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly JcmsDbContext _context;

        public CustomerRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public Customer GetById(int id) => _context.Customers.Find(id);

        public IEnumerable<Customer> Search(string query)
        {
            // TODO: Implement search accross last name, email, phone
            return new List<Customer>();
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }
    }
}
