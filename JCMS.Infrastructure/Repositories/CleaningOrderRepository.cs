using System.Collections.Generic;
using System.Linq;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace JCMS.Infrastructure.Repositories
{
    public class CleaningOrderRepository : ICleaningOrderRepository
    {
        private readonly JcmsDbContext _context;

        public CleaningOrderRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public CleaningOrder? GetById(int id)
        {
            return _context.CleaningOrder
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.JewelryItem)
                .FirstOrDefault(o => o.Id == id);
        }

        public CleaningOrder? GetByConfirmationNumber(string confirmationNumber)
        {
            return _context.CleaningOrder
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.JewelryItem)
                .FirstOrDefault(o => o.ConfirmationNumber == confirmationNumber);
        }

        public IEnumerable<CleaningOrder> GetActiveOrders()
        {
            return _context.CleaningOrder
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.JewelryItem)
                .Where(o => o.Status != "Picked Up" && o.Status != "Cancelled")
                .OrderByDescending(o => o.CheckInAt)
                .ToList();
        }

        public bool ItemIsInActiveOrder(int jewelryItemId)
        {
            return _context.OrderItems
                .Include(oi => oi.CleaningOrder)
                .Any(oi =>
                    oi.JewelryItemId == jewelryItemId &&
                    oi.CleaningOrder.Status != "Picked Up" &&
                    oi.CleaningOrder.Status != "Cancelled");
        }

        public void Add(CleaningOrder order)
        {
            _context.CleaningOrder.Add(order);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}