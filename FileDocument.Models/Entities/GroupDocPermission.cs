using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class GroupDocPermission
    {
        [Key]
        public int Id { get; set; }
        public string GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        public string DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }
}
