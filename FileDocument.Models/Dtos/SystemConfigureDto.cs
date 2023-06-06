using Microsoft.AspNetCore.Http;

namespace FileDocument.Models.Dtos
{
    public class SystemConfigureDto 
    {
        public ThemeOptions Theme { get; set; }
        public IFormFile? File { get; set; }
        public bool Captcha { get; set; } = false;
        
    }

    public enum ThemeOptions
    {
        Default,
        Light,
        Dark
    }
}
