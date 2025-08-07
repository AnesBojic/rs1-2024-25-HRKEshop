using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ProductEndpoints;
using RS1_2024_25.API.Endpoints.ProductRatingEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductRatingEndpoint
{
    public class ProductRatingDeleteTests : EndpointTestBase
    {
        private readonly ProductRatingDeleteEndpoint _endpoint;
        

        public ProductRatingDeleteTests()
        {
            _endpoint = new ProductRatingDeleteEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };


        }

        [Fact]

        public async Task HandleAsync_ShouldReturnNotFound_OnNonExistingRating()
        {
            var productId = await _db.Products.Select(p=> p.ID).FirstOrDefaultAsync();


            var response = await _endpoint.HandleAsync(productId);


            var notFoundObjRes = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("Product rating not found.", notFoundObjRes.Value);
        }

        [Fact]

        public async Task HandleAsync_ShouldReturnOkResponse_WhenValidRequest()
        {
            //first we add productRating 
            var productId = await _db.Products.Select(p => p.ID).FirstOrDefaultAsync();

            var newProductRating = new ProductRating
            {
                AppUserId = _db.GetUserIdThrow(),
                ProductId = productId,
                Rating = 2,
                Comment = "fordeletion"
            };

            _db.ProductRatingsAll.Add(newProductRating);
            await _db.SaveChangesAsync();


            var response = await _endpoint.HandleAsync(productId);

            var OkObjRes = Assert.IsType<OkObjectResult>(response.Result);

            var GoodResponse = Assert.IsType<ProductRatingDeleteEndpoint.ProductRatingDeleteResponse>(OkObjRes.Value);

            Assert.Equal("Product rating removed succesfully.",GoodResponse.Message);



        }


    }
}
