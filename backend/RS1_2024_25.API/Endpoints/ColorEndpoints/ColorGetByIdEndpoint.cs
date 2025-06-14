using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.ColorEndpoints.ColorGetByIdEndpoint;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints
{
    [Authorize]
    [Route("color")]
    public class ColorGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<ColorGetByIdResponse>
    {
        [HttpGet("{id}")]
        public override async Task<ActionResult<ColorGetByIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var color = await db.Colors
                                .Where(c => c.ID == id)
                                .Select(c => new ColorGetByIdResponse
                                {
                                    ID = c.ID,
                                    Name = c.Name,
                                    Hex_Code = c.Hex_Code,

                                })
                                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (color == null)
            {
                throw new ArgumentException("Color not found");
            }


            return Ok(color);
        }

        public class ColorGetByIdResponse
        {
            public required int ID { get; set; }
            public required string Name { get; set; }
            public required string Hex_Code { get; set; }

        }
    }
}