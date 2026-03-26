using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public interface IJewelryItemRepository
    {
        JewelryItem? GetById(int id);
        IEnumerable<JewelryItem> GetByCustomerId(int Id);
        IEnumerable<JewelryItem> GetBraceletsForCustomer(int customerId);
        void Add(JewelryItem item);
        void Update(JewelryItem item);
        void SaveChanges();
    }
}
