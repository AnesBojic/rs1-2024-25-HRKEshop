using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;
using System.Runtime.CompilerServices;

namespace RS1_2024_25.API.Endpoints.FavoriteEndpoints
{

    [Authorize(Roles =("Manager,Customer,Admin"))]
    [Route("favorites/add/{productId}")]
    public class FavoriteAddEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<FavoriteAddEndpoint.FavoriteAddResponse>

    {
        [HttpPost]
        public override async Task<ActionResult<FavoriteAddResponse>> HandleAsync([FromRoute] int productId, CancellationToken cancellationToken = default)
        {
            var currentUser = db.GetUserIdThrow();

            var alreadyFavorited = await db.Favorites.AnyAsync(f => f.ProductId == productId && f.AppUserId == currentUser, cancellationToken);
            if (alreadyFavorited)
            {
                return Conflict("Product is already favorited");
            }

            var favorite = new Favorite
            {
                AppUserId = currentUser,
                ProductId = productId,

            };


            db.FavoritesAll.Add(favorite);
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new FavoriteAddResponse
            {
                ID = productId,
                Message = "Product add to favorites"
            });




        }







       

        public class FavoriteAddResponse : BaseResponse
        {
            

        }
    }
}
