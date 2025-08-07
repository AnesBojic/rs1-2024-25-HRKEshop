using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.CategoryEndpoint.CategoryGetAll1Endpoint;

namespace RS1_2024_25.API.Endpoints.CategoryEndpoint;
[Authorize]
[Route("category")]
public class CategoryGetAll1Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
 .WithoutRequest
 .WithResult<CategoryGetAll1Response[]>
{
    [HttpGet("all")]
    public override async Task<CategoryGetAll1Response[]> HandleAsync(CancellationToken cancellationToken = default)
    {
        var result = await db.Categories
                        .Select(b => new CategoryGetAll1Response
                        {
                            ID = b.ID,
                            Name = b.Name,


                        })
                        .ToArrayAsync(cancellationToken);

        return result;
    }

    public class CategoryGetAll1Response
    {
        public required int ID { get; set; }
        public required string Name { get; set; }

    }
}

