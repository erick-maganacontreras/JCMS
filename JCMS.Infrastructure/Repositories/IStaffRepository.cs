using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public interface IStaffRepository
    {
        Staff? GetById(int id);
        Staff? GetByUsername(string username);
        void Add(Staff staff);
        void Update(Staff staff);
        void SaveChanges();
    }
}
