using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfWork unit,
                              RoleManager<IdentityRole> roleManager, 
                              UserManager<User> userManager, 
                              SignInManager<User> signInManager, 
                              IConfiguration configuration)
        {
            _unit = unit;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("seed-roles")]
        public async Task<ActionResult> SeedRoles()
        {
            int count = 0;

            foreach(var roleName in GetRoles())
            {
                var isRoleExists = await _roleManager.RoleExistsAsync(roleName);
                if(!isRoleExists)
                {
                    var role = new IdentityRole(roleName);
                    await _roleManager.CreateAsync(role);
                    count++;
                }
            }

            if(count >0)
            {
                return Ok("Seed Roles successfully");
            }

            return Ok("All roles had been seeded");
        }

        [HttpPost("add-role")]
        public async Task<ActionResult> AddRole(string email,string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var checkRole = await _roleManager.RoleExistsAsync(roleName);

            if (user == null || !checkRole)
            {
                return BadRequest("Add role failed. Invalid email or role name.");
            }

            if (roleName == StaticUserRoles.Admin)
            {
                var admin = await _userManager.GetUsersInRoleAsync(roleName);
                if (admin != null)
                {
                    return BadRequest("Add role failed. An admin already exists.");
                }
            }

            await _userManager.AddToRoleAsync(user, roleName);
            return Ok("Add role successfully");

        }

        [HttpPost("remove-role")]
        public async Task<ActionResult> RemoveRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var checkRole = await _roleManager.RoleExistsAsync(roleName);
            if (user != null && checkRole)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
                return Ok($"{roleName} is removed from {email}");
            }
            return BadRequest("Remove role failed, email or role name is wrong");
        }

        [HttpDelete("remove-all-roles")]
        public async Task<ActionResult> RemoveAllRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var rolesUser = await _userManager.GetRolesAsync(user);
            if (user != null && rolesUser != null)
            {
                await _userManager.RemoveFromRolesAsync(user, rolesUser);
                return Ok($"{email} has been removed all roles");
            }
            return BadRequest("Remove role failed, email or role name is wrong");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);
            var signIn = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
            if(user.IsTokenValid)
            {
                if (checkPassword && user != null && signIn.Succeeded)
                {
                    RefreshTokenDto refreshTokenDto = await GenerateAccessToken(user);
                    await _unit.SaveChangesAsync();
                    return Ok(refreshTokenDto);
                }

                return Unauthorized("Email or Password is wrong");
            }

            return Unauthorized("You have been disable. Please contact Admin");
            
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
            var validateToken = await _unit.Authenticate.ValidateRefreshToken(user, refreshTokenDto.RefreshToken);
            if (user != null && validateToken)
            {
                await _signInManager.RefreshSignInAsync(user);
                RefreshTokenDto refreshToken = await GenerateAccessToken(user);
                await _unit.SaveChangesAsync();
                return Ok(refreshToken);
            }

            return Unauthorized("You are disable by Admin, please contact for more information");
        }

        [HttpPost("disable-user")]
        public async Task<ActionResult> DisabaleUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                user.IsTokenValid = false;
                await _userManager.UpdateAsync(user);

                return Ok("User has been disabled. Their token is invalidated");
            }

            return NotFound("User not found");
        }

        [HttpPost("enable-user")]
        public async Task<ActionResult> EnableUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.IsTokenValid = true;
                await _userManager.UpdateAsync(user);

                return Ok("User has been unlock. Their token is validated");
            }

            return NotFound("User not found");
        }

        [HttpPost("change-owner")]
        public async Task<ActionResult> ChangeOwner(string email, string password)
        {
            var newOwner = await _userManager.FindByEmailAsync(email);
            var token = await HttpContext.GetTokenAsync("access_token");
            if(token != null)
            {
                var oldOwner = await GetUserFromAccessToken(token);
                
                if(oldOwner != null && newOwner != null)
                {
                    var checkPassword = await _userManager.CheckPasswordAsync(oldOwner, password);
                    if(checkPassword && newOwner.IsTokenValid)
                    {
                        await _userManager.AddToRoleAsync(newOwner, StaticUserRoles.Admin);
                        await _userManager.RemoveFromRoleAsync(oldOwner, StaticUserRoles.Admin);
                        await _signInManager.SignOutAsync();
                        return Ok($"{oldOwner.Email} is no longer the owner of system,{newOwner.Email} become new owner of system.");
                    }
                    return BadRequest("Wrong password or selected user was disable");
                }
                return NotFound("Can't find the user");
            }
            return NotFound("Can't get the token");
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
                RefreshToken = (await _unit.Authenticate.GenerateRefreshToken(user.Id, token.Id)).Token,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = token.ValidTo
            };
            return refresTokenDto;
        }

        private JwtSecurityToken GenerateNewJWT(List<Claim> authClaims)
        {
            var authSerect = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(15),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSerect, SecurityAlgorithms.HmacSha512Signature)
                );
            return tokenObject;
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

        private IEnumerable<string> GetRoles()
        {
            yield return StaticUserRoles.Default;
            yield return StaticUserRoles.Crew;
            yield return StaticUserRoles.Pilot;
            yield return StaticUserRoles.GO;
            yield return StaticUserRoles.Admin;
        }
    }
}
