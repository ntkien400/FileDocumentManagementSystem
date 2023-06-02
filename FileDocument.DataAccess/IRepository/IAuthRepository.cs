using FileDocument.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.IRepository
{
    public interface IAuthRepository
    {
        Task<RefreshToken> GenerateRefreshToken(string userId, string tokenId);
        Task<bool> ValidateRefreshToken(User user, string refreshToken);
    }
}
