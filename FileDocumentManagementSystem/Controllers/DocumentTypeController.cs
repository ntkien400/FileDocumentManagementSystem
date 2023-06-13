using FileDocument.DataAccess.UnitOfWork;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileDocumentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IUnitOfWork _unit;

        public DocumentTypeController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        [HttpGet("get-all-document-type")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<DocumentType>> GetAllDocumentType()
        {
            var listDocumentType = await _unit.DocumentType.GetAllAsync();
            return Ok(new
            {
                Message = "Success",
                Data = listDocumentType
            });
        }

        [HttpGet("get-document-type-by-id")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<DocumentType>> GetDocumentTypeById(string id)
        {
            var documentType = await _unit.DocumentType.GetAsync(d => d.Id == id);
            if(documentType == null)
            {
                return NotFound("Id does not exists");
            }

            return Ok(new
            {
                Message = "Success",
                Data = documentType
            });
        }

        [HttpPost("insert-document-type")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<DocumentType>> InsertDocumentType(string typeName)
        {
            var isFileNameExists = await _unit.DocumentType.GetAsync(d => d.Name == typeName);
            if(isFileNameExists != null)
            {
                return BadRequest("Document type name already exists");
            }

            var lastDocType = await _unit.DocumentType.GetLastDocumentType();

            int newNumber;
            if (lastDocType != null)
            {
                var currentNumber = int.Parse(lastDocType.Id.Split('-')[1]);
                newNumber = currentNumber + 1;
            }
            else
            {
                newNumber = 1;
            }

            var newDocTypeId = $"TYPE-{newNumber:D2}"; 
            DocumentType documentType = new DocumentType
            {
                Id = newDocTypeId,
                Name = typeName,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };

            await _unit.DocumentType.AddAsync(documentType);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = documentType
                });
            }

            return BadRequest("Something went wrong when adding");
        }

        [HttpPut("update-document-type")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult<DocumentType>> UpdateDocumentType(string id, string typeName)
        {
            var documentType = await _unit.DocumentType.GetAsync(d => d.Id == id);
            if(documentType == null)
            {
                return NotFound("Id does not found");
            }

            documentType.Name = typeName;
            documentType.DateUpdated = DateTime.Now;
            _unit.DocumentType.Update(documentType);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
            {
                return Ok(new
                {
                    Message = "Success",
                    Data = documentType
                });
            }

            return BadRequest("Something went wrong when updating");
        }

        [HttpDelete("delete-document-type")]
        [Authorize(Roles = StaticUserRoles.Admin)]
        public async Task<ActionResult> DeleteDocumentType(string id)
        {
            var documentType = await _unit.DocumentType.GetAsync(d => d.Id == id);
            if(documentType == null)
            {
                return NotFound("Id does not found");
            }

            _unit.DocumentType.Delete(documentType);
            var count = await _unit.SaveChangesAsync();

            if(count > 0)
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
