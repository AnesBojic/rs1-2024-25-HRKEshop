using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.FavoriteEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TFavoriteEndpoints
{
    public class FavoriteDeleteEndpointTests :EndpointTestBase
    {
        private readonly FavoriteRemoveEndpoint _endpoint;


        public FavoriteDeleteEndpointTests()
        {
            _endpoint = new FavoriteRemoveEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };


        }


        [Fact]

        public async Task ShouldReturnNotFound_WhenProductIdIsInvalid()
        {

            //We can alos send valid productId , cause there is no favorite currently
            var response = await _endpoint.HandleAsync(-1);


            var NotFoundObj = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("Favorite not found", NotFoundObj.Value);
        }

        [Fact]

        public async Task ShouldRemoveFavorite_ValidRequest()
        {
            //we add favorite first

            var currentUserId = _db.GetUserIdThrow();


            var newFavorite = new Favorite
            {
                AppUserId = currentUserId,
                ProductId = 1
            };

            _db.FavoritesAll.Add(newFavorite);
            await _db.SaveChangesAsync();

            var response = await _endpoint.HandleAsync(1);

            var OkObjectRes = Assert.IsType<OkObjectResult>(response.Result);


            var responseRemoveFavorite = Assert.IsType<FavoriteRemoveEndpoint.FavoriteRemoveResponse>(OkObjectRes.Value);

            Assert.Equal("Favorite removed successfully", responseRemoveFavorite.Message);
            Assert.Equal(1, responseRemoveFavorite.ID);




        }




    }
}
