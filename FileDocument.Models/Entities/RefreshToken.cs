using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace FileDocument.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExpried { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
