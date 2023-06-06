using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.Repository
{
    public class GroupDocTypePermissionRepository : GenericRepository<GroupDocTypePermission>, IGroupDocTypePermissionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GroupDocTypePermissionRepository(ApplicationDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }
    }
}
