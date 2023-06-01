using FileDocument.DataAccess.IRepository;
using FileDocument.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Address = new AddressRepository(_dbContext);
        }

        public IAddressRepository Address { get; private set; }
        public IUserRepository User { get; private set; }

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
