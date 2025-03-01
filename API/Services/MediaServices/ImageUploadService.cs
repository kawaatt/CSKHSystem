using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace API.Services.MediaServices
{
    public interface IImageUploadService
    {
        Task<string> UploadImageBase64Async(string base64Image, string system);
    }

    public class ImageUploadService : IImageUploadService
    {
        public ImageUploadService()
        {
        }

        public async Task<string> UploadImageBase64Async(string base64Image, string system)
        {
            try
            {
                // Remove data:image/jpeg;base64, prefix if it exists
                if (base64Image.Contains(","))
                {
                    base64Image = base64Image.Substring(base64Image.IndexOf(",") + 1);
                }

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                string extension = GetImageExtensionFromBase64(base64Image);
                string fileName = $"{Guid.NewGuid()}{extension}";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/UploadImage/{system}", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                await File.WriteAllBytesAsync(filePath, imageBytes);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                string imageUrl = $"UploadImage/{system}/{fileName}";

                return imageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return string.Empty;
            }
        }

        private string GetImageExtensionFromBase64(string base64Image)
        {
            var data = base64Image.Substring(0, 5).ToUpper();

            switch (data)
            {
                case "IVBOR":
                    return ".png";
                case "/9J/4":
                    return ".jpg";
                case "AAAAF":
                    return ".mp4";
                case "JVBER":
                    return ".pdf";
                case "AAABA":
                    return ".ico";
                case "UMFYI":
                    return ".rar";
                case "E1XYD":
                    return ".rtf";
                case "U1PKC":
                    return ".txt";
                case "MQOWM":
                case "77U/M":
                    return ".srt";
                default:
                    return string.Empty;
            }
        }
    }
}
