using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> FilterDocuments(string? flightId, string? departureDate, string? typeDocumentId);
    }
}
