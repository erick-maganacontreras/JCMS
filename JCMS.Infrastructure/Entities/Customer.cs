using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class Customer
    {
        public int Id { get; set; } //PK
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!; //unique
        public string Phone { get; set; } = null!; //(XXX) XXX-XXXX
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Navigation
        public ICollection<JewelryItem> JewelryItems { get; set; } = new List<JewelryItem>();
        public ICollection<CleaningOrder> CleaningOrders { get; set; } = new List<CleaningOrder>();
    }
}
