using FileDocument.DataAccess.IRepository;
using FileDocument.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace FileDocument.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Address = new AddressRepository(_dbContext);
            User = new UserRepository(_dbContext);
            Authenticate = new AuthRepository(_dbContext);
            Group = new GroupRepository(_dbContext);
            GroupUser = new GroupMemberRepository(_dbContext);
            Permission = new PermissionRepository(_dbContext);
            SystemConfigure = new SystemConfigureRepository(_dbContext);
            GroupDocTypePermission = new GroupDocTypePermissionRepository(_dbContext);
            DocumentType = new DocumentTypeRepository(_dbContext);
            Aircraft = new AircraftRepository(_dbContext);
            Airport = new AirportRepository(_dbContext);
            Flight = new FlightRepository(_dbContext);
            Document = new DocumentRepository(_dbContext);
        }

        public IAddressRepository Address { get; private set; }
        public IUserRepository User { get; private set; }
        public IAuthRepository Authenticate { get; private set; }
        public IGroupRepository Group { get; private set; }
        public IGroupMemberRepository GroupUser { get; private set; }
        public IPermissionRepoitory Permission { get; private set; }
        public ISystemConfigureRepository SystemConfigure { get; private set; }
        public IGroupDocTypePermissionRepository GroupDocTypePermission { get; private set; }
        public IDocumentTypeRepository DocumentType { get; private set; }
        public IAircraftRepository Aircraft { get; private set; }
        public IAirportRepository Airport { get; private set; }
        public IFlightRepository Flight { get; private set; }
        public IDocumentRepository Document { get; private set; }
        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

        //public async Task<int> SaveChangesAsync()
        //{
        //    var count = await _dbContext.SaveChangesAsync();
        //    return count;
        //}

        public async Task<int> SaveChangesAsync()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified)
                {
                    // Get the original values of the entity
                    var originalValues = await entry.GetDatabaseValuesAsync();

                    // Update only the modified properties
                    entry.CurrentValues.SetValues(entry.Entity);
                    entry.OriginalValues.SetValues(originalValues);
                }
            }

            var count = await _dbContext.SaveChangesAsync();
            return count;
        }

    }
}
