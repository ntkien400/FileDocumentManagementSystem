using AutoMapper;
using FileDocument.DataAccess.Repository;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;

        public AddressController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet("get_all_address")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddress()
        {
            var listAddress = await _unit.Address.GetAllAsync();
            return Ok(listAddress);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddressById(int id)
        {
            var address = await _unit.Address.GetAsync(a => a.Id == id);
            if (address != null)
            {
                return Ok(new Address
                {
                    Id = address.Id,
                    HouseNumber = address.HouseNumber,
                    Street = address.Street,
                    Ward = address.Ward,
                    District = address.District,
                    City = address.City,
                    UserId = address.UserId
                });
            }
            else
            {
                return BadRequest("Address not exists");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AddressDto>>  InsertAddress([FromForm]AddressDto addressDto)
        {
            var checkFeildNull = addressDto.GetType()
                                           .GetProperties()
                                           .Select(a => a.GetValue(addressDto))
                                           .Any(value => value != null);
            if(checkFeildNull)
            {
                var address = new Address();
                _mapper.Map(addressDto, address);
                await _unit.Address.AddAsync(address);
                var count = await _unit.SaveChangesAsync();
                if (count > 0)
                {
                    return Ok(addressDto);
                }
                else
                {
                    return BadRequest("Something went wrong when adding");
                }
            }
            else
            {
                return BadRequest("You must fill all fields");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAddress([FromForm] AddressDto addressDto, int id)
        {
            var address = await _unit.Address.GetAsync(a => a.Id == id);
            if(address != null)
            {
                _mapper.Map(addressDto, address);
                await _unit.Address.AddAsync(address);
                var count = await _unit.SaveChangesAsync();
                if(count > 0)
                {
                    return Ok("Update successfully");
                }
                else
                {
                    return BadRequest("Something went wrong when updating");
                }
            }
            else
            {
                return BadRequest("Address is not exists");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            var address = await _unit.Address.GetAsync(a => a.Id == id);
            if(address != null)
            {
                _unit.Address.Delete(address);
                var count = await _unit.SaveChangesAsync();
                if(count > 0)
                {
                    return Ok("Delete successfully");
                }
                else
                {
                    return BadRequest("Something went wrong when deleting");
                }
            }
            else
            {
                return BadRequest("Address is not exists");
            }
        }
    }
}
