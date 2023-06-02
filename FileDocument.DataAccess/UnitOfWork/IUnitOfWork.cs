using FileDocument.DataAccess.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAddressRepository Address { get; }
        IUserRepository User { get; }
        IAuthRepository Authenticate { get; }
        Task DisposeAsync();
        Task<int> SaveChangesAsync();
    }
}
