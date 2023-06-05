namespace FileDocument.Models.Dtos
{
    public class MemberGroupView
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
