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
    public class SizeTypeGetAllEndpointTests : EndpointTestBase
    {
        private readonly SizeTypeGetAllEndpoint _endpoint;

        public SizeTypeGetAllEndpointTests() :base("Manager")
        {
            _endpoint = new SizeTypeGetAllEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };

        }



        [Fact]
        public async Task HandleAsync_ShouldReturnValidList()
        {
            //we use already seeded base


            var response = await _endpoint.HandleAsync();


            var OkObjectResult = Assert.IsType<OkObjectResult>(response.Result);

            var listOfGetAllSizeType = Assert.IsType<List<SizeTypeGetAllEndpoint.SizeTypeGetAllResponse>>(OkObjectResult.Value);

            Assert.NotEmpty(listOfGetAllSizeType);
        }
        

        


    }
}
