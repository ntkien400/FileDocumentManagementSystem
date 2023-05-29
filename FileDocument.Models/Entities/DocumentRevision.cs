using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class DocumentRevision
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public DateTime DateRevision { get; set; }
        public string ChangeDescription { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
    }
}
