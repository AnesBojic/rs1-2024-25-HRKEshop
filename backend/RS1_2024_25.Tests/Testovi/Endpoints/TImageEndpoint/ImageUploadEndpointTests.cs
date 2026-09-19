using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ImageEndpoints;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;
using RS1_2024_25.Tests.Services;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    public class ImageUploadEndpointTests : IDisposable
    {
        private readonly ApplicationDbContext _db;
        private readonly FileService _fileService;
        private readonly string _webRoot;

        public ImageUploadEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();
            _webRoot = Path.Combine(Path.GetTempPath(), "hrkeshop-upload-tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_webRoot);

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(_webRoot);
            envMock.Setup(e => e.ContentRootPath).Returns(_webRoot);

            _fileService = new FileService(envMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ValidUpload_SavesFileAndReturnsSuccess()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Test image",
                ImageableId = user.ID,
                Imageabletype = "users",
                File = ImageHelper.CreateFakeFormFile()
            };

            var result = await endpoint.HandleAsync(request);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<ImageUploadEndpoint.ImageUploadResponse>(ok.Value);

            Assert.True(payload.ImageId > 0);
            Assert.StartsWith("/images/users/", payload.Url);
            Assert.True(File.Exists(Path.Combine(_webRoot, payload.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))));

            var savedImage = await _db.ImagesAll.FirstOrDefaultAsync(i => i.ID == payload.ImageId);
            Assert.NotNull(savedImage);
            Assert.Equal("users", savedImage.ImageableType);
            Assert.Equal("Test image", savedImage.Name);
        }

        [Fact]
        public async Task HandleAsync_ProductImage_SavesUnderProductsFolder()
        {
            var product = await _db.Products.FirstAsync();
            var endpoint = CreateEndpoint(1, "Admin");

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Product photo",
                ImageableId = product.ID,
                Imageabletype = "products",
                File = ImageHelper.CreateFakeFormFile("shoes.png")
            };

            var result = await endpoint.HandleAsync(request);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var payload = Assert.IsType<ImageUploadEndpoint.ImageUploadResponse>(ok.Value);

            Assert.StartsWith("/images/products/", payload.Url);
            Assert.True(File.Exists(Path.Combine(_webRoot, payload.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar))));
        }

        [Fact]
        public async Task HandleAsync_InvalidFileNoFile_ReturnsNoFileUploaded()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Test no file",
                File = null!,
                ImageableId = user.ID,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => endpoint.HandleAsync(request));
            Assert.Equal("No file uploaded", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_InvalidImageableType_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Test bad type",
                File = ImageHelper.CreateFakeFormFile(),
                ImageableId = user.ID,
                Imageabletype = "udara me Damjan"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => endpoint.HandleAsync(request));
            Assert.Equal("Not valid type!", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_NotExistingIdForGivenType_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Not existing Id for valid type",
                File = ImageHelper.CreateFakeFormFile(),
                ImageableId = 55555,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => endpoint.HandleAsync(request));
            Assert.Equal("Invalid Id for the given ImageableType", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_NotSupportedTypeForImage_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Unsupported extension for file",
                File = ImageHelper.CreateFakeFormFile("test.txt", "text/plain"),
                ImageableId = user.ID,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => endpoint.HandleAsync(request));
            Assert.Equal("Not allowed extension for image!", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_ExceedingSizeForImage_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Exceeding limit test",
                File = ImageHelper.CreateFakeFormFile(byteSize: 6291456),
                ImageableId = user.ID,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => endpoint.HandleAsync(request));
            Assert.Equal("It is exceeding limits!", ex.Message);
        }

        [Fact]
        public async Task HandleAsync_OtherUsersImage_ReturnsForbidden()
        {
            var user = await _db.AppUsersAll.FirstAsync();
            var endpoint = CreateEndpoint(user.ID + 99);

            var request = new ImageUploadEndpoint.ImageUploadRequest
            {
                Name = "Someone else",
                ImageableId = user.ID,
                Imageabletype = "users",
                File = ImageHelper.CreateFakeFormFile()
            };

            var result = await endpoint.HandleAsync(request);
            var forbidden = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, forbidden.StatusCode);
        }

        private ImageUploadEndpoint CreateEndpoint(int appUserId, string role = "Customer")
        {
            var authMock = new Mock<IAuthContext>();
            authMock.Setup(a => a.AppUserId).Returns(appUserId);
            authMock.Setup(a => a.Role).Returns(role);

            var endpoint = new ImageUploadEndpoint(_db, _fileService, authMock.Object);
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(userId: appUserId.ToString(), role: role);
            endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };
            return endpoint;
        }

        public void Dispose()
        {
            if (Directory.Exists(_webRoot))
            {
                Directory.Delete(_webRoot, true);
            }
        }
    }
}
