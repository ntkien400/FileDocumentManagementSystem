using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IFlightRepository : IGenericRepository<Flight>
    {
        Task<Flight> GetLastFlight();
    }
}
