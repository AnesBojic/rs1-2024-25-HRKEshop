using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ImageEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.ImageEndpoint
{
    public class ImageGetAllEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ImageGetAllEndpoint _endpoint;

        public ImageGetAllEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new ImageGetAllEndpoint(_db);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext

            };

        }


        [Fact]

        public async Task HandleAsync_ShouldReturnAllUsersWithTenantFilerAsWell()
        {
            //JWT is defaultly admin , so it should work if needed for the test change JWT tenant or role to somethin else..

            var result =  await _endpoint.HandleAsync();


            Assert.NotNull(result);
            Assert.Equal(10, result.Count);


            List <Image> slike = new List<Image>();

            for (int i = 0; i < 5; i++)
            {

                var image = new Image
                {
                    Name = "test2Usput",
                    FilePath = $"beze\\{Guid.NewGuid()}",
                    ImageableId = 1,
                    ImageableType = "users",


                };
                slike.Add(image);
            }
            await _db.ImagesAll.AddRangeAsync(slike);

            await _db.SaveChangesAsync();

            var result2 = await _endpoint.HandleAsync();

            Assert.NotNull(result2);
            Assert.Equal(15,result2.Count);
            Assert.Equal("test2Usput", result2[11].Name);


        }
        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyList()
        {
            _db.ImagesAll.RemoveRange(_db.ImagesAll);

            await _db.SaveChangesAsync();

            var result = await _endpoint.HandleAsync();


            Assert.NotNull(result);
            Assert.Empty(result);





        }

    }
}
