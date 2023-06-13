using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using FileDocumentManagementSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupDocTypePermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public GroupDocTypePermissionController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet("get-all-group-document-type-permission")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<GroupDocTypePermission>>> GetAllGroupDocumentTypePermission(int? pageIndex)
        {
            var listDocTypePermission = await _unit.GroupDocTypePermission.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listDocTypePermission, pageIndex);
            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpGet("get-group-document-type-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<GroupDocTypePermission>> GetGroupDocumentTypePermissionById(int id)
        {
            var docTypePermission = await _unit.GroupDocTypePermission.GetAsync(d => d.Id == id);
            if(docTypePermission == null)
            {
                return NotFound("Id does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = docTypePermission
            });
        }

        [HttpPost("insert-group-document-type-permission")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<GroupDocTypePermission>> InsertGroupDocumentTypePermission(string groupId, string documentTypeId, int permissionId)
        {
            var group = await _unit.Group.GetAsync(g => g.Id == groupId);
            var docType = await _unit.DocumentType.GetAsync(d => d.Id == documentTypeId);
            var permission = await _unit.Permission.GetAsync(p => p.Id == permissionId);

            if(group == null || docType == null || permission == null)
            {
                return NotFound("Id does not exists");
            }

            var groupDocTypePermission = new GroupDocTypePermission
            {
                GroupId = groupId,
                DocumentTypeId = documentTypeId,
                PermissionId = permissionId
            };
            await _unit.GroupDocTypePermission.AddAsync(groupDocTypePermission);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = groupDocTypePermission
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-group-document-type-permission")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<GroupDocTypePermission>> UpdateGroupDocumentTypePermission(int id, string groupId, string documentTypeId, int permissionId)
        {
            var group = await _unit.Group.GetAsync(g => g.Id == groupId);
            var docType = await _unit.DocumentType.GetAsync(d => d.Id == documentTypeId);
            var permission = await _unit.Permission.GetAsync(p => p.Id == permissionId);
            var groupDocTypePermission = await _unit.GroupDocTypePermission.GetAsync(gd => gd.Id == id);
            if (group == null || docType == null || permission == null || groupDocTypePermission == null)
            {
                return NotFound("Id does not exists");
            }

            groupDocTypePermission.GroupId = groupId;
            groupDocTypePermission.DocumentTypeId = documentTypeId;
            groupDocTypePermission.PermissionId = permissionId;

            _unit.GroupDocTypePermission.Update(groupDocTypePermission);
            var count = await _unit.SaveChangesAsync();
            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = groupDocTypePermission
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-group-document-type-permission")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> DeleteGroupDcoumentTypePermission(int id)
        {
            var groupDocumentTypePermission = await _unit.GroupDocTypePermission.GetAsync(g => g.Id == id);
            if(groupDocumentTypePermission == null)
            {
                return NotFound("Id does not exists");
            }

            _unit.GroupDocTypePermission.Delete(groupDocumentTypePermission);
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
    }
}
