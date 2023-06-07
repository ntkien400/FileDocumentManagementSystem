using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class Flight
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime DepartureDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime ArrivalDate { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan DepartureTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan ArrivalTime { get; set; }
        [Required]
        public string SourceAirporttId { get; set; }
        [ForeignKey("SourceAirporttId")]
        public Airport Airport1 { get; set; }
        [Required]
        public string DestinationAirporttId { get; set; }
        [ForeignKey("DestinationAirporttId")]
        public Airport Airport2 { get; set; }
        [Required]
        public string AircraftId { get; set; }
        [ForeignKey("AircraftId")]
        public Aircraft Aircraft { get; set; }
    }
}
