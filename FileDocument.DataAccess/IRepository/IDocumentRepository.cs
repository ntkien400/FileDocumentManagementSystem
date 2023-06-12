using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> FilterDocuments(string? flightId = null, string? createdDate = null, string? typeDocumentId = null);
        Task<Document> CheckDocumentNameExistsInFlight(string fileName, string flightId);
        Task<int> GetUserPermission(string userId, string docTypeId);
    }
}
