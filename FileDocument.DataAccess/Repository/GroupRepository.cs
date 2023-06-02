using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.Repository
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
