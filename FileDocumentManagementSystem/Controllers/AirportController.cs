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
    public class AirportController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public AirportController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet("get-all-airport")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<Airport>>> GetAllAirport(int? pageIndex)
        {
            var listAirport = await _unit.Airport.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listAirport, pageIndex);
            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpGet("get-airport-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<Airport>>> GetAirpoortById(string airportId)
        {
            var airport = await _unit.Airport.GetAsync(a => a.Id == airportId);
            if (airport == null)
            {
                return NotFound("Id does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = airport
            });
        }

        [HttpPost("insert-airport")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Airport>> InsertAirport(string name, string airportCode)
        {
            if (name == null || airportCode == null)
            {
                return BadRequest("Please fill all field");
            }

            var lastAirport = await _unit.Airport.GetLastAirport();
            int newNumber;
            if (lastAirport != null)
            {
                var currentNumber = int.Parse(lastAirport.Id.Split('-')[1]);
                newNumber = currentNumber + 1;
            }
            else
            {
                newNumber = 1;
            }

            Airport newAirport = new Airport
            {
                Id = $"AIRPORT-{newNumber:D2}",
                AirportCode = airportCode,
                Name = name
            };

            await _unit.Airport.AddAsync(newAirport);
            var count = await _unit.SaveChangesAsync();
            if (count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = newAirport
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-airport")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Airport>> UpdateAirport(string airportId, string name, string airportCode)
        {
            var airport = await _unit.Airport.GetAsync(a => a.Id == airportId);
            if (airport == null)
            {
                return NotFound("Id does not exists");
            }

            airport.Name = name;
            airport.AirportCode = airportCode;
            _unit.Airport.Update(airport);
            var count = await _unit.SaveChangesAsync();

            if (count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = airport
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-airport")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Airport>> DeleteAirport(string airportId)
        {
            var airport = await _unit.Airport.GetAsync(a => a.Id == airportId);
            if (airport == null)
            {
                return NotFound("Id does not exists");
            }

            _unit.Airport.Delete(airport);
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
