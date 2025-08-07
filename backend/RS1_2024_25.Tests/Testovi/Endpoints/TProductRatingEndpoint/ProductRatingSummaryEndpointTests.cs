using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.ProductRatingEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductRatingEndpoint
{
    public class ProductRatingSummaryEndpointTests : EndpointTestBase
    {
        private readonly ProductRatingSummaryEndpoint _endpoint;

        public ProductRatingSummaryEndpointTests()
        {
            _endpoint = new ProductRatingSummaryEndpoint(_db);
            
        }





        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_WhenProductIdIsInvalid()
        {
            var requestId = -1;



            var response = await _endpoint.HandleAsync(requestId);


            Assert.NotNull(response);
            var notFoundObj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("Product not found.", notFoundObj.Value);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnValidResponse_WhenValidRequest()
        {
            //first we will add two ratings for one product from different users

            var productId = await _db.Products.Select(p => p.ID).FirstOrDefaultAsync();
            var appuserId = await _db.AppUsers.Select(au => au.ID).FirstOrDefaultAsync();

            var productRating1 = new ProductRating
            {
                AppUserId = _db.GetUserIdThrow(),
                ProductId = productId,
                Rating = 5
            };
            var productRating2 = new ProductRating
            {
                AppUserId = appuserId,
                ProductId = productId,
                Rating = 1

            };
            await _db.ProductRatingsAll.AddRangeAsync(productRating1, productRating2);
            await _db.SaveChangesAsync();

            var request = productId;

            var response = await _endpoint.HandleAsync(request);


            var okObjResu = Assert.IsType<OkObjectResult>(response.Result);

            var PrRatingSummaryResponse = Assert.IsType<ProductRatingSummaryEndpoint.ProductRatingSummaryEndpointResponse>(okObjResu.Value);


            Assert.Equal(3.0, PrRatingSummaryResponse.AverageRating);
            Assert.Equal(2, PrRatingSummaryResponse.RatingsCount);


        }



    }
}
