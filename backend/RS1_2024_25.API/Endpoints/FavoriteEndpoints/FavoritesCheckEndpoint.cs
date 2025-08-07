using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.FavoriteEndpoints
{

    [Authorize]
    [Route("favorites/check/{productId}")]
    public class FavoritesCheckEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<FavoritesCheckEndpoint.FavoriteCheckResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<FavoriteCheckResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {
            var currentUserId = db.GetUserIdThrow();

            var isFavorited = await db.Favorites.AnyAsync(f =>
            f.ProductId == productId && f.AppUserId == currentUserId, cancellationToken);

            return Ok(new FavoriteCheckResponse
            {
                ProductId = productId,
                IsFavorited = isFavorited
            });


        }



        public class FavoriteCheckResponse
        {
            public int ProductId { get; set; }

            public bool IsFavorited { get; set; }


        }

    }
}
