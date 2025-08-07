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
    public class FavoriteAddEndpointTests : EndpointTestBase
    {
        private readonly FavoriteAddEndpoint _endpoint;
        private readonly int _currentUserId;


        public FavoriteAddEndpointTests() : base("Customer")
        {
            _endpoint = new FavoriteAddEndpoint(_db);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
            _currentUserId = _db.GetUserIdThrow();
        }


        [Fact]

        public async Task HandleAsync_ShouldReturnConflictObjectResult_WhenProductIsAlreadyFavorited()
        {
            //We add new favorite

            //CurentUser

            


            var newFavorite = new Favorite
            {
                ProductId = 1,
                AppUserId = _currentUserId,

            };

            _db.FavoritesAll.Add(newFavorite);

            await _db.SaveChangesAsync();

            //We try to add to favorites same one

            var response = await _endpoint.HandleAsync(1);

            var conflictObjRes = Assert.IsType<ConflictObjectResult>(response.Result);

            Assert.Equal("Product is already favorited", conflictObjRes.Value);

        }
        [Fact]
        public async Task HandleAsync_ShouldReturnOkResponse_WhenValidRequest()
        {




            var response = await _endpoint.HandleAsync(1);

            var OkResponseObj = Assert.IsType<OkObjectResult>(response.Result);

            var responseFavoriteAdd = Assert.IsType<FavoriteAddEndpoint.FavoriteAddResponse>(OkResponseObj.Value);

            Assert.Equal("Product add to favorites", responseFavoriteAdd.Message);
            Assert.Equal(1, responseFavoriteAdd.ID);


        }

    }
}
