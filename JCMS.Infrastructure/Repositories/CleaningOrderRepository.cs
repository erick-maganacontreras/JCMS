using System.Collections.Generic;
using System.Linq;
using JCMS.Infrastructure.Constants;
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
                .Where(o => o.Status != OrderStatuses.PickedUp && o.Status != OrderStatuses.Cancelled)
                .OrderByDescending(o => o.CheckInAt)
                .ToList();
        }

        public IEnumerable<CleaningOrder> Search(string? searchTerm, string? status)
        {
            var query = _context.CleaningOrder
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.JewelryItem)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(o =>
                o.ConfirmationNumber.Contains(searchTerm) ||
                o.Customer.FirstName.Contains(searchTerm) ||
                o.Customer.LastName.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            return query.OrderByDescending(o => o.CheckInAt).ToList();
        }

        public IEnumerable<CleaningOrder> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.CleaningOrder
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.JewelryItem)
                .Where(o => o.CheckInAt >= startDate && o.CheckInAt <= endDate)
                .OrderByDescending(o => o.CheckInAt)
                .ToList();
        }

        public bool ItemIsInActiveOrder(int jewelryItemId)
        {
            return _context.OrderItems
                .Include(oi => oi.CleaningOrder)
                .Any(oi =>
                    oi.JewelryItemId == jewelryItemId &&
                    oi.CleaningOrder.Status != OrderStatuses.PickedUp &&
                    oi.CleaningOrder.Status != OrderStatuses.Cancelled);
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