using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigureController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public SystemConfigureController(IUnitOfWork unit)
        {
            _unit = unit;
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

        // POST api/<SystemConfigureController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SystemConfigureController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SystemConfigureController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
