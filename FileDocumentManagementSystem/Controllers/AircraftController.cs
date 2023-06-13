using AutoMapper;
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
    public class AircraftController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public AircraftController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet("get-all-aircraft")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<Aircraft>>> GetAllAircraft(int? pageIndex)
        {
            var listAircraft = await _unit.Aircraft.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listAircraft, pageIndex);
            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpGet("get-aircraft-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Aircraft>> GetAircraftById(string aircraftId)
        {
            var aircraft = await _unit.Aircraft.GetAsync(a => a.Id == aircraftId);
            if(aircraft == null)
            {
                return NotFound("Id does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = aircraft
            });
        }

        [HttpPost("insert-aircraft")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Aircraft>> InsertAircarft([FromForm]AircraftDto aircraftDto)
        {
            var checkFieldNotNull = aircraftDto.GetType()
                                               .GetProperties()
                                               .Select(a => a.GetValue(aircraftDto))
                                               .Any(value => value != null);
            if(!checkFieldNotNull)
            {
                return BadRequest("Please fill all field");
            }

            var lastAircraft = await _unit.Aircraft.GetLastAircraft();
            int newNumber;
            if (lastAircraft != null)
            {
                var currentNumber = int.Parse(lastAircraft.Id.Split('-')[1]);
                newNumber = currentNumber + 1;
            }
            else
            {
                newNumber = 1;
            }

            Aircraft newAircraft = new Aircraft {Id = $"AIRCRAFT-{newNumber:D3}"};
            _mapper.Map(aircraftDto, newAircraft);
            await _unit.Aircraft.AddAsync(newAircraft);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = newAircraft
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-aircraft")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> UpdateAircraft(string aircraftId, [FromForm]AircraftDto aircraftDto)
        {
            var aircraft = await _unit.Aircraft.GetAsync(a => a.Id == aircraftId);
            if(aircraft == null)
            {
                return NotFound("Id does not exists");
            }

            _mapper.Map(aircraftDto, aircraft);
            _unit.Aircraft.Update(aircraft);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = aircraft
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-aircraft")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> DeleteAircraft(string aircraftId)
        {
            var aircraft = await _unit.Aircraft.GetAsync(a => a.Id == aircraftId);
            if (aircraft == null)
            {
                return NotFound("Id does not exists");
            }

            _unit.Aircraft.Delete(aircraft);
            var count = await _unit.SaveChangesAsync();

            if (count > 0)
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
