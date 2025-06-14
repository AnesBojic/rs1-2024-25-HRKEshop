using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;


        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly List<string> AllowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif" };

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subdirectory)
        {
            if(file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }
            if(!isAllowedExtension(file.FileName))
            {
                throw new ArgumentException("Not allowed extension for image!");
            }
            if(!isWithingTheSizeLimit(file.Length))
            {
                throw new ArgumentException("It is exceeding limits!");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "images", subdirectory);
            Directory.CreateDirectory(folderPath);

            string extension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string fullPath  =Path.Combine(folderPath, uniqueFileName);
            
            using(var stream = new FileStream(fullPath,FileMode.Create))
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
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public string GeneratePublicUrl(string filePath)
        {
            var relativePath = filePath.Replace(_env.WebRootPath, "").Replace("\\", "/");
            return $"/{relativePath.TrimStart('/')}";
        }
    }
}
