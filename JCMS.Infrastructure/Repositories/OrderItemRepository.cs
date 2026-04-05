using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly JcmsDbContext _context;

        public OrderItemRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public void Add(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
        }
    }
}
