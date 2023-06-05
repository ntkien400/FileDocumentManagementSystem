using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.Repository
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepoitory
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
