using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{

    [Authorize]
    [Route("products/{productId}/ratings")]

    public class ProductRatingDeleteEndpoint(ApplicationDbContext db) :MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<ProductRatingDeleteEndpoint.ProductRatingDeleteResponse>
    {
        [HttpDelete]
        public override async Task<ActionResult<ProductRatingDeleteResponse>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {
            var currentUserId = db.GetUserIdThrow();

            var existingProductRating = await db.ProductRatings.Where(pr => pr.ProductId == productId && pr.AppUserId == currentUserId).FirstOrDefaultAsync();


            if(existingProductRating == null )
            {
                return NotFound("Product rating not found.");
            }


            db.ProductRatingsAll.Remove(existingProductRating);
            await db.SaveChangesAsync(cancellationToken);


            return Ok(new ProductRatingDeleteResponse
            {
                ID = existingProductRating.ID,
                Message = "Product rating removed succesfully."

            });





            
        }




        public class ProductRatingDeleteResponse : BaseResponse
        {


        }
    }
}
