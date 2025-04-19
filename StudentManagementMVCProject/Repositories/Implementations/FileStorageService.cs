using StudentManagementMVCProject.Repositories.Interfaces;
using static NuGet.Packaging.PackagingConstants;

namespace StudentManagementMVCProject.Repositories.Implementations
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<bool> DeleteImageAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || filePath.Contains("default"))
                return false;

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file extension.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("File size exceeds 2MB.");

            var fileName = Guid.NewGuid().ToString() + extension;
            var folder = Path.Combine("images", "profiles");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            var fullPath = Path.Combine(uploadsFolder, fileName);

            Directory.CreateDirectory(uploadsFolder);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}".Replace("\\", "/");
        }
    }
}
