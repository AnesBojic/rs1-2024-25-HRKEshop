using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ImageEndpoints;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Services;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    

    public class ImageGetByIdEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ImageGetByIdEndpoint _endpoint;
        private readonly FileService _fileService;

        public ImageGetByIdEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new ImageGetByIdEndpoint(_db);

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _fileService = new FileService(env.Object);




            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext


            };
        }

        [Fact]
        public async Task HandleAsync_ReturnsSuccessfully_WhenImageExists()
        {
            var file = ImageHelper.CreateFakeFormFile();
            var fullPath = await _fileService.SaveFileAsync(file, "users");
            var url = "Evo me opet dolazimmm";

            var image = new Image
            {
                Name = "Test for byId",
                ImageableId = 1,
                ImageableType = "users",
                FilePath = fullPath,
                Url = url
            };

            await _db.ImagesAll.AddAsync(image);

            await _db.SaveChangesAsync();



            var requestId = image.ID;


            var result = await _endpoint.HandleAsync(requestId);


            Assert.NotNull(result);
            Assert.Equal("Test for byId",result.Name);
            Assert.Equal("Evo me opet dolazimmm", result.Url);

        }

        [Fact]
        public async Task HandleAsync_ReturnsKeyNotFound_WhenImageDoesNotExist()
        {

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _endpoint.HandleAsync(-555));

            Assert.Equal("Image not found", ex.Message);




        }



    }
}
