using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FristName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool Gender { get; set; }
        public bool Disable { get; set; } = false;
        public Address Address { get; set; }
    }
}
