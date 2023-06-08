using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Dropbox.Api;
using FileDocument.Models.Dtos;
using System.Security.Claims;
using AutoMapper;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        public DocumentController(IUnitOfWork unit, IMapper mapper)
        {
            _unit = unit;
            _mapper = mapper;
        }

        [HttpGet("get-all-document")]
        public async Task<ActionResult<IEnumerable<Document>>> GetAllDocument()
        {
            var listDocument = await _unit.Document.GetAllAsync();
            return Ok(new
            {
                Message = "Success",
                Data = listDocument
            });
        }

        [HttpGet("filter-document")]
        public async Task<ActionResult<IEnumerable<Document>>> FilterDocument(string? flightId, string? departureDate, string? typeDocumentId)
        {
            var listDocument = await _unit.Document.FilterDocuments(flightId, departureDate, typeDocumentId);
            if(listDocument == null)
            {
                return NotFound("Not found any document");
            }

            return Ok(new
            {
                Message = "Success",
                Data = listDocument
            });
        }

        [HttpGet("search-document")]
        public async Task<ActionResult<IEnumerable<Document>>> SearchDocument(string searchRequest)
        {
            if(searchRequest != null)
            {
                var listDocument = await _unit.Document.GetAllAsync(d => d.FlightId == searchRequest);
                if(listDocument != null)
                {
                    return Ok(new
                    {
                        Message = "Success",
                        Data = listDocument
                    });
                }

                listDocument = await _unit.Document.GetAllAsync(d => d.Name == searchRequest);
                if(listDocument != null)
                {
                    return Ok(new
                    {
                        Message = "Success",
                        Data = listDocument
                    });
                }
                
                return NotFound("Don't have any document fit with your search");
            }

            return BadRequest("Please fill in the search bar");
        }

        [HttpPost("insert-document")]
        public async Task<ActionResult<Document>> InsertDocument(IFormFile file, string flightId, string docTypeId )
        {
            const decimal version = 1;
            var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            if(userId == null)
            {
                return Unauthorized("You need login to do this action");
            }

            Document newDocument = new Document
            {
                Id = Guid.NewGuid().ToString(),
                Name = file.Name,
                Version = version.ToString("0.0"),
                DateCreated = DateTime.Now.Date,
                UserId = userId,
                DocumentTypeId = docTypeId,
                FlightId = flightId
            };
        }
    }
}
