using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IAirportRepository : IGenericRepository<Airport>
    {
        Task<Airport> GetLastAirport();
    }
}
