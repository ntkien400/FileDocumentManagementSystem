using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.Models.Dtos
{
    public class ResetPasswordDto
    {
        public string Password { get; set; }
        public string ConfrimPassword { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
