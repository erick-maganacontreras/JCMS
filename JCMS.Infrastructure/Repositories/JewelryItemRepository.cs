using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace JCMS.Infrastructure.Repositories
{
    public class JewelryItemRepository : IJewelryItemRepository
    {
        private readonly JcmsDbContext _context;

        public JewelryItemRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public JewelryItem? GetById(int id)
        {
            return _context.JewelryItems
                .Include(j => j.Customer)
                .Include(j => j.ParentItem)
                .Include(j => j.ChildItems)
                .FirstOrDefault(j => j.Id == id);
        }

        public IEnumerable<JewelryItem> GetByCustomerId(int customerId)
        {
            return _context.JewelryItems
                .Where(j => j.CustomerId == customerId)
                .OrderBy(j => j.ItemType)
                .ThenBy(j => j.Description)
                .ToList();
        }

        public IEnumerable<JewelryItem> GetBraceletsForCustomer(int customerId)
        {
            return _context.JewelryItems
                .Where(j => j.CustomerId == customerId && j.ItemType == "Bracelet")
                .OrderBy(j => j.Description)
                .ToList();
        }

        public void Add(JewelryItem item)
        {
            _context.JewelryItems.Add(item);
        }

        public void Update(JewelryItem item)
        {
            _context.JewelryItems.Update(item);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
