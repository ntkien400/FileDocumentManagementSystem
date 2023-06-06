using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class GroupDocTypePermission
    {
        [Key]
        public int Id { get; set; }
        public string GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
        public string DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentType DocumentType { get; set; }
        public int PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }
}
