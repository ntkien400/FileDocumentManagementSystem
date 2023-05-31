using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;

namespace FileDocument.DataAccess.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<LoginResult> Login(LoginDto loginDto);
        Task Logout();
        Task ForgotPassword(string email);
        Task ResetPassword(string newPassword);
        Task<RegisterResult> Register(User user);
        Task<RefreshTokenDto> GenerateAccessToken(User user);
        public User GetUserFromAccessToken(string token);
        Task<bool> ValidateRefreshToken(User user, string refreshToken);
    }
}
