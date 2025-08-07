using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Endpoints.SizeTypeEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeTypeEndpoints
{
    public class SizeTypeGetByIdTests : EndpointTestBase
    {
        private readonly SizeTypeGetByIdEndpoint _endpoint;

        public SizeTypeGetByIdTests() : base("Manager")
        {
            _endpoint = new SizeTypeGetByIdEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };


        }


        [Fact]

        public async Task HandleAsync_ShouldReturnOkResponse_WhenValidRequest()
        {
            //we will use already existing sizetype

            var existingSizeType = await _db.SizeTypes.FirstOrDefaultAsync();

            var response = await _endpoint.HandleAsync(existingSizeType.ID);


            var OkObject = Assert.IsType<OkObjectResult>(response.Result);

            var SizebyIdResponse = Assert.IsType<SizeTypeGetByIdEndpoint.SizeTypeGetByIdEndpointResponse>(OkObject.Value);

            Assert.Equal(existingSizeType.ID, SizebyIdResponse.ID);
            Assert.Equal(existingSizeType.Name, SizebyIdResponse.Name);

        }

        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_WhenInvalidID()
        {
            var invalidId = -1;

            var response = await _endpoint.HandleAsync(invalidId);

            var notfoundobj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("Size type not found", notfoundobj.Value);





        }

    }
}
