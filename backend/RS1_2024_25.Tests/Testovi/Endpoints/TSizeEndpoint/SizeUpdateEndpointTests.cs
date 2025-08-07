using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.SizeEndpoints;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeEndpoint
{
    public class SizeUpdateEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly SizeUpdateEndpoint _endpoint;



        public SizeUpdateEndpointTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(role: "Manager");


            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();


            _endpoint = new SizeUpdateEndpoint(_db);


            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext

            };
        }



        [Fact]

        public async Task HandleAsync_ShouldUpdateSize_When_RequestIsValid()
        {
            //First we will add new size

            var existingSizeType = await _db.SizeTypes.FirstOrDefaultAsync();

            var newSize = new Size
            {
                SizeTypeId = existingSizeType.ID,
                Value = "newValue"
            };

            _db.SizesAll.Add(newSize);
            await _db.SaveChangesAsync();


            //now we try to update

            var updateSize = new SizeUpdateEndpoint.SizeUpdateRequest
            {
                Id = newSize.ID,
                SizeTypeId = newSize.SizeTypeId,
                Value = "newwValue"
            };


            var response = await _endpoint.HandleAsync(updateSize);


            var OkObjectResult = Assert.IsType<OkObjectResult>(response.Result);

            var SizeUpdateResponse = Assert.IsType<SizeUpdateEndpoint.SizeUpdateResponse>(OkObjectResult.Value);


            Assert.NotNull(SizeUpdateResponse);
            Assert.Equal($"Size with the id {newSize.ID} successfully updated", SizeUpdateResponse.Message);
            Assert.Equal(newSize.ID, SizeUpdateResponse.ID);

        }
        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_ForInvalidIdSize()
        {
            var request = new SizeUpdateEndpoint.SizeUpdateRequest
            {
                Id = -2,
                SizeTypeId = 1,
                Value = "hej"

            };

            var response = await _endpoint.HandleAsync(request);


            var notFoundObject = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.NotNull(notFoundObject);
            Assert.Equal("Size not found", notFoundObject.Value);

        }
        [Fact]

        public async Task HandleAsync_ShouldReturnBadRequest_WhenSizeTypeIdIsInvalid()
        {
            var request = new SizeUpdateEndpoint.SizeUpdateRequest
            {
                Id = 1,
                SizeTypeId = -1,
                Value = "test"

            };

            var response = await _endpoint.HandleAsync(request);


            var badRequestObject = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.NotNull(badRequestObject);
            Assert.Equal("Size type does not exist", badRequestObject.Value);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenSizeAlreadyExists()
        {
            var exisitingSizeType = _db.SizeTypes.FirstOrDefaultAsync();

            var newSize = new Size
            {
                SizeTypeId = exisitingSizeType.Id,
                Value = "test"

            };

            _db.SizesAll.Add(newSize);
            await _db.SaveChangesAsync();


            //Test from the same sizeType

            var sizeToUpdate = await _db.Sizes.Where(s => s.SizeTypeId == exisitingSizeType.Id && s.ID != newSize.ID).Select(s => s.ID).FirstAsync();

            var request = new SizeUpdateEndpoint.SizeUpdateRequest
            {
                Id = sizeToUpdate,
                Value = "test",
                SizeTypeId = exisitingSizeType.Id


            };


            var response = await _endpoint.HandleAsync(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.NotNull(badRequest);
            Assert.Equal("Size with value asssociated with this type already exists", badRequest.Value);
            




        }



    }
}
