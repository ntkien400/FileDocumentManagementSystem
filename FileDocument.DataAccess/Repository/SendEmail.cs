using FileDocument.DataAccess.IRepository;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FileDocument.DataAccess.Repository
{
    public class SendEmail : ISendEmail
    {
        public async Task<bool> SendResetPasswordAsync(string email, string passwordResetUrl)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ntkien400@gmail.com", "FestivalHue");
            var subject = "Mã khôi phục mật khẩu";
            var to = new EmailAddress(email, "User");
            var plainTextContent = "Success";
            var htmlContent = "Nhấn vào đường link này để reset mật khẩu của bạn:\n" + $"<a href='{ passwordResetUrl}'>Reset Link</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var result = await client.SendEmailAsync(msg);

            return result.IsSuccessStatusCode ? true : false;
        }
    }
}
