using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Endpoints.ProductRatingEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductRatingEndpoint
{
    public class ProductRatingGetAllForProductEndpointTests : EndpointTestBase
    {
        private readonly ProductRatingGetAllForProduct _endpoint;

        public ProductRatingGetAllForProductEndpointTests()
        {
            _endpoint = new ProductRatingGetAllForProduct(_db);

        }


        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_WhenProductIdIsInvalid()
        {
            var request = -1;


            var response = await _endpoint.HandleAsync(request);

            var notfoundobj = Assert.IsType<NotFoundObjectResult>(response.Result);


            Assert.Equal("Product does not exist.", notfoundobj.Value);

        }
        [Fact]
        public async Task HandleAsync_ShouldReturnEmptyList_ValidRequest()
        {
            //we use seeded db

            var productId = await _db.Products.Select(p => p.ID).FirstAsync();


            var response = await _endpoint.HandleAsync(productId);


            var okObjRes = Assert.IsType<OkObjectResult>(response.Result);

            var ListResponse = Assert.IsType<List<ProductRatingGetAllForProduct.ProductRatingGetAllForProductResponse>>(okObjRes.Value);

            Assert.Equal(0, ListResponse.Count());






        }



    }
}
