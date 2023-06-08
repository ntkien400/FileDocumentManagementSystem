using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Dtos
{
    public class FlightDto
    {
        public string DepartureDate { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public string SourceAirporttId { get; set; }
        public string DestinationAirporttId { get; set; }
        public string AircraftId { get; set; }
    }
}
