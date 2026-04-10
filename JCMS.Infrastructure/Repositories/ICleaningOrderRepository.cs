using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public interface ICleaningOrderRepository
    {
        CleaningOrder? GetById(int id);
        CleaningOrder? GetByConfirmationNumber(string confirmationNumber);
        IEnumerable<CleaningOrder> GetActiveOrders();
        IEnumerable<CleaningOrder> Search(string? searchTerm, string? status);
        IEnumerable<CleaningOrder> GetOrdersByDateRange(DateTime stardDate, DateTime endDate);
        bool ItemIsInActiveOrder(int jewelryItemId);
        void Add(CleaningOrder order);
        void SaveChanges();
    }
}
