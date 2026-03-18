using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly JcmsDbContext _context;

        public StaffRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public Staff? GetById(int id)
        {
            return _context.Staff.FirstOrDefault(s => s.Id == id);
        }

        public Staff? GetByUsername(string username)
        {
            var normalized = username.Trim().ToLower();
            return _context.Staff.FirstOrDefault(s => s.Username.ToLower() == normalized);
        }

        public void Add(Staff staff)
        {
            _context.Staff.Add(staff);
        }

        public void Update(Staff staff)
        {
            _context.Staff.Update(staff);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
