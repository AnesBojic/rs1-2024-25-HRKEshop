using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{
    [Authorize]
    [Route("ratings/add")]
    public class ProductRatingAddOrUpdateEndpoint(ApplicationDbContext db,IValidator<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest> validator) : MyEndpointBaseAsync
        .WithRequest<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest>
        .WithActionResult<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateResponse>

    {
        [HttpPost]
        public override async Task<ActionResult<ProductRatingAddOrUpdateResponse>> HandleAsync([FromBody]ProductRatingAddOrUpdateRequest request, CancellationToken cancellationToken = default)
        {

            var validationProblems = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);

            if(validationProblems != null)
            {
                return validationProblems;
            }


            var doesProductExist = await db.Products.Where(p => p.ID == request.ProductId).AnyAsync();

            if(!doesProductExist)
            {
                return NotFound("Product is not found");
            }

            var currentUserId = db.GetUserIdThrow();


            var existingRating = await db.ProductRatings.Where(pr => pr.AppUserId == currentUserId && pr.ProductId == request.ProductId).FirstOrDefaultAsync();



        

            if(existingRating != null)
            {
                existingRating.Rating = request.Rating;
                existingRating.Comment = request.Comment;


            }
            else
            {
                db.ProductRatingsAll.Add(new ProductRating
                {
                    ProductId = request.ProductId,
                    AppUserId = currentUserId,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
                    Rating = request.Rating



                });

            }
            await db.SaveChangesAsync(cancellationToken);


            return Ok(new ProductRatingAddOrUpdateResponse
            {
                ID = request.ProductId,
                Message = existingRating != null ? "Rating updated succesfully" : "Rating added succesfully"


            });




        }














        public class ProductRatingAddOrUpdateRequest
        {
            public required int ProductId { get; set; }
            public required int Rating { get; set; }

            public string? Comment { get; set; }

        }

        public class ProductRatingAddOrUpdateResponse : BaseResponse
        {

        }
    }
}
