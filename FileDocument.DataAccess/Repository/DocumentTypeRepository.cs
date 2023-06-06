using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.Repository
{
    public class DocumentTypeRepository : GenericRepository<DocumentType>, IDocumentTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentTypeRepository(ApplicationDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }
    }
}
