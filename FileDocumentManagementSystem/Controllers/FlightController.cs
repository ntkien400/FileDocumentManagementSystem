using AutoMapper;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public FlightController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet("get-all-flight")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetAllFlight()
        {
            var listFlight = await _unit.Flight.GetAllAsync();
            return Ok(new
            {
                Message = "Success",
                Data = listFlight
            });
        }

        [HttpGet("get-flight-by-id")]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlightById(string flightId)
        {
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);
            if (flight == null)
            {
                return NotFound("Id does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = flight
            });
        }

        [HttpPost("insert-flight")]
        public async Task<ActionResult<Flight>> InsertFlight([FromForm] FlightDto flightDto)
        {
            var checkFieldNull = flightDto.GetType()
                                          .GetProperties()
                                          .Select(f => f.GetValue(flightDto))
                                          .Any(value => value == null);
            if (checkFieldNull)
            {
                return BadRequest("Please fill all field");
            }

            var lastFlight = await _unit.Flight.GetLastFlight();
            int newNumber;
            if (lastFlight != null)
            {
                var currentNumber = int.Parse(lastFlight.Id.Substring(2,3));
                newNumber = currentNumber + 1;
            }
            else
            {
                newNumber = 1;
            }

            Flight newFlight = new Flight { Id = $"VJ{newNumber:D3}" };
            _mapper.Map(flightDto, newFlight);
            await _unit.Flight.AddAsync(newFlight);
            var count = await _unit.SaveChangesAsync();

            if (count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = newFlight
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-flight")]
        public async Task<ActionResult<Flight>> UpdateFlight(string flightId,[FromForm] FlightDto flightDto)
        {
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);
            if(flight == null)
            {
                return NotFound("Id does not exists");
            }

            _mapper.Map(flightDto, flight);
            _unit.Flight.Update(flight);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = flight
                });
            }

            return BadRequest("Something went wrong when upadating");
        }
    }
}
