using AutoMapper;
using FileDocument.DataAccess.IRepository;
using FileDocument.DataAccess.Repository;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISendEmail _sendEmail;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unit,
                              IMapper mapper,
                              UserManager<User> userManager,
                              SignInManager<User> signInManager,
                              IConfiguration configuration,
                              ISendEmail sendEmail)
        {
            _unit = unit;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _sendEmail = sendEmail;
        }

        [HttpGet("get_all_user")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {
            var listUser = await _unit.User.GetAllAsync();
            return Ok(listUser);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _unit.User.GetAsync(a => a.Id == id, properties:"Address");
            
            if (user != null)
            {
                return Ok(new User
                {
                    Id = user.Id,
                    FristName = user.FristName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address
                });
            }
            return NotFound("User not exists");
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterDto>> Register([FromForm] RegisterDto register)
        {
            var isExistEmail = await _userManager.FindByEmailAsync(register.Email);
            var isExistPhoneNumber = await _unit.User.GetAsync(x => x.PhoneNumber == register.PhoneNumber);
            var validEmail = _unit.User.CheckValidEmail(register.Email);
            var validPhoneNumber = _unit.User.CheckValidEmail(register.PhoneNumber);
            if (isExistEmail == null && isExistPhoneNumber == null)
            {
                if (!validEmail)
                {
                    return BadRequest("Invalid Email");
                }

                if (!validPhoneNumber)
                {
                    return BadRequest("Invalid Phone Number");
                }

                User user = new User();
                _mapper.Map(register, user);
                var createUser = await _userManager.CreateAsync(user);

                if (createUser.Succeeded)
                {
                    return Ok($"Create User Successfully \n {user}");
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in createUser.Errors)
                    {
                        errors.Add(error.Description);
                    }

                    return BadRequest($"Failed create User beacause: {errors}");
                }
            }

            return BadRequest("Email or PhoneNumber has been used");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            var signIn = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);

            if (checkPassword && user != null && signIn.Succeeded)
            {
                RefreshTokenDto refreshTokenDto = await GenerateAccessToken(user);
                return Ok(refreshTokenDto);
            }

            return Unauthorized("Email or Password is wrong");
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successfully");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var user = await GetUserFromAccessToken(refreshTokenDto.Token);
            var validateToken = await _unit.User.ValidateRefreshToken(user, refreshTokenDto.RefreshToken);
            if(user != null && validateToken)
            {
                await _signInManager.RefreshSignInAsync(user);
                RefreshTokenDto refreshToken = await GenerateAccessToken(user);
                return Ok(refreshToken);
            }
            
            return Unauthorized();
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var uriBuilder = new UriBuilder("https://localhost:7102/reset-password");
                var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                query["email"] = email;
                uriBuilder.Query = query.ToString();
                string passwordResetUrl = uriBuilder.ToString();
                await _sendEmail.SendResetPasswordAsync(email, passwordResetUrl);

                return Ok($"Reset password link is sent to {email}");
            }
            return BadRequest("Email is not exists");
        }

        [HttpGet("reset-password")]
        public async Task<ActionResult> ResetPassword(string token, string email)
        {
            var resetPassword = new ResetPasswordDto { Email = email, Token = token };
            return Ok(new { resetPassword });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if(resetPasswordResult.Succeeded)
                {
                    return Ok("Password has been changed");
                }
            }
            return BadRequest("Cannot change password");
            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([FromForm] RegisterDto updateUser, string id)
        {
            var user = await _unit.User.GetAsync(a => a.Id == id);
           
            if(user != null)
            {
                _mapper.Map(updateUser, user);
                await _unit.User.AddAsync(user);
                var count = await _unit.SaveChangesAsync();

                if(count > 0)
                {
                    return Ok("Update successfully");
                }

                return BadRequest("Something went wrong when updating");
            }

            return NotFound("User is not exists");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _unit.User.GetAsync(a => a.Id == id);

            if(user != null)
            {
                _unit.User.Delete(user);
                var count = await _unit.SaveChangesAsync();

                if(count > 0)
                {
                    return Ok("Delete successfully");
                }
                
                return BadRequest("Something went wrong when deleting");
            }
            
            return NotFound("User is not exists");
        }


        private async Task<RefreshTokenDto> GenerateAccessToken(User user)
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
                RefreshToken = (await _unit.User.GenerateRefreshToken(user.Id, token.Id)).Token,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo
            };
            return refresTokenDto;
        }

        private async Task<User> GetUserFromAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = authSigningKey,
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                var userEmail = principal.FindFirstValue(ClaimTypes.Email);
                var userInfo = await _unit.User.GetAsync(a => a.Email == userEmail);
                return userInfo;
            }

            return null;
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
    }
}
