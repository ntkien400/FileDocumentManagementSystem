using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileDocument.Models.Entities
{
    public class GroupMember
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }

    }
}
