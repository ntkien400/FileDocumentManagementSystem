using FileDocument.DataAccess.IRepository;
using FileDocument.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public AuthRepository(ApplicationDbContext dbContext)
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
                refreshToken.DateExpried = DateTime.Now.AddDays(1);
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
                && user.IsTokenValid
                && userRefreshToken.UserId == user.Id
                && userRefreshToken.DateExpried > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
