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
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace FileDocument.DataAccess.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public UserRepository(ApplicationDbContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration) : base(dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public User GetUserFromAccessToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResult> Register(User user)
        {
            var registerResult = new RegisterResult();
            var isExistEmail = await _userManager.FindByEmailAsync(user.Email);
            var validEmail = CheckValidEmail(user.Email);
            if (isExistEmail == null)
            {
                if (validEmail)
                {
                    await _userManager.CreateAsync(user);
                    registerResult.Message = "Create User successfully";
                    registerResult.Success = true;
                    return registerResult;
                }
                else
                {
                    registerResult.Message = "Invalid Email";
                    return registerResult;
                }
            }
            else
            {
                registerResult.Message = "Email is already exists";
                return registerResult;
            }
        }

        public async Task<LoginResult> Login(LoginDto loginDto)
        {
            var loginResult = new LoginResult();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user != null)
            {
                var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (checkPassword)
                {
                    RefreshTokenDto refreshTokenDto = await GenerateAccessToken(user);
                    loginResult.Success = true;
                    loginResult.Message = refreshTokenDto.Token;
                    return loginResult;
                }
                else
                {
                    loginResult.Message = "Wrong password";
                    return loginResult;
                }
            }
            else
            {
                loginResult.Message = "Email is not exists";
                return loginResult;
            }
            
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public Task ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshTokenDto> GenerateAccessToken(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = GenerateNewJWT(authClaims);
            var refresTokenDto = new RefreshTokenDto
            {
                RefreshToken = (await GenerateRefreshToken(user.Id, token.Id)).Token,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo
            };
            return refresTokenDto;
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

        private JwtSecurityToken GenerateNewJWT(List<Claim> authClaims)
        {
            var authSerect = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(30),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSerect, SecurityAlgorithms.HmacSha512Signature)
                );
            return tokenObject;
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

        private bool CheckValidEmail(string email)
        {
            const string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(email);
            var domainEmail = email.Split('@')[1];
            if (match.Success && domainEmail.Equals("gmail.com"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
    }
}
