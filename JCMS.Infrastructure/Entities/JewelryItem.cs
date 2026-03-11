using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class JewelryItem
    {
        public int Id { get; set; }  // PK
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        // Ring, Necklace, Bracelet, Earrings, Charm, Anklet, Other
        public string ItemType { get; set; } = null!;
        public string Description { get; set; } = null!;  // up to 500 chars
        // For charm bracelet parent-child relationship
        public int? ParentItemId { get; set; }
        public JewelryItem? ParentItem { get; set; }
        public ICollection<JewelryItem> ChildItems { get; set; } = new List<JewelryItem>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CleaningHistory> CleaningHistoryEntries { get; set; } = new List<CleaningHistory>();
    }
}
