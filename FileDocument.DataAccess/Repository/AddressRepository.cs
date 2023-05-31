using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.Repository
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
