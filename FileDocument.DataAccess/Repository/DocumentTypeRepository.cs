using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.Repository
{
    public class DocumentTypeRepository : GenericRepository<DocumentType>, IDocumentTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentTypeRepository(ApplicationDbContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<DocumentType> GetLastDocumentType()
        {
            return await _dbContext.DocumentTypes.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }
    }
}
