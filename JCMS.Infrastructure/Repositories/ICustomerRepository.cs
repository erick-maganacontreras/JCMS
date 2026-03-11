using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;
using JCMS.Infrastructure.Repositories;


namespace JCMS.Infrastructure.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetById(int id);
        IEnumerable<Customer> Search(string query);
        void Add(Customer customer);
        void Update(Customer customer);
    }
}
