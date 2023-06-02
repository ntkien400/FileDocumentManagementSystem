using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public bool CheckValidEmail(string email);
        public bool CheckValidPhoneNumber(string phoneNumber);
    }
}
