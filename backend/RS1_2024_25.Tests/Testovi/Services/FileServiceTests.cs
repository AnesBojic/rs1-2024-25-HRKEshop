using Microsoft.AspNetCore.Hosting;
using Moq;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.Tests.Testovi.Services
{
    public class FileServiceTests : IDisposable
    {
        private readonly string _tempRoot;

        public FileServiceTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), "hrkeshop-file-service", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRoot);
        }

        [Fact]
        public async Task SaveFileAsync_WhenWebRootPathIsNull_CreatesWwwrootAndSavesFile()
        {
            var contentRoot = Path.Combine(_tempRoot, "content");
            Directory.CreateDirectory(contentRoot);

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns((string?)null);
            env.Setup(e => e.ContentRootPath).Returns(contentRoot);

            var service = new FileService(env.Object);
            var file = ImageHelper.CreateFakeFormFile("photo.webp", "image/webp");

            var savedPath = await service.SaveFileAsync(file, "users");
            var url = service.GeneratePublicUrl(savedPath);

            Assert.True(File.Exists(savedPath));
            Assert.Contains(Path.Combine("wwwroot", "images", "users"), savedPath);
            Assert.StartsWith("/images/users/", url);
            Assert.EndsWith(".webp", url);
        }

        [Fact]
        public async Task DeleteFile_RemovesSavedFile()
        {
            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(_tempRoot);

            var service = new FileService(env.Object);
            var savedPath = await service.SaveFileAsync(ImageHelper.CreateFakeFormFile(), "products");

            Assert.True(File.Exists(savedPath));
            service.DeleteFile(savedPath);
            Assert.False(File.Exists(savedPath));
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, true);
            }
        }
    }
}
