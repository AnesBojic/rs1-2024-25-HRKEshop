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
    public class SizeTypeUpdateEndpointTests : EndpointTestBase
    {
        private readonly SizeTypeUpdateEndpoint _endpoint;

        public SizeTypeUpdateEndpointTests() : base("Manager")
        {
            _endpoint = new SizeTypeUpdateEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
        }

        [Fact]

        public async Task HandleAsnyc_ShouldReturnNotfound_WhenSizeTypeDoesNotExist()
        {
            var request = new SizeTypeUpdateEndpoint.SizeTypeUpdateRequest
            {
                Id = -1,
                UpdatedName = "invalid"

            };

            var response = await _endpoint.HandleAsync(request);


            var noftounfobj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("No sizetype with specified Id", noftounfobj.Value);

        }
        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            var request = new SizeTypeUpdateEndpoint.SizeTypeUpdateRequest
            {
                Id = 1,
                UpdatedName = ""
            };

            var response = await _endpoint.HandleAsync(request);

            var badreqobj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Not valid name for sizeType", badreqobj.Value);

        }
        [Fact]

        public async Task HandleAsync_ShouldReturnBadRequst_WhenNameAlreadyExists()
        {
            //We will use seeded base for tests

            var existingName = await _db.SizeTypes.Select(st => st.Name).FirstAsync();



            var request = new SizeTypeUpdateEndpoint.SizeTypeUpdateRequest
            {
                Id = 2,
                UpdatedName = existingName
            };


            var response = await _endpoint.HandleAsync(request);

            var badreqobj = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.Equal("Size type with this name already exists.", badreqobj.Value);

        }
        [Fact]

        public async Task HandleAsync_ShouldRetunOkOobject_WhenValidRequest()
        {
            //we use already made db

            var sizetypeId = await _db.SizeTypes.Select(st => st.ID).FirstOrDefaultAsync();

            var request = new SizeTypeUpdateEndpoint.SizeTypeUpdateRequest
            {
                Id = sizetypeId,
                UpdatedName = "New name wohohoh"

            };

            var response = await _endpoint.HandleAsync(request);

            var OkObjectres = Assert.IsType<OkObjectResult>(response.Result);

            var responseSizeTypeUpdate = Assert.IsType<SizeTypeUpdateEndpoint.SizeTypeUpdateResponse>(OkObjectres.Value);

            Assert.Equal(sizetypeId, responseSizeTypeUpdate.Id);
            Assert.Contains($"Size type with id {sizetypeId} successfuly updated to :",responseSizeTypeUpdate.Message);


        }


    }
}
