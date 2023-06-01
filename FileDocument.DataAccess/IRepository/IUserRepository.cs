using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<RefreshToken> GenerateRefreshToken(string userId, string tokenId);
        Task<bool> ValidateRefreshToken(User user, string refreshToken);
        public bool CheckValidEmail(string email);
        public bool CheckValidPhoneNumber(string phoneNumber);
    }
}
