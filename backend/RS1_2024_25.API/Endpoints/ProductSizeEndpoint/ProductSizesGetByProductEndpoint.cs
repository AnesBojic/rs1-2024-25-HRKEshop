
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductSizeEndpoint
{
    [Authorize]
    [Route("products/{productId}/sizes")]
    public class ProductSizesGetByProductEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<List<ProductSizesGetByProductEndpoint.ProductSizeResponse>>
    {
        [HttpGet]
        public override async Task<ActionResult<List<ProductSizeResponse>>> HandleAsync([FromRoute] int productId, CancellationToken cancellationToken = default)
        {

            var productSize = await db.ProductSizes.Include(ps => ps.Size).Include(ps=> ps.Product).Where(ps => ps.ProductId == productId).ToListAsync();

            if(!productSize.Any())
            {
                return NotFound($"No sizes found for the product with the ID {productId}");
            }

            var response = productSize.Select(ps => new ProductSizeResponse
            {
                ProductName = ps.Product.Name,
                ProductSizeId = ps.ID,
                SizeId = ps.SizeId,
                SizeName = ps.Size.Value,
                PriceForItem = ps.Price ?? (decimal)ps.Product.Price,
                Stock = ps.Stock,


            }).ToList();

            return Ok(response);

            
        }





        public class ProductSizeResponse
        {
            public string ProductName { get; set; } = string.Empty;
            public int ProductSizeId { get; set; }
            public int SizeId { get; set; }
            public string SizeName { get; set; } = string.Empty;
            public decimal PriceForItem { get; set; }

            public int Stock { get; set; }



        }
    }
}
