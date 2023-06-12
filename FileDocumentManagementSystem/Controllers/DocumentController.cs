using AutoMapper;
using Dropbox.Api;
using Dropbox.Api.Files;
using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            if (listDocument.Count() == 0)
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
            if (searchRequest != null)
            {
                var listDocument = await _unit.Document.GetAllAsync(d => d.FlightId == searchRequest);
                if (listDocument.Count() > 0)
                {
                    return Ok(new
                    {
                        Message = "Success",
                        Data = listDocument
                    });
                }

                listDocument = await _unit.Document.GetAllAsync(d => d.Name == searchRequest);
                if (listDocument.Count() > 0)
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
        public async Task<ActionResult<Document>> InsertDocumentToFlight(IFormFile file, string flightId, string docTypeId)
        {
            const decimal version = 1;
            if (file != null && flightId != null && docTypeId != null)
            {
                var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_ver" + version.ToString("0.0") + Path.GetExtension(file.FileName);
                var documentUploadUrl = await UploadFile(fileName, flightId);
                var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);

                if (documentUploadUrl != null && flight != null && !flight.FlightEnd)
                {
                    Document newDocument = new Document
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = file.FileName,
                        Version = version.ToString("0.0"),
                        Url = documentUploadUrl,
                        DateCreated = DateTime.Now,
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

        [HttpPost("updload-document")]
        public async Task<ActionResult> UpdloadDocument(IFormFile file, string flightId, string typeDocId)
        {
            if (file == null || flightId == null || typeDocId == null)
            {
                return BadRequest("All fields are not filled");
            }

            double version = 1;
            var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
            var userPermission = await _unit.Document.GetUserPermission(userId, typeDocId);
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);

            if (userPermission != 1 || flight.IsDocumentReported)
            {
                return BadRequest("You don't have permission to update or Flight Document was reported");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_ver" + version.ToString("0.0") + Path.GetExtension(file.FileName);
            var fileExists = await _unit.Document.CheckDocumentNameExistsInFlight(file.FileName, flightId);

            if (fileExists != null)
            {
                version = double.Parse(fileExists.Version) + 0.1;
                fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_ver" + version.ToString("0.0") + Path.GetExtension(file.FileName);
            }

            var documentUploadUrl = await UploadFile(fileName, flightId);

            if (documentUploadUrl == null)
            {
                return BadRequest("Error when uploading to cloud");
            }

            Document newDocument = new Document
            {
                Id = Guid.NewGuid().ToString(),
                Name = file.FileName,
                Version = version.ToString("0.0"),
                Url = documentUploadUrl,
                DateCreated = DateTime.Now,
                UserId = userId,
                DocumentTypeId = typeDocId,
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

            return BadRequest("Something went wrong when adding to the database");

        }

        [HttpPut("update-document")]
        public async Task<ActionResult> UpdateDocument(IFormFile file, string documentId, string typeDocId, string flightId)
        {
            if (file == null || documentId == null || typeDocId == null)
            {
                return BadRequest("All fields are not filled");
            }

            var document = await _unit.Document.GetAsync(d => d.Id == documentId);
            var typeDoc = await _unit.DocumentType.GetAsync(td => td.Id == typeDocId);
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);

            if (document == null || typeDoc == null || flight == null || flight.FlightEnd)
            {
                return BadRequest("Id does not exist or flight has ended");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_ver" + document.Version + Path.GetExtension(file.FileName);
            var deleteResult = await DeleteFile(document.Name, flightId);

            if (deleteResult)
            {
                var documentUrl = await UploadFile(file.FileName, flightId);
                document.Url = documentUrl;
                document.DocumentTypeId = typeDocId;
                document.FlightId = flightId;
                document.DateUpdate = DateTime.Now;

                _unit.Document.Update(document);
                var count = await _unit.SaveChangesAsync();

                if (count > 0)
                {
                    return Ok(new { Message = "Success", Data = document });
                }

                return BadRequest("Something went wrong when updating");
            }

            return BadRequest("Error when deleting document in the cloud");

        }

        [HttpPost("report-document")]
        public async Task<ActionResult> ReportDocument(string flightId, IFormFile file)
        {
            var listDocByFlight = await _unit.Document.FilterDocuments(flightId);
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);
            if (listDocByFlight.Any() && file != null && !flight.FlightEnd)
            {
                var signatureUploadUrl = await UploadFile(flightId, file.FileName);
                foreach (var document in listDocByFlight)
                {
                    document.SignatureUrl = signatureUploadUrl;
                }

                flight.IsDocumentReported = true;
                _unit.Document.UpdateRange(listDocByFlight);
                _unit.Flight.Update(flight);
                var count = await _unit.SaveChangesAsync();

                if (count > 0)
                {
                    return Ok(new { Message = "Report successfully" });
                }

                return BadRequest("Something went wrong when updating");
            }

            return BadRequest("Failed to report");
        }

        [HttpDelete("delete-document")]
        public async Task<ActionResult> DeleteDocument(string documentId, string flightId)
        {
            var document = await _unit.Document.GetAsync(d => d.Id == documentId);
            var flight = await _unit.Flight.GetAsync(f => f.Id == flightId);
            var userId = HttpContext.User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;

            if (document != null && document.UserId == userId && !flight.IsDocumentReported)
            {
                _unit.Document.Delete(document);
                var count = await _unit.SaveChangesAsync();
                if (count > 0)
                {
                    return Ok(new { Message = "Success" });
                }

                return BadRequest("Something went wrong when deleting");
            }

            return BadRequest("Can't not delete because wrong document Id or document was reported");
        }

        [HttpGet("get-user-permission")]
        public async Task<ActionResult> GetUserPermission(string userId, string docTypeId)
        {
            var userPermission = await _unit.Document.GetUserPermission(userId, docTypeId);
            if (userPermission != 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = userPermission
                });
            }

            return BadRequest("User Id is not exists");
        }

        [HttpGet("download-file")]
        public async Task DownloadFile(string filePath)
        {
            var accessToken = _configuration["DropBox:AccessToken"];
            var client = new DropboxClient(accessToken);
            var savePath = "C:\\Users\\ntkie\\Downloads\\Documents\\New folder";

            using (var response = await client.Files.DownloadAsync(filePath))
            {
                using (var fileStream = System.IO.File.Create(savePath))
                {
                    var contentStream = await response.GetContentAsStreamAsync();
                    contentStream.CopyTo(fileStream);
                }
            }
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
                if (uploadMetadata.IsFile)
                {
                    return filePath;
                }
                return null;
            }
        }
        private async Task<bool> DeleteFile(string fileName, string flightId)
        {
            var accessToken = _configuration["DropBox:AccessToken"];
            var client = new DropboxClient(accessToken);
            var filePath = "/" + flightId + "/" + fileName;

            using (var mem = new MemoryStream())
            {
                var upload = await client.Files.DeleteV2Async(filePath);
                var metadata = await client.Files.GetMetadataAsync(filePath);
                if (metadata.IsFile)
                {
                    return false;
                }
                return true;
            }
        }

    }
}
