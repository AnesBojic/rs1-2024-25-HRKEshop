using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.FavoriteEndpoints
{

    [Authorize(Roles =("Manager,Customer,Admin"))]
    [Route("favorites/remove/{productId}")]
    public class FavoriteRemoveEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<FavoriteRemoveEndpoint.FavoriteRemoveResponse>
    {
        [HttpDelete]
        public override async Task<ActionResult<FavoriteRemoveResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {

            var currentUserId = db.GetUserIdThrow();

            var favorite = await db.Favorites.FirstOrDefaultAsync(f => f.ProductId == productId && f.AppUserId == currentUserId);

            if(favorite == null)
            {
                return NotFound("Favorite not found");
            }

            db.FavoritesAll.Remove(favorite);
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new FavoriteRemoveResponse
            {
                ID = productId,
                Message = "Favorite removed successfully"

            });




        }







        
        public class FavoriteRemoveResponse : BaseResponse
        {

        }

    }
}
