using System.ComponentModel.DataAnnotations;

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
    }
}
