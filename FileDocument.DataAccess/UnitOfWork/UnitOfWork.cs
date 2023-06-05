using FileDocument.DataAccess.IRepository;
using FileDocument.DataAccess.Repository;

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
        }

        public IAddressRepository Address { get; private set; }
        public IUserRepository User { get; private set; }
        public IAuthRepository Authenticate { get; private set; }
        public IGroupRepository Group { get; private set; }
        public IGroupMemberRepository GroupUser { get; private set; }
        public IPermissionRepoitory Permission { get; private set; }
        public ISystemConfigureRepository SystemConfigure { get; private set; }
        public async Task DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            var count = await _dbContext.SaveChangesAsync();
            return count;
        }
    }
}
