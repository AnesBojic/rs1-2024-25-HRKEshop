using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Endpoints.FavoriteEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TFavoriteEndpoints
{
    public class FavoriteToggleEndpointTests :EndpointTestBase
    {
        private readonly FavoriteToggleEndpoint _endpoint;

        public FavoriteToggleEndpointTests()
        {

            _endpoint = new FavoriteToggleEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };


        }



        //We will write one test for this all in one

        [Fact]
        public async Task HandleAsync_ShouldAddFavorites_ThenRemoveIt()
        {

            //we use productId 1 already seeded
            var response = await _endpoint.HandleAsync(1);

            var OkObjectRes = Assert.IsType<OkObjectResult>(response.Result);

            var ToggleRes = Assert.IsType<FavoriteToggleEndpoint.FavoriteToggleResponse>(OkObjectRes.Value);


            Assert.Equal(1, ToggleRes.ProductId);
            Assert.True(ToggleRes.IsFavorited);
            Assert.Equal("Product added to favorites", ToggleRes.Message);

            //now second call

            response = await _endpoint.HandleAsync(1);

            OkObjectRes = Assert.IsType<OkObjectResult>(response.Result);

            ToggleRes = Assert.IsType<FavoriteToggleEndpoint.FavoriteToggleResponse>(OkObjectRes.Value);

            Assert.Equal(1, ToggleRes.ProductId);
            Assert.False(ToggleRes.IsFavorited);
            Assert.Equal("Product removed from favorites", ToggleRes.Message);





        }




    }
}
