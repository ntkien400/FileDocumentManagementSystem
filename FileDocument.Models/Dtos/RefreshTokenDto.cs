using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.Models.Dtos
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expires { get; set; }
    }
}
