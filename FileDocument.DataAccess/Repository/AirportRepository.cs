using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.Repository
{
    public class AirportRepository : GenericRepository<Airport>, IAirportRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AirportRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Airport> GetLastAirport()
        {
            return await _dbContext.Airports.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
