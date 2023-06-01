using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace FileDocument.DataAccess.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        

        public async Task<RefreshToken> GenerateRefreshToken(string userId, string tokenId)
        {
            var refreshToken = new RefreshToken();
            var randomNumber = new byte[32];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
                refreshToken.DateCreated = DateTime.Now;
                refreshToken.DateExpried = DateTime.Now.AddMonths(1);
                refreshToken.UserId = userId;
                refreshToken.JwtId = tokenId;
            }

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshToken(User user, string refreshToken)
        {
            var userRefreshToken = await _dbContext.RefreshTokens
                .OrderByDescending(a => a.DateExpried)
                .FirstOrDefaultAsync(a => a.Token == refreshToken);
            if (userRefreshToken != null
                && userRefreshToken.UserId == user.Id
                && userRefreshToken.DateExpried > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public bool CheckValidEmail(string email)
        {
            const string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(email);
            var domainEmail = email.Split('@')[1];
            if (match.Success && domainEmail.Equals("gmail.com"))
            {
                return true;
            }
            return false;
        }

        public bool CheckValidPhoneNumber(string phoneNumber)
        {
            const string pattern = "/(0[3|5|7|8|9])+([0-9]{8})\b/g";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(phoneNumber);
            return match.Success ? true : false;
        }
    }
}
