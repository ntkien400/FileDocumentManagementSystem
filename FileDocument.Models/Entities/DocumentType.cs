using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class DocumentType
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
