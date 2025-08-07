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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    public class ImageUpdateEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ImageUpdateEndpoint _endpoint;
        private readonly FileService _fileService;

        public ImageUpdateEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _fileService = new FileService(envMock.Object);
            

            _endpoint = new ImageUpdateEndpoint(_db,_fileService);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };

        }


        [Fact]
        public async Task HandleAsync_ValidUpdate_SavesChangesAndReturnsSuccess()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();

            Assert.NotNull(image);

            var formFile = ImageHelper.CreateFakeFormFile();

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "Updated image",
                ImageableId = image.ImageableId,
                ImageableType = image.ImageableType,
                File = formFile


            };

            var result = await _endpoint.HandleAsync(request);

            var updatedImage = await _db.ImagesAll.FindAsync(image.ID);


            Assert.Equal("Updated image", updatedImage.Name);
            Assert.False(string.IsNullOrEmpty(updatedImage.FilePath));
            Assert.Contains(".jpg", updatedImage.FilePath);
            Assert.Equal(image.ID, result.Id);
            Assert.Contains($"Successfully updated image {updatedImage.ID}", result.Message);
        }

        [Fact]
        public async Task HandleAsync_TestNotExsitingImage_ThrowsKeyNotFoundException()
        {
            var file = ImageHelper.CreateFakeFormFile();

            var request = new ImageUpdateRequest
            {
                Id = -55,
                Name = "No Id",
                ImageableId = 3,
                ImageableType = "users",
                File = file
            };



            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _endpoint.HandleAsync(request));
            Assert.Equal("Image with this ID not found!", ex.Message);

        }

        [Fact]
        public async Task HandleAsync_TestWrongImageabletype_ThrowsArgumentException()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();


            var file = ImageHelper.CreateFakeFormFile();

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "Testing wrong type",
                ImageableId = image.ID,
                ImageableType = "macka mjauce",
                File = file

            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Invalid imageable type", ex.Message);

        }

        [Fact]

        public async Task HandleAsync_TestNotExistingIdForType_ThrowsArgumentException()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();

            var file = ImageHelper.CreateFakeFormFile();

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "Testing not existing Id",
                ImageableId = -333,
                ImageableType = "users",
                File = file


            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));


            Assert.Equal("Invalid Id for the given ImageableType", ex.Message);




        }

        [Fact]
        public async Task HandleAsync_TestNotAllowedType_ThrowsArgumentException()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();
            var file = ImageHelper.CreateFakeFormFile(fileName: "test.txt");

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "Testing not allowed type for image",
                ImageableId = image.ImageableId,
                ImageableType = image.ImageableType,
                File = file


            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Not allowed extension for image!", ex.Message);



        }

        [Fact]
        public async Task HandleAsync_TestExceedLimit_ThrowsArgumentException()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();
            var file = ImageHelper.CreateFakeFormFile(byteSize: 6300000);

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "Testing for exceeding limit of the image",
                ImageableId = image.ImageableId,
                ImageableType = image.ImageableType,
                File = file


            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("It is exceeding limits!", ex.Message);

        }
        [Fact]
        public async Task HandleAsync_NoFileProvided_KeepexistingFile()
        {
            var image = await _db.ImagesAll.FirstOrDefaultAsync();
            Assert.NotNull(image);

            var oldPath = image.FilePath;
            var oldUrl = image.Url;

            var request = new ImageUpdateRequest
            {
                Id = image.ID,
                Name = "No file uploaded",
                ImageableId = image.ImageableId,
                ImageableType = image.ImageableType,
                File = null
            };

            var result = await _endpoint.HandleAsync(request);

            var updated = await _db.ImagesAll.FindAsync(image.ID);


            Assert.Equal(oldPath, updated.FilePath);
            Assert.Equal(oldUrl, updated.Url);





        }


    }
}
