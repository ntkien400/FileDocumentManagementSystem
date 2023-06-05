using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.Repository
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Group> GetLastGroup()
        {
            return await _dbContext.Groups.OrderByDescending(g => g.Id).FirstOrDefaultAsync();
        }
    }
}
