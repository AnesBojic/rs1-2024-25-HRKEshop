using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.BrandEndpoints.BrandGetAll1Endpoint;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints
{
    [Authorize]
    [Route("brands")]
    public class BrandGetAll1Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
     .WithoutRequest
     .WithResult<BrandGetAll1Response[]>
    {

       

        [HttpGet("all")]
        public override async Task<BrandGetAll1Response[]> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = await db.Brands
                            .Select(b => new BrandGetAll1Response
                            {
                                ID = b.ID,
                                Name = b.Name,
                                TenantId = b.TenantId


                            })
                            .ToArrayAsync(cancellationToken);

            return result;
        }

        public class BrandGetAll1Response
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
            public required int TenantId { get; set; }

        }
    }
}