using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } //PK
        public int CleaningOrderId { get; set; }
        public CleaningOrder CleaningOrder { get; set; } = null!;
        public int JewelryItemId { get; set; }
        public JewelryItem JewelryItem { get; set; } = null!;
    }
}
