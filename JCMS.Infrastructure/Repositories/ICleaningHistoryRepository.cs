using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCMS.Infrastructure.Entities;

namespace JCMS.Infrastructure.Repositories
{
    public interface ICleaningHistoryRepository
    {
        void Add(CleaningHistory history);
    }
}
