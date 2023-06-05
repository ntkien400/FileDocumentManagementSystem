using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class SystemConfigure
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Theme { get; set; }
        [Required]
        public string LogoUrl { get; set; }
        [Required]
        public bool Captcha { get; set; } = false;
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
