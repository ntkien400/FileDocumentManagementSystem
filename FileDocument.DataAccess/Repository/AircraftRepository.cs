using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.Repository
{
    public class AircraftRepository : GenericRepository<Aircraft>, IAircraftRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AircraftRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Aircraft> GetLastAircraft()
        {
            return await _dbContext.Aircrafts.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
