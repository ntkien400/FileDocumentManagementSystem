using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IDocumentTypeRepository : IGenericRepository<DocumentType>
    {
        Task<DocumentType> GetLastDocumentType();
    }
}
