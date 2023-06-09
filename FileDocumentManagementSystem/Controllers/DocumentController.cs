using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Dropbox.Api;
using System.Security.Claims;
using AutoMapper;
using Dropbox.Api.Files;
using System;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IUnitOfWork _unit;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public DocumentController(IUnitOfWork unit, IMapper mapper, IConfiguration configuration)
        {
            _unit = unit;
            _mapper = mapper;
            _configuration = configuration;
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

        [HttpPost("insert-document-to-flight")]
        public async Task<ActionResult<Document>> InsertDocumentToFlight(IFormFile file, string flightId, string docTypeId )
        {
            const decimal version = 1;
            if(file == null || flightId == null || docTypeId == null)
            {
                var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
                if (userId == null)
                {
                    return StatusCode(500, "Something wrong when get userId");
                }

                var documentUploadUrl = await UploadFile(file.FileName, flightId);
                if (documentUploadUrl != null)
                {
                    Document newDocument = new Document
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = file.Name,
                        Version = version.ToString("0.0"),
                        Url = documentUploadUrl,
                        DateCreated = DateTime.Now.Date,
                        UserId = userId,
                        DocumentTypeId = docTypeId,
                        FlightId = flightId
                    };

                    await _unit.Document.AddAsync(newDocument);
                    var count = await _unit.SaveChangesAsync();

                    if (count > 0)
                    {
                        return Ok(new
                        {
                            Message = "Success",
                            Data = newDocument
                        });
                    }
                }

                return BadRequest("Insert unsuccessfully, can't upload file");
            }

            return BadRequest("Please fill all field");
            
        }

        [HttpPost("update-document")]
        public async Task<ActionResult> UpdateDocument(IFormFile file, string flightId, string typeDocId)
        {
            double version =1;
            if(file != null && flightId != null)
            {
                var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
                var fileName = file.FileName + "_" + version.ToString("0.0");
                var fileExists = await _unit.Document.CheckDocumentNameExistsInFlight(file.FileName, flightId);
                
                if (fileExists != null)
                {
                    version = int.Parse(fileExists.Version) + 0.1;
                    fileName = file.FileName + "_" + fileExists.Version;
                }
               
                var documentUploadUrl = await UploadFile(fileName, flightId);
                if (documentUploadUrl != null)
                {
                    Document newDocument = new Document
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = file.Name,
                        Version = version.ToString("0.0"),
                        Url = documentUploadUrl,
                        DateCreated = DateTime.Now.Date,
                        UserId = userId,
                        DocumentTypeId = typeDocId,
                        FlightId = flightId
                    };

                    await _unit.Document.AddAsync(newDocument);
                    var count = await _unit.SaveChangesAsync();
                    if(count > 0)
                    {
                        return Ok(new
                        {
                            Message = "Success",
                            Data = newDocument
                        });
                    }

                    return BadRequest("Something went wrong when adding to database");
                }
                return BadRequest("Error when uploading to cloud");
            }

            return BadRequest("Some field is not fill");
        }

        private async Task<int> UserPermission(string userId)
        {
            var userMember = await _unit.GroupMember.GetAsync(u => u.UserId == userId);
            var groupPermission = await _unit.GroupDocTypePermission.GetAsync(gp => gp.GroupId == userMember.GroupId);
            return groupPermission.PermissionId;
        }
        private async Task<string> UploadFile(string fileName, string flightId) 
        {
            var accessToken = _configuration["DropBox:AccessToken"];
            var client = new DropboxClient(accessToken);
            var filePath = "/" + flightId + "/" + fileName;

            using (var mem = new MemoryStream())
            {
                var upload = await client.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: mem);
                var uploadMetadata = await client.Files.GetMetadataAsync(filePath);
                if(uploadMetadata.IsFile)
                {
                    return filePath;
                }
                return null;
            }
        }

        private async Task DownloadFile(string filePath)
        {
            var accessToken = _configuration["DropBox:AccessToken"];
            var client = new DropboxClient(accessToken);
            var savePath = "C:\\Users\\ntkie\\OneDrive\\Desktop\\DownloadFile";

            using (var response = await client.Files.DownloadAsync(filePath))
            {
                using (var fileStream = System.IO.File.Create(savePath))
                {
                    var contentStream = await response.GetContentAsStreamAsync();
                    contentStream.CopyTo(fileStream);
                }
            }
        }
    }
}
