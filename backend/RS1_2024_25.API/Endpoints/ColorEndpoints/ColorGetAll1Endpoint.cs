using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.ColorEndpoints.ColorGetAll1Endpoint;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints
{
    [Route("colors")]
    public class ColorGetAll1Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
     .WithoutRequest
     .WithResult<ColorGetAll1Response[]>
    {
        [HttpGet("all")]
        public override async Task<ColorGetAll1Response[]> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = await db.Colors
                            .Select(c => new ColorGetAll1Response
                            {
                                ID = c.ID,
                                Name = c.Name,
                                Hex_Code = c.Hex_Code

                            })
                            .ToArrayAsync(cancellationToken);

            return result;
        }

        public class ColorGetAll1Response
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
            public required string Hex_Code { get; set; }

        }
    }
}