using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class CleaningHistory
    {
        public int Id { get; set; }  // PK
        public int JewelryItemId { get; set; }
        public JewelryItem JewelryItem { get; set; } = null!;
        public DateTime CleaningDate { get; set; }  // completion date
        public string ConfirmationNumber { get; set; } = null!;
        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;
    }
}
