using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{

    [Authorize]
    [Route("products/{productId}/ratings/me")]
    public class ProductRatingGetMyRatingEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<ProductRatingGetMyRatingEndpoint.ProductRatingGetMyRatingEndpointResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<ProductRatingGetMyRatingEndpointResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();

            var productRating = await db.ProductRatings.Where(pr => pr.ProductId == productId && pr.AppUserId == userId).FirstOrDefaultAsync(cancellationToken);

            if(productRating == null)
            {
                return NotFound("You have not rated this product.");
            }

            return Ok(new ProductRatingGetMyRatingEndpointResponse
            {
                ProductId = productId,
                Comment = productRating.Comment,
                Rating = productRating.Rating


            });


            
        }



        public class ProductRatingGetMyRatingEndpointResponse
        {
            public int ProductId { get; set; }

            public int Rating { get; set; }

            public string? Comment { get; set; }
        }
    }
}
