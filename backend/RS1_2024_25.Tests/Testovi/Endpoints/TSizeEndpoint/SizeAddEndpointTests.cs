using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.SizeEndpoints;
using RS1_2024_25.API.Endpoints.SizeTypeEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeEndpoint
{
    public class SizeAddEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly SizeAddEndpoint _endpoint;


        public SizeAddEndpointTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(role: "Manager");

            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();


            _endpoint = new SizeAddEndpoint(_db);

            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };
        }


        [Fact]

        public async Task HandleAsync_ShouldReturnOkResponse_ValidRequest()
        {
            //first we add sizetype

            var sizeType = new SizeType
            {
                Name = "testSize"

            };

            _db.SizeTypesAll.Add(sizeType);
            await _db.SaveChangesAsync();

            //creating request based on this sizetype

            var sizeRequest = new SizeAddEndpoint.SizeAddRequest
            {
                SizeTypeId = sizeType.ID,
                Value = "testingvalue"

            };


            var response = await _endpoint.HandleAsync(sizeRequest);


            var okObject = Assert.IsType<OkObjectResult>(response.Result);

            var sizeResponse = Assert.IsType<SizeAddEndpoint.SizeAddResponse>(okObject.Value);

            var lastSizeAddedId = await _db.Sizes.OrderByDescending(s => s.CreatedAt).Select(s => s.ID).FirstAsync();

            Assert.NotNull(sizeResponse);
            Assert.Equal(lastSizeAddedId, sizeResponse.ID);
            Assert.Equal($"Size {sizeRequest.Value} successfully created.", sizeResponse.Message);

        }
        [Fact]

        public async Task HandleAsync_ShouldReturnBadRequest_SizeTypeNotFound()
        {

            var request = new SizeAddEndpoint.SizeAddRequest
            {
                SizeTypeId = -1,
                Value = "testingValue"

            };




            var response = await _endpoint.HandleAsync(request);

            var badObject = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.NotNull(badObject);
            Assert.Equal("SizeType not found", badObject.Value);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_SizeAlreadyExists()
        {
            var sizetypeAlreadyExisting = await _db.SizeTypes.FirstOrDefaultAsync();

            var newSize = new Size
            {
                SizeTypeId = sizetypeAlreadyExisting.ID,
                Value = "testingSize"
            };

            _db.SizesAll.Add(newSize);

            await _db.SaveChangesAsync();

            var request = new SizeAddEndpoint.SizeAddRequest
            {
                SizeTypeId = sizetypeAlreadyExisting.ID,
                Value = "testingSize"
            };


            var response = await _endpoint.HandleAsync(request);

            var BadRequestObject = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.NotNull(BadRequestObject);
            Assert.Equal("Size like this already exists.", BadRequestObject.Value);









        }

    }
}
