using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.Repository
{
    public class SystemConfigureRepository : GenericRepository<SystemConfigure>, ISystemConfigureRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SystemConfigureRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
