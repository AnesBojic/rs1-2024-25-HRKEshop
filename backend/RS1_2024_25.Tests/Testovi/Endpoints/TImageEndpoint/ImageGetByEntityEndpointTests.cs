using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
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

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{


    public class ImageGetByEntityEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ImageGetByEntityEndpoint _endpoint;
        private readonly FileService _fileService;


        public ImageGetByEntityEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new ImageGetByEntityEndpoint(_db);

            var MockWebEnv = new Mock<IWebHostEnvironment>();

            MockWebEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _fileService = new FileService(MockWebEnv.Object);


            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();


            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };

        }


        [Fact]

        public async Task HandleAsync_ReturnsListImagesByTypeAndIIdConnectionSuccess()
        {
            var listaSlikaRoles = new List<Image>();
            var listaSlikaUsers = new List<Image>();

            var user = await _db.AppUsersAll.FirstOrDefaultAsync();
            var role = await _db.Roles.FirstOrDefaultAsync();

            for (int i = 0; i < 2; i++)
            {
                var slikaUser = new Image
                {
                    Name = $"SlikaUserEntity{i}",
                    FilePath = "userPath",
                    Url = "userUrl",
                    ImageableId = user.ID,
                    ImageableType = "users",
                };
                var slikaRole = new Image
                {
                    Name = $"SlikaRoleEntity{i}",
                    FilePath = "rolePath",
                    Url = "roleUrl",
                    ImageableId = role.ID,
                    ImageableType = "roles",
                };

                listaSlikaUsers.Add(slikaUser);
                listaSlikaRoles.Add(slikaRole);
            }

            await _db.ImagesAll.AddRangeAsync(listaSlikaUsers);
            await _db.ImagesAll.AddRangeAsync(listaSlikaRoles);

            await _db.SaveChangesAsync();


            var requestUser = new ImageGetByEntityRequest
            {
                ImageableId = user.ID,
                ImageableType = "users"

            };
            var requestRoles = new ImageGetByEntityRequest
            {
                ImageableId = role.ID,
                ImageableType = "roles"
            };

            var resultUser = await _endpoint.HandleAsync(requestUser);


            Assert.NotNull(resultUser);
            //Also the baseSeed with the first image given 
            Assert.Equal(3, resultUser.Count);
            Assert.Contains(resultUser,img=>img.Name =="SlikaUserEntity1");

            var resultRole = await _endpoint.HandleAsync(requestRoles);

            Assert.NotNull(resultRole);
            Assert.Equal(2,resultRole.Count);
            Assert.Contains(resultRole,img=> img.Name == "SlikaRoleEntity0");
        }
        [Fact]
        public async Task HandleAsync_ThrowArgumentException_NonExistentType()
        {
            var appUser = await _db.AppUsersAll.FirstOrDefaultAsync();


            var request = new ImageGetByEntityRequest
            {
                ImageableId = appUser.ID,
                ImageableType = "NonExistent"


            };


            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Invalid imageable type", ex.Message);
        }
        [Fact]
        public async Task HandleAsync_ThrowArgumentException_NoEntityAssociatedWithTheType()
        {
            var request = new ImageGetByEntityRequest
            {
                ImageableId = -555,
                ImageableType = "users"
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _endpoint.HandleAsync(request));

            Assert.Equal("Entity with given Id does not exist", ex.Message);






        }
        [Fact]

        public async Task HandleAsync_ReturnsEmptyList_WhenNoImagesExistForEntity()
        {
            //user 11 na baseSeed nema slika validan je
            //Ili bilo koji role

            var appUser = await _db.AppUsersAll.FindAsync(11);

            var request = new ImageGetByEntityRequest
            {
                ImageableId = 11,
                ImageableType = "users"

            };

            var result = await _endpoint.HandleAsync(request);

            Assert.NotNull(result);
            Assert.Empty(result);




        }

    }
}
