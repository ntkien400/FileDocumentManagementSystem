using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.Models.Entities
{
    public class Document
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string Version { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdate { get; set; }
        public bool Report { get; set; } = false;
        public string? SignatureUrl { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Required]
        public string DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentType { get; set; }
        [Required]
        public string FlightId { get; set; }
        [ForeignKey("FlightId")]
        public Flight Flight { get; set; }
    }
}
