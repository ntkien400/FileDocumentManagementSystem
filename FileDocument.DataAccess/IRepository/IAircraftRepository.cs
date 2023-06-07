using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IAircraftRepository : IGenericRepository<Aircraft>
    {
        Task<Aircraft> GetLastAircraft();
    }
}
