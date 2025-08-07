using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.SizeTypeEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeTypeEndpoints
{
    public class SizeTypeDeleteEndpointTests : EndpointTestBase
    {
        private readonly SizeTypeDeleteEndpoint _endpoint;

        public SizeTypeDeleteEndpointTests() : base("Manager")
        {
            _endpoint = new SizeTypeDeleteEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
        }


        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_WhenRequestInvalid()
        {
            var sizetypeId = -1;


            var response = await _endpoint.HandleAsync(sizetypeId);


            var notFoundObj = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal("No size type found", notFoundObj.Value);

        }
        [Fact]

        public async Task HandleAsync_ShouldReturnBadRequest_WhenFKConstraint()
        {
            //We will take one siztype from the seeded base

            var existingSizeType = await _db.SizeTypes.Select(st => st.ID).FirstAsync();


            var response = await _endpoint.HandleAsync(existingSizeType);

            var BadRequestObj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Cannot delete sizeType because it is used by one or more sizes", BadRequestObj.Value);

        }

        [Fact]

        public async Task HandleAsync_ShouldReturnOkResponse_WhenValidRequest()
        {
            //We will add new sizeType

            var newSizeType = new SizeType
            {
                Name = "NewOne"

            };

            _db.SizeTypesAll.Add(newSizeType);
            await _db.SaveChangesAsync();

            var response = await _endpoint.HandleAsync(newSizeType.ID);

            var OkResult = Assert.IsType<OkObjectResult>(response.Result);

            Assert.Equal("SizeType deleted succesfully", OkResult.Value);


        }





    }
}
