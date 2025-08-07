using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.SizeEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TSizeEndpoint
{
    public class SizeGetByIdEndpointTests  : EndpointTestBase
    {
        private readonly SizeGetByIdEndpoint _endpoint;


        public SizeGetByIdEndpointTests() : base("Manager")
        {

            _endpoint = new SizeGetByIdEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };

        }

        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            var requestId = -2;

            var response = await _endpoint.HandleAsync(requestId);


            var NotFoundObj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.NotNull(NotFoundObj);
            Assert.Equal("Size does not exist.", NotFoundObj.Value);
        }

        [Fact]

        public async Task HandleAsync_ShouldReturnSizeGetByIdRESPONSE_WhenRequestIsValid()
        {
            //we will add new size on exisiting sizetype

            var sizetype = await _db.SizeTypes.FirstOrDefaultAsync();

            var newSize = new Size
            {
                SizeTypeId = sizetype.ID,
                Value = "500"

            };

            _db.SizesAll.Add(newSize);
            await _db.SaveChangesAsync();

            var response = await _endpoint.HandleAsync(newSize.ID);

            var OkObject = Assert.IsType<OkObjectResult>(response.Result);

            var resDto = Assert.IsType<SizeGetByIdEndpoint.SizeGetByIdResponse>(OkObject.Value);

            Assert.NotNull(resDto);
            Assert.Equal(sizetype.Name, resDto.SizeType);
            Assert.Equal(newSize.ID, resDto.Id);
            Assert.Equal(newSize.Value, resDto.Value);








        }

            

    }
}
