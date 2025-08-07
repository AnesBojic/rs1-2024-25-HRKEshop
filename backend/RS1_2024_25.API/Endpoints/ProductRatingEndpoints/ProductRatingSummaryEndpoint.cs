using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{
    [Authorize]
    [Route("products/{productId}/ratings/summary")]
    public class ProductRatingSummaryEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<ProductRatingSummaryEndpoint.ProductRatingSummaryEndpointResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<ProductRatingSummaryEndpointResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {
            var doesProductExist = await db.Products.AnyAsync(p => p.ID == productId,cancellationToken);

            if(!doesProductExist)
            {
                return NotFound("Product not found.");
            }

            var ratingsQuery = db.ProductRatings.Where(pr => pr.ProductId == productId);

            var count = await ratingsQuery.CountAsync(cancellationToken);

            var average = count == 0 ? 0 : await ratingsQuery.AverageAsync(r => r.Rating, cancellationToken);


            return Ok(new ProductRatingSummaryEndpointResponse
            {
                ProductId = productId,
                AverageRating = (float)Math.Round(average, 2),
                RatingsCount = count



            });







        }






        public class ProductRatingSummaryEndpointResponse
        {

            public int ProductId { get; set; }

            public float AverageRating { get; set; }

            public int RatingsCount { get; set; }



        }
    }
}
