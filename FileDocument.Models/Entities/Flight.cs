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
        public string SourceAircraftId { get; set; }
        [ForeignKey("SourceAircraftId")]
        [Required]
        public string DestinationAircraftId { get; set; }
        [ForeignKey("DestinationAircraftId")]
        public Aircraft Aircraft { get; set; }
        [Required]
        public string AirportId { get; set; }
        [ForeignKey("AirportId")]
        public Airport Airport { get; set; }
    }
}
