using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ProductSizeEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductSizeEndpoints
{
    public class ProductSizesGetByProductEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductSizesGetByProductEndpoint _endpoint;



        public ProductSizesGetByProductEndpointTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();

            _endpoint = new ProductSizesGetByProductEndpoint(_db);


            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext

            };
        }


        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_ForInvalidProductSizeId()
        {
            var requestId = 5000;


            var response = await _endpoint.HandleAsync(requestId);


            var notFoundObject = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.NotNull(notFoundObject);
            Assert.Equal($"No sizes found for the product with the ID {requestId}", notFoundObject.Value);






        }
        [Fact]
        public async Task HandleAsync_ShouldReturnProductSizeResponse_ForValidProductSizeId()
        {
            var product = await _db.Products.FirstOrDefaultAsync();


            var requestId = product.ID;


            var response = await _endpoint.HandleAsync(requestId);


            var OkObjectResult = Assert.IsType<OkObjectResult>(response.Result);

            var dtoProducget = Assert.IsType<List<ProductSizesGetByProductEndpoint.ProductSizeResponse>>(OkObjectResult.Value);

            Assert.NotNull(dtoProducget);
            Assert.IsType<List<ProductSizesGetByProductEndpoint.ProductSizeResponse>>(dtoProducget);

            var firstItem = dtoProducget.First();
            Assert.Equal(product.Name, firstItem.ProductName);







        }


    }
}
