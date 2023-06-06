using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public PermissionController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet("get-all-permission")]
        public async Task<ActionResult<IEnumerable<Permission>>> GetAllPermission()
        {
            var listPermission = await _unit.Permission.GetAllAsync();
            return Ok(new
            {
                Message = "Success",
                Data= listPermission
            });
        }

        [HttpGet("get-permission-by-id")]
        public async Task<ActionResult<Permission>> GetPermissionById(int id)
        {
            var permission = await _unit.Permission.GetAsync(p => p.Id == id);
            if(permission == null)
            {
                return NotFound("Permission does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = permission
            });
        }

        [HttpPost("insert-permission")]
        public async Task<ActionResult<Permission>> InsertPermission(string permissionName)
        {
            if(permissionName == null)
            {
                return BadRequest();
            }

            var permission = new Permission();
            permission.Name = permissionName;
            await _unit.Permission.AddAsync(permission);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = permissionName
                });
            }

            return BadRequest("Something went wrong when adding");
        }
        
        [HttpPut("update-permission")]
        public async Task<ActionResult> UpdatePermission([FromForm] Permission permissionDto)
        {
            var permission = await _unit.Permission.GetAsync(p => p.Id == permissionDto.Id);
            if(permission == null)
            {
                return NotFound("Permission does not exists");
            }

            permission.Name = permissionDto.Name;
            _unit.Permission.Update(permission);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = permissionDto
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-permission")]
        public async Task<ActionResult> DeletePermission(int id)
        {
            var permission = await _unit.Permission.GetAsync(p => p.Id == id);
            if(permission == null)
            {
                return NotFound("Permission does not exists");
            }

            _unit.Permission.Delete(permission);
            var count = await _unit.SaveChangesAsync();
            if(count > 0)
            {
                return Ok("Delete successfully");
            }

            return BadRequest("Something went wrong when deleting");
        }
    }
}
