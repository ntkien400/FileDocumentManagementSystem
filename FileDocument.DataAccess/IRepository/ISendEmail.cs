using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.DataAccess.IRepository
{
    public interface ISendEmail
    {
        Task<bool> SendResetPasswordAsync(string email, string passwordResetUrl);
    }
}
