using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string City { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}