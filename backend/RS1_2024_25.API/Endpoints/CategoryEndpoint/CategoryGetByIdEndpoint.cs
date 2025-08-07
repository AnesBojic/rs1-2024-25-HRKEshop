using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.CategoryEndpoint.CategoryGetByIdEndpoint;

namespace RS1_2024_25.API.Endpoints.CategoryEndpoint
{
    [Authorize]
    [Route("category")]
    public class CategoryGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<CategoryGetByIdResponse>
    {
        [HttpGet("{id}")]
        public override async Task<ActionResult<CategoryGetByIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await db.Categories
                                .Where(c => c.ID == id)
                                .Select(c => new CategoryGetByIdResponse
                                {
                                    ID = c.ID,
                                    Name = c.Name,
                                    TenantId = c.TenantId

                                })
                                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (category == null)
            {
                throw new ArgumentException("Category not found");
            }


            return Ok(category);
        }

        public class CategoryGetByIdResponse
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
            public required int TenantId { get; set; }


        }
    }
}
