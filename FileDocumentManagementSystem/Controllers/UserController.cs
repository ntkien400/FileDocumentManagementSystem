using AutoMapper;
using FileDocument.DataAccess.IRepository;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using FileDocumentManagementSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<User> _userManager;
        private readonly ISendEmail _sendEmail;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unit, IMapper mapper, UserManager<User> userManager, ISendEmail sendEmail)
        {
            _unit = unit;
            _mapper = mapper;
            _userManager = userManager;
            _sendEmail = sendEmail;
        }

        [HttpGet("get_all_user")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser(int? pageIndex)
        {
            var listUser = await _unit.User.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listUser, pageIndex);
            return Ok(paginationResult);
        }

        [HttpGet("get-user-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<User>> GetUserById(string userId)
        {
            var user = await _unit.User.GetAsync(a => a.Id == userId, properties:"Address");
            
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

        [HttpPost("create-user")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<RegisterDto>> CreateUser([FromForm] RegisterDto register)
        {
            var isExistEmail = await _userManager.FindByEmailAsync(register.Email);
            var isExistPhoneNumber = await _unit.User.GetAsync(x => x.PhoneNumber == register.PhoneNumber);
            var validEmail = _unit.User.CheckValidEmail(register.Email);
            var validPhoneNumber = _unit.User.CheckValidPhoneNumber(register.PhoneNumber);
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
                user.UserName = register.Email;
                var createUser = await _userManager.CreateAsync(user, register.Password);

                if (createUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, StaticUserRoles.Default);

                    if (user.Email == "ntkien400@gmail.com")
                    {
                        await _userManager.AddToRoleAsync(user, StaticUserRoles.Admin);
                    }
                    
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

        [HttpPost("forgot-password")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var uriBuilder = new UriBuilder("https://localhost:7102/api/User/reset-password");
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
        [AllowAnonymous]
        public ActionResult<ResetPasswordDto> ResetPassword(string token, string email)
        {
            var resetPassword = new ResetPasswordDto { Email = email, Token = token };
            return resetPassword;
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
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

        [HttpPut("update-user")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> UpdateUser([FromForm] RegisterDto updateUser, string userId)
        {
            var user = await _unit.User.GetAsync(a => a.Id == userId);
           
            if(user != null)
            {
                _mapper.Map(updateUser, user);
                _unit.User.Update(user);
                var count = await _unit.SaveChangesAsync();

                if(count > 0)
                {
                    return Ok("Update successfully");
                }

                return BadRequest("Something went wrong when updating");
            }

            return NotFound("User is not exists");
        }

        [HttpDelete("delete-user")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _unit.User.GetAsync(a => a.Id == userId);

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

        
    }
}
