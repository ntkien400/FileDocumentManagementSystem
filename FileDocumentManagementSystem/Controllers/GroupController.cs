using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using FileDocumentManagementSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace FileDocumentManagementSystem.Controllers
{
    public class GroupController : Controller
    {
        private readonly IUnitOfWork _unit;

        public GroupController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet("get-group-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Group>> GetGroupById(string id)
        {
            var group = await _unit.Group.GetAsync(g => g.Id == id);
            if(group == null)
            {
                return NotFound("Group does not exists");
            }
            return Ok(group);
        }

        [HttpGet("get-all-group")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroup(int? pageIndex)
        {
            var listGroup = await _unit.Group.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listGroup, pageIndex);

            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpPost("create-group")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Group>> CreateGroup(CreateGroupDto groupDto)
        {
            var group = await _unit.Group.GetAsync(g => g.Name == groupDto.Name);
            if( group != null)
            {
                return BadRequest($"{groupDto.Name} have been used, please use another name");
            }
            else
            {
                var lastGroup = await _unit.Group.GetLastGroup();

                int newNumber;
                if (lastGroup != null)
                {
                    var currentNumber = int.Parse(lastGroup.Id.Split('-')[1]);
                    newNumber = currentNumber + 1;
                }
                else
                {
                    newNumber = 1;
                }

                var newGroupId = $"GROUP-{newNumber:D2}"; //  newNumber should have 2 digits
                var newGroup = new Group 
                { 
                    Id = newGroupId,
                    Name = groupDto.Name,
                    Note = groupDto.Note,
                    DateCreated = DateTime.Now,
                    Creator = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value
                };
                
                await _unit.Group.AddAsync(newGroup);
                var count = await _unit.SaveChangesAsync();
                if(count > 0)
                {
                    return Ok(new
                    {
                        Message = "Create successfully",
                        Data = newGroup
                    });

                }

                return BadRequest("Something wrong when adding");

            }
        }
        
        [HttpPut("update-group")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> UpdateGroup(CreateGroupDto groupDto, string id)
        {
            var group = await _unit.Group.GetAsync(g => g.Id == id);
            if(group == null)
            {
                return NotFound("Group does not exists");
            }

            group.Name = groupDto.Name;
            group.Note = groupDto.Note;
            _unit.Group.Update(group);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Create successfully",
                    Data = group
                });
            }

            return BadRequest("Something wrong when updating");
        }

        [HttpDelete("delete-group")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> Delete(string id)
        {
            var group = await _unit.Group.GetAsync(g => g.Id == id);

            if(group == null)
            {
                return NotFound("Group does not exists");
            }

            _unit.Group.Delete(group);
            var count = await _unit.SaveChangesAsync();
            if(count > 0)
            {
                return Ok("Delete successfully");
            }

            return BadRequest("Something wrong when deleting");
        }
    }
}
