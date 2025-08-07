using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.SizeEndpoints;
using RS1_2024_25.Tests.Services;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RS1_2024_25.API.Endpoints.SizeEndpoints.SizeDeleteEndpoint;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeEndpoint
{
    public class SizeDeleteEndpointTests : EndpointTestBase
    {
        private readonly SizeDeleteEndpoint _endpoint;

        public SizeDeleteEndpointTests() : base("Manager")
        {
            _endpoint = new SizeDeleteEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
            
        }

        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var requestId = -2;


            var response = await _endpoint.HandleAsync(requestId);


            var Notfoundobj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("No size with that id", Notfoundobj.Value);
        }

        [Fact]

        public async Task HandleAsync_ShouldReturnBadrequeqst_ForeignKeyConstraint()
        {
            //for this we can use already seeded base

            var sizeId = await _db.Sizes.Select(s => s.ID).FirstOrDefaultAsync();



            var response = await _endpoint.HandleAsync(sizeId);

            var badReqObj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Size is foreign key in one or more objects", badReqObj.Value);
        }

        [Fact]

        public async Task HandleAsync_ShouldReturnOkResponse_WhenRequestIsValid()
        {
            //Okay we will need to create new size to avoid foreign key constraint

            var sizeTypeid = await _db.SizeTypes.Select(st => st.ID).FirstOrDefaultAsync();

            var newSize = new Size
            {
                SizeTypeId = sizeTypeid,
                Value = "2"
            };

            _db.SizesAll.Add(newSize);
            await _db.SaveChangesAsync();

            var response = await _endpoint.HandleAsync(newSize.ID);
            var okResult = Assert.IsType<OkObjectResult>(response.Result);
            var deleteResponse = Assert.IsType<SizeDeleteEndpointResponse>(okResult.Value);
            Assert.Equal("Size successfully deleted.", deleteResponse.Message);






        }





    }
}
