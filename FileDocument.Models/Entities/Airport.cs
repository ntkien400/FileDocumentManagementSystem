using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class Airport
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(3)]
        public string AirportCode { get; set; }
    }
}
