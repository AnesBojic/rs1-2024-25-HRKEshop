using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ImageEndpoints;
using RS1_2024_25.API.Services;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    public class ImageDeleteEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ImageDeleteEndpoint _endpoint;
        private readonly FileService _fileService;

        public ImageDeleteEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _fileService = new FileService(envMock.Object);

            _endpoint = new ImageDeleteEndpoint(_db, _fileService);



            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };


        }


        [Fact]

        public async Task HandleAsync_DeleteExistingImageSuccessfully()
        {
            var testImagePath = Path.Combine(Path.GetTempPath(), "images", "users");
            Directory.CreateDirectory(testImagePath);

            var fullPath = Path.Combine(testImagePath, "testfile.jpg");
            await File.WriteAllTextAsync(fullPath, "delete test");

            var image = new Image
            {
                Name = "Test image",
                FilePath = fullPath,
                Url = "/images/users/testfile.jpg",
                ImageableId = 1,
                ImageableType = "users"
            };

            _db.ImagesAll.Add(image);
            await _db.SaveChangesAsync();

            var imageId = image.ID;

            await _endpoint.HandleAsync(imageId);

            await _db.SaveChangesAsync();

            var deletedImage = await _db.ImagesAll.FindAsync(imageId);

            Assert.Null(deletedImage);

            Assert.False(File.Exists(fullPath));
        }

        [Fact]
        public async Task HandleAsync_ThrowKeyNotFoundExpection()
        {

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _endpoint.HandleAsync(-555));

            Assert.Equal("Image not found", ex.Message);




        }



    }
}
