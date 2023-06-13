using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using FileDocumentManagementSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly UserManager<User> _userManager;

        public GroupMemberController(IUnitOfWork unit, UserManager<User> userManager)
        {
            _unit = unit;
            _userManager = userManager;
        }

        [HttpGet("get-members-by-group")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> GetMembersByGroup(string groupId, int? pageIndex)
        {
            var membersGroup = await _unit.GroupMember.GetAllAsync(m => m.GroupId == groupId);
            if (membersGroup == null)
            {
                return NotFound();
            }

            var listMember = new List<MemberGroupView>();
            foreach(var memberGroup in membersGroup)
            {
                var member = await GetUserWithRoles(memberGroup.UserId);
                listMember.Add(member);
            }

            var paginationResult = PaginationHelper.Paginate(listMember, pageIndex);
            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpPost("add-member")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> Addmember(string groupId,string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var group = await _unit.Group.GetAsync(g => g.Id == groupId);
            if(user == null || group == null)
            {
                return NotFound();
            }

            GroupMember groupUser = new GroupMember
            {
                UserId = user.Id,
                GroupId = group.Id
            };

            await _unit.GroupMember.AddAsync(groupUser);
            var count = await _unit.SaveChangesAsync();
            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success"
                });
            }

            return BadRequest("Something went wrong when adding");
        }
        
        [HttpDelete("delete-member")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> DeleteMember(string userId)
        {
            var member = await _unit.GroupMember.GetAsync(m => m.UserId == userId);
            if(member == null)
            {
                return NotFound("User does not exists");
            }

            _unit.GroupMember.Delete(member);
            var count = await _unit.SaveChangesAsync();
            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success"
                });
            }

            return BadRequest("Something went wrong when deleting");

        }

        private async Task<MemberGroupView> GetUserWithRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var member = new MemberGroupView();
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                member.Name = user.LastName + " " + user.FristName;
                member.Email = user.Email;
                member.Roles = roles;
            }

            return member;
        }

    }
}
