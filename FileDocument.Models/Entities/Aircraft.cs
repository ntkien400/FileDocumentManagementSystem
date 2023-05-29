using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class Aircraft
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string AircraftNumber { get; set; }
        [Required]
        public string Manufacturer { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string YearOfManufacturer { get; set; }
        public string Status { get; set; }
    }
}
