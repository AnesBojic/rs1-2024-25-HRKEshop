using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subdirectory)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }
            if (!isAllowedExtension(file.FileName))
            {
                throw new ArgumentException("Not allowed extension for image!");
            }
            if (!isWithingTheSizeLimit(file.Length))
            {
                throw new ArgumentException("It is exceeding limits!");
            }
            if (!ImageHelper.isValid(subdirectory))
            {
                throw new ArgumentException("Not valid type!");
            }

            var webRoot = GetWebRootPath();
            Directory.CreateDirectory(webRoot);

            var safeSubdirectory = subdirectory.Trim().ToLowerInvariant();
            string folderPath = Path.Combine(webRoot, "images", safeSubdirectory);
            Directory.CreateDirectory(folderPath);

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(folderPath, uniqueFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        public bool isAllowedExtension(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;
            var extension = Path.GetExtension(filename).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        public bool isWithingTheSizeLimit(long size)
        {
            return size <= MaxFileSize;
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var fullPath = ResolveFullPath(filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public string GeneratePublicUrl(string filePath)
        {
            var webRoot = GetWebRootPath();
            var fullPath = ResolveFullPath(filePath);
            var relativePath = Path.GetRelativePath(webRoot, fullPath).Replace("\\", "/");

            if (relativePath.StartsWith(".."))
            {
                return $"/images/{Path.GetFileName(fullPath)}";
            }

            return "/" + relativePath.TrimStart('/');
        }

        private string GetWebRootPath()
        {
            if (!string.IsNullOrWhiteSpace(_env.WebRootPath))
            {
                return _env.WebRootPath;
            }

            var contentRoot = !string.IsNullOrWhiteSpace(_env.ContentRootPath)
                ? _env.ContentRootPath
                : Directory.GetCurrentDirectory();

            var fallback = Path.Combine(contentRoot, "wwwroot");
            Directory.CreateDirectory(fallback);
            return fallback;
        }

        private string ResolveFullPath(string filePath)
        {
            if (Path.IsPathRooted(filePath))
            {
                return filePath;
            }

            var relative = filePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(GetWebRootPath(), relative);
        }
    }
}
