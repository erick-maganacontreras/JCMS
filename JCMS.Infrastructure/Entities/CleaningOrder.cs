using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCMS.Infrastructure.Entities
{
    public class CleaningOrder
    {
        public int Id { get; set; }  // PK
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int StaffId { get; set; }
        public Staff Staff { get; set; } = null!;
        public string ConfirmationNumber { get; set; } = null!;  // 6-char unique
        // Checked In, In Progress, Completed, Picked Up, Cancelled
        public string Status { get; set; } = "Checked In";
        public DateTime CheckInAt { get; set; } = DateTime.UtcNow;
        public DateTime? InProgressAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
