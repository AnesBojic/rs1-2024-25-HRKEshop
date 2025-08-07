using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.FavoriteEndpoints
{
    [Authorize]
    [Route("favorites/my")]
    public class FavoritesGetAllForUserEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<List<FavoritesGetAllForUserEndpoint.FavoriteGetAllForUserResponse>>

    {
        [HttpGet]
        public override async Task<ActionResult<List<FavoriteGetAllForUserResponse>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = db.GetUserIdThrow();

            var favorites = await db.Favorites.Include(f => f.Product).Where(f => f.AppUserId == currentUserId).Select
                (f => new FavoriteGetAllForUserResponse
                {
                    ProductId = f.ProductId,
                    ProductName = f.Product.Name


                }).ToListAsync();

            return Ok(favorites);

        }






        public class FavoriteGetAllForUserResponse
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            //Can be extended Later

        }
    }
}
