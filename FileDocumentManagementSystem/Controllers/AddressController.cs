using AutoMapper;
using FileDocument.DataAccess.Repository;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using FileDocumentManagementSystem.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

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
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddress(int? pageIndex)
        {
            var listAddress = await _unit.Address.GetAllAsync();
            var paginationResult = PaginationHelper.Paginate(listAddress, pageIndex);
            return Ok(new
            {
                Message = "Success",
                Data = paginationResult
            });
        }

        [HttpGet("get-address-by-user")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<Address>> GetAddressById(string userId)
        {
            var address = await _unit.Address.GetAsync(a => a.UserId == userId);
            
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
            
            return NotFound("Address not exists");
        }

        [HttpPost("insert-address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>>  InsertAddress([FromForm]AddressDto addressDto)
        {
            var checkFeildNull = addressDto.GetType()
                                           .GetProperties()
                                           .Select(a => a.GetValue(addressDto))
                                           .Any(value => value != null);
            if(checkFeildNull)
            {
                var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
                var address = await _unit.Address.GetAsync(a => a.UserId == userId);
                if(address != null)
                {
                    return BadRequest("User already have address");
                }
                address = new Address();
                address.UserId = userId;
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

        [HttpPut("update-address")]
        [Authorize]
        public async Task<ActionResult> UpdateAddress([FromForm] AddressDto addressDto)
        {
            var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var address = await _unit.Address.GetAsync(a => a.UserId == userId);
            
            if(address != null)
            {
                _mapper.Map(addressDto, address);
                _unit.Address.Update(address);
                var count = await _unit.SaveChangesAsync();
                
                if(count > 0)
                {
                    return Ok("Update successfully");
                }
                
                return BadRequest("Something went wrong when updating");
            }
            
            return NotFound("Address is not exists");
        }

        [HttpDelete("delte-address")]
        [Authorize(Roles = StaticUserRoles.Admin)]
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
                
                return BadRequest("Something went wrong when deleting");
            }
            
            return NotFound("Address is not exists");
        }
    }
}
