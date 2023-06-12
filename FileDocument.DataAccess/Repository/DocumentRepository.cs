using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FileDocument.DataAccess.Repository
{

    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetUserPermission(string userId, string docTypeId)
        {
            var userMember = await _dbContext.GroupMember.FirstOrDefaultAsync(gm => gm.UserId == userId);
            if (userMember != null)
            {
                var groupPermission = await _dbContext.GroupDocTypePermissions.FirstOrDefaultAsync(gp => gp.GroupId == userMember.GroupId && gp.DocumentTypeId == docTypeId);
                if(groupPermission == null)
                {
                    return 0;
                }

                return groupPermission.PermissionId;
            }

            return 0;
        }
        public async Task<Document> CheckDocumentNameExistsInFlight(string fileName, string flightId)
        {
            var document = await _dbContext.Documents.Where(x => x.Name == fileName && x.FlightId == flightId).OrderByDescending(d => d.DateCreated).FirstOrDefaultAsync();
            return document;
        }

        public async Task<IEnumerable<Document>> FilterDocuments(string? flightId, string? createdDate, string? typeDocumentId)
        {
            IQueryable<Document> query = _dbContext.Documents;
            if(flightId != null)
            {
                query = query.Where(d => d.FlightId == flightId);
            }

            if(createdDate != null)
            {
                DateTime date;
                DateTime.TryParseExact(createdDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                query = query.Where(d => d.DateCreated.Date == date);
                
            }

            if (typeDocumentId != null)
            {
                query = query.Where(d => d.DocumentTypeId == typeDocumentId);
            }

            return await query.ToListAsync();
        }
    }
}
