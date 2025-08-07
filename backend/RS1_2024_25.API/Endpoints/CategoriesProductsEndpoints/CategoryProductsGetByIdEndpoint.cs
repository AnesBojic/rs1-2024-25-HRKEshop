using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints.CategoryProductsGetByIdEndpoint;

namespace RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints
{
    [Authorize]
    [Route("CategoriesProducts")]
    public class CategoryProductsGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<CategoriesProductsGetByIdResponse>
    {
        [HttpGet("{id}")]
        public override async Task<ActionResult<CategoriesProductsGetByIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var categoryProduct = await db.CategoriesProducts
                                .Where(c => c.ID == id)
                                .Select(c => new CategoriesProductsGetByIdResponse
                                {
                                    ID = c.ID,
                                    ProductId = c.ProductId,
                                    CategoryId = c.CategoryId,
                                    TenantId = c.TenantId

                                })
                                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (categoryProduct == null)
            {
                throw new ArgumentException("CategoryProduct was not found");
            }


            return Ok(categoryProduct);
        }

        public class CategoriesProductsGetByIdResponse
        {
            public required int ID { get; set; }
            public required int ProductId { get; set; }
            public required int CategoryId { get; set; }
            public required int TenantId { get; set; }


        }
    }
}
