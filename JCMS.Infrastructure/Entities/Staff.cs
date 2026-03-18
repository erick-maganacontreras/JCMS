namespace JCMS.Infrastructure.Entities
{
    public class Staff
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;  // unique, case-insensitive
        public string PasswordHash { get; set; } = null!;

        // "Staff" or "Admin"
        public string Role { get; set; } = "Staff";

        public bool IsActive { get; set; } = true;

        public ICollection<CleaningOrder> CleaningOrders { get; set; } = new List<CleaningOrder>();
        public ICollection<CleaningHistory> CleaningHistoryEntries { get; set; } = new List<CleaningHistory>();
    }
}
