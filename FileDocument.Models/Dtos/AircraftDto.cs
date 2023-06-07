using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Dtos
{
    public class AircraftDto
    {
        public string? AircraftNumber { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? YearOfManufacturer { get; set; }
        public StatusOptions Status { get; set; }
    }

    public enum StatusOptions
    {
        Loading,
        Unloading
    }
}
