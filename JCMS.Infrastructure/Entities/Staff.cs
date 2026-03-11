using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class Staff
    {
        public int Id { get; set; } //PK
        public string UserName { get; set; } = null!; //unique
        public string PasswordHash { get; set; } = null!;

        public string Role {  get; set; } = "Staff";
        public bool IsActive { get; set; } = true;

        public ICollection<CleaningOrder> CleaningOrders { get; set; } = new List<CleaningOrder>();
        public ICollection<CleaningHistory> CleaningHistoryEntries { get; set; } = new List<CleaningHistory>();
    }
}
