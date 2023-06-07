using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.Repository
{
    public class FlightRepository : GenericRepository<Flight>, IFlightRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FlightRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Flight> GetLastFlight()
        {
            return await _dbContext.Flights.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
