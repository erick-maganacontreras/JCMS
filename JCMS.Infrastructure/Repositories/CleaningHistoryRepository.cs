using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Data;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public class CleaningHistoryRepository : ICleaningHistoryRepository
    {
        private readonly JcmsDbContext _context;

        public CleaningHistoryRepository(JcmsDbContext context)
        {
            _context = context;
        }

        public void Add(CleaningHistory history)
        {
            _context.CleaningHistory.Add(history);
        }
    }
}
