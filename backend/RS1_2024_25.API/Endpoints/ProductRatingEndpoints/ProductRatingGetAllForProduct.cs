using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using System.Net.WebSockets;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{

    [Authorize]
    [Route("ratings/product/{productId}")]
    public class ProductRatingGetAllForProduct(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<List<ProductRatingGetAllForProduct.ProductRatingGetAllForProductResponse>>
    {

        [HttpGet]
        public override async Task<ActionResult<List<ProductRatingGetAllForProductResponse>>> HandleAsync([FromRoute]int productId, CancellationToken cancellationToken = default)
        {

            var doesProductExist = await db.Products.AnyAsync(p => p.ID == productId);
            if(!doesProductExist)
            {
                return NotFound("Product does not exist.");
            }

            var allRatings = await db.ProductRatings.Include(pr => pr.AppUser).Where(pr => pr.ProductId == productId).Select
                (pr => new ProductRatingGetAllForProductResponse
                {
                    UserId = pr.AppUserId,
                    FullName = $"{pr.AppUser.Name} {pr.AppUser.Surname}",
                    Rating = pr.Rating,
                    Comment = pr.Comment,
                    CreatedAt = pr.CreatedAt.ToString("yyyy-MM-dd HH:mm")





                }).ToListAsync(cancellationToken);


            return Ok(allRatings);

            
        }







        public class ProductRatingGetAllForProductResponse
        {
            public int UserId { get; set; }
            public string FullName { get; set; } = string.Empty;

            public int Rating { get; set; }

            public string? Comment { get; set; }
            public string CreatedAt { get; set; }





        }
    }
}
