using AutoMapper;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigureController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public SystemConfigureController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet("get-all-system-config")]
        public async Task<ActionResult<IEnumerable<SystemConfigure>>> GetAllSystemConfigure()
        {
            var listConfig = await _unit.SystemConfigure.GetAllAsync();
            return Ok(new
            {
                Message = "Success",
                Data = listConfig
            });
        }

        [HttpGet("get-system-config-by-user")]
        public async Task<ActionResult<SystemConfigure>> GetSystemConfigureByUser(string userId)
        {
           var user = await _unit.User.GetAsync(u => u.Id == userId);
            if(user == null)
            {
                return NotFound();
            }

            var systemConfig = await _unit.SystemConfigure.GetAsync(sc => sc.UserId == userId);
            if(systemConfig== null)
            {
                return NotFound();
            }

            return Ok(new
            {
                Message = "Success",
                Data = systemConfig
            });

        }

        [HttpPost("insert-config")]
        public async Task<ActionResult> InsertConfigure([FromForm]SystemConfigureDto systemConfigDto)
        {
            var systemConfig = new SystemConfigure();
            var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            if(userId == null || systemConfigDto.File == null)
            {
                return BadRequest("Can't get user from token");
            }

            string fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(systemConfigDto.File.FileName);

            using (var fileStream = new FileStream(
                Path.Combine(@"Logo", fileName + extension),
                FileMode.Create))
            {
                systemConfigDto.File.CopyTo(fileStream);
            }

            systemConfig.LogoUrl = @"\Logo\" + fileName + extension;
            _mapper.Map(systemConfigDto, systemConfig);
            systemConfig.UserId = userId;
            await _unit.SystemConfigure.AddAsync(systemConfig);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Add successfully",
                    Data = systemConfigDto
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-system-config")]
        public async Task<ActionResult> UpdateSystemConfig(int id, [FromForm]SystemConfigureDto systemConfigDto)
        {
            var systemConfig = await _unit.SystemConfigure.GetAsync(s => s.Id == id);
            if(systemConfig == null)
            {
                return NotFound("Id does not exists");
            }

            _mapper.Map(systemConfigDto, systemConfig);
            _unit.SystemConfigure.Update(systemConfig);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Update successfully",
                    Data = systemConfigDto
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-system-config")]
        public async Task<ActionResult> DeleteSystemConfigure(int id)
        {
            var systemConfig = await _unit.SystemConfigure.GetAsync(s => s.Id == id);
            if(systemConfig == null)
            {
                return NotFound("Id does not exists");
            }

            _unit.SystemConfigure.Delete(systemConfig);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Delete successfully"
                });
            }

            return BadRequest("Something went wrong when deleting");
        }
    }
}
