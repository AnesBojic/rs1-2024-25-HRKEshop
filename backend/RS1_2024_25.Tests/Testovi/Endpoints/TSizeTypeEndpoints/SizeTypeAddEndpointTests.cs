using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Endpoints.SizeEndpoints;
using RS1_2024_25.API.Endpoints.SizeTypeEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeTypeEndpoints
{
    public class SizeTypeAddEndpointTests : EndpointTestBase
    {
        private readonly SizeTypeAddEndpoint _endpoint;


        public SizeTypeAddEndpointTests() : base("Manager")
        {
            _endpoint = new SizeTypeAddEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
        }

        [Fact]

        public async Task HandleAsync_ShouldAddSizeType_WhenRequestIsValid()
        {
            var request = new SizeTypeAddEndpoint.SizeTypeAddRequest
            {
                Name = "New one"

            };

            var response = await _endpoint.HandleAsync(request);

            var OkObjectRes = Assert.IsType<OkObjectResult>(response.Result);

            var sizeTypeAddResponse = Assert.IsType<SizeTypeAddEndpoint.SizeTypeAddResponse>(OkObjectRes.Value);

            var lastSizeTypeAddedId = await _db.SizeTypes.OrderByDescending(st => st.CreatedAt).Select(st => st.ID).FirstAsync();

            Assert.Equal(lastSizeTypeAddedId, sizeTypeAddResponse.ID);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            var reqeust = new SizeTypeAddEndpoint.SizeTypeAddRequest
            { Name = "" };

            var response = await _endpoint.HandleAsync(reqeust);


            var badReqObj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Not valid name for the sizeType", badReqObj.Value);

           
        }
        [Fact]

        public async Task HandleAsync_ShouldReturnBadRequest_WhenNameAlreadyExists()
        {
            var nameForTheSizeType = await _db.SizeTypes.Select(st => st.Name).FirstOrDefaultAsync();


            var request = new SizeTypeAddEndpoint.SizeTypeAddRequest
            { Name = nameForTheSizeType };

            var response = await _endpoint.HandleAsync(request);

            var badreqObj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Size type with this name already exists", badreqObj.Value);



        }


    }
}
