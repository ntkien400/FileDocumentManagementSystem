using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
