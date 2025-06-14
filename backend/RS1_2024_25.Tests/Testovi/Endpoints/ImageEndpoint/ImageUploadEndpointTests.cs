using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.ImageEndpoints;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    public class ImageUploadEndpointTests
    {

        private readonly ApplicationDbContext _db;
        private readonly ImageUploadEndpoint _endpoint;
        private readonly FileService _fileService;
        

        public ImageUploadEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            
           

            _fileService = new FileService(envMock.Object);


            _endpoint = new ImageUploadEndpoint(_db, _fileService);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };

        }

        [Fact]
        public async Task HandleAsync_ValidUpload_SavesFileAndReturnsSuccess()
        {

            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Test image",
                ImageableId = user.ID,
                Imageabletype = "users",
                File = ImageHelper.CreateFakeFormFile()


            };


            var result = await _endpoint.HandleAsync(request);

            Assert.NotNull(result);
            Assert.Contains("Image uploaded successfully", result);

            var savedImage = await _db.ImagesAll.FirstOrDefaultAsync(i => i.ImageableId == user.ID && i.Name =="Test image");

            Assert.NotNull(savedImage);
            Assert.Equal("Test image", savedImage.Name);


            Assert.False(string.IsNullOrWhiteSpace(savedImage.FilePath));
            Assert.EndsWith(".jpg", savedImage.FilePath);


            Assert.StartsWith("/images", savedImage.Url);

        }

        [Fact]
        public async Task HandleAsync_InvalidFileNoFile_ReturnsNoFileUploaded()
        {
            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Test no file",
                File = null,
                ImageableId = user.ID,
                Imageabletype = "users",



            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));
            Assert.Equal("No file uploaded", ex.Message);




        }

        [Fact]
        public async Task HandleAsync_InvalidImageableType_ReturnsArgumentExpection()
        {
            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Test bad type",
                File = ImageHelper.CreateFakeFormFile(),
                ImageableId = user.ID,
                Imageabletype = "udara me Damjan"


            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Not valid type!", ex.Message);


        }


        [Fact]
        public async Task HandleAsync_NotExistingIdForGivenType_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Not existing Id for valid type",
                File = ImageHelper.CreateFakeFormFile(),
                ImageableId = 555,
                Imageabletype = "users",



            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Invalid Id for the given ImageableType", ex.Message);




        }

        [Fact]

        public async Task HandleAsync_NotSupportedTypeForImage_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Unsupported extension for file",
                File = ImageHelper.CreateFakeFormFile("test.txt", "text/plain"),
                ImageableId = user.ID,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Not allowed extension for image!", ex.Message);









        }
        [Fact]

        public async Task HandleAsync_ExceedingSizeForImage_ReturnsArgumentException()
        {
            var user = await _db.AppUsersAll.FirstOrDefaultAsync();

            var request = new ImageUploadRequest
            {
                Name = "Exceeding limit test",
                File = ImageHelper.CreateFakeFormFile(byteSize: 6291456),
                ImageableId = user.ID,
                Imageabletype = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("It is exceeding limits!", ex.Message);



        }






    }
}
