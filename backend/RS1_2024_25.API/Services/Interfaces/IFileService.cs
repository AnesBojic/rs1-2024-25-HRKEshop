namespace RS1_2024_25.API.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subdirectory);
        bool isAllowedExtension(string filename);

        bool isWithingTheSizeLimit(long size);

        void DeleteFile(string filePath);

        string GeneratePublicUrl(string filePath);


    }
}
