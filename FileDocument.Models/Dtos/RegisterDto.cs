using FileDocument.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Dtos
{
    public class RegisterDto
    {
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool Gender { get; set; }
    }
}
