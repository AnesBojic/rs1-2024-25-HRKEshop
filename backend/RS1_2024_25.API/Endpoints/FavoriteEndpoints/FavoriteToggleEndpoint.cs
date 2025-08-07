using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.FavoriteEndpoints
{

    //Also one for toggle for easier ui 
    [Authorize]
    [Route("favorites/toggle/{productId}")]
    public class FavoriteToggleEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<FavoriteToggleEndpoint.FavoriteToggleResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<FavoriteToggleResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {
            var currentUserId = db.GetUserIdThrow();

            var favorite = await db.Favorites.FirstOrDefaultAsync(f => f.ProductId == productId && f.AppUserId == currentUserId, cancellationToken);

            bool isFavorited;

            if (favorite == null)
            {
                var newFavorite = new Favorite
                {
                    AppUserId = currentUserId,
                    ProductId = productId,

                    

                };

                db.FavoritesAll.Add(newFavorite);
                isFavorited = true;
            }
            else
            {
                db.FavoritesAll.Remove(favorite);
                isFavorited = false;




            }

            await db.SaveChangesAsync(cancellationToken);

            return Ok(new FavoriteToggleResponse
            {
                ProductId = productId,
                IsFavorited = isFavorited,
                Message = isFavorited ? "Product added to favorites" : "Product removed from favorites"


            });




        }




        public class FavoriteToggleResponse
        {
            public int ProductId { get; set; }
            public bool IsFavorited { get; set; }

            public string Message { get; set; } = string.Empty;


        }
    }
}
