using System.Collections.Generic;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public interface IStaffRepository
    {
        Staff? GetById(int id);
        Staff? GetByUsername(string username);
        IEnumerable<Staff> GetAll();
        int CountActiveAdmins();
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(Staff staff);
        void SaveChanges();
    }
}