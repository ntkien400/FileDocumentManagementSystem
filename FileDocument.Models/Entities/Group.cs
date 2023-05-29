using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class Group
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public string Creator { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
