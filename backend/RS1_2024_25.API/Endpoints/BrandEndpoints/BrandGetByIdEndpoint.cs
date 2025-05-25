using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.BrandEndpoints.BrandGetByIdEndpoint;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints;



[Route("brand")]
public class BrandGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<BrandGetByIdResponse>
{
    [HttpGet("{id}")]
    public override async Task<ActionResult<BrandGetByIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        var color = await db.Brands
                            .Where(c => c.ID == id)
                            .Include(c => c.Tenant) // Assuming you want to include Tenant information
                            .Select(c => new BrandGetByIdResponse
                            {
                                ID = c.ID,
                                Name = c.Name,
                                TenantId = c.TenantId

                            })
                            .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

        if (color == null)
        {
            throw new ArgumentException("Color not found");
        }


        return Ok(color);
    }

    public class BrandGetByIdResponse
    {
        public required int ID { get; set; }
        public required string Name { get; set; }
        public required int TenantId { get; set; }

    }
}