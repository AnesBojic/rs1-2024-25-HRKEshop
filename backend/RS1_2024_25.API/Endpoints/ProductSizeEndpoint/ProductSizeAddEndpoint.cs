using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductSizeEndpoint
{

    [Authorize(Roles = "Manager")]
    [Route("productSize/add/")]
    public class ProductSizeAddEndpoint(ApplicationDbContext db,IValidator<ProductSizeAddEndpoint.ProductSizeAddRequest> validator) : MyEndpointBaseAsync
        .WithRequest<ProductSizeAddEndpoint.ProductSizeAddRequest>
        .WithActionResult<ProductSizeAddEndpoint.ProductSizeAddResponse>

    {
        [HttpPost]
        public override async Task<ActionResult<ProductSizeAddResponse>> HandleAsync([FromBody] ProductSizeAddRequest request, CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);

            if(validationProblem != null)
            {
                return validationProblem;
            }



            var productExisting = await db.Products.FirstOrDefaultAsync(p => p.ID == request.ProductId);

            //Already checking this in validator
            /*if (productExisting == null)
            {
                return NotFound("No product :(");

            }*/

            var productSize = new ProductSize
            {
                ProductId = productExisting.ID!,
                SizeId = request.SizeId,
                Price = request.Price ?? (decimal)productExisting.Price,
                Stock = request.Stock



            };

            db.ProductsSizesAll.Add(productSize);
            await db.SaveChangesAsync();

            return Ok(new ProductSizeAddResponse
            {
                ID = productSize.ID,
                Message = "Product size for given product is succesfully created"


            });

        }








        public class ProductSizeAddResponse
        {
            public int ID { get; set; }

            public string Message { get; set; } = string.Empty;


        }

        public class ProductSizeAddRequest
        {
            public required int ProductId { get; set; }
            public required int SizeId { get; set; }

            public decimal? Price { get; set; }

            public int Stock { get; set; }


        }

    }
}
