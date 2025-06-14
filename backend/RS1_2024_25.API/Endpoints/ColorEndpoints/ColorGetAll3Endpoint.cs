using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Helper;
using static RS1_2024_25.API.Endpoints.ColorEndpoints.ColorGetAll3Endpoint;
using Microsoft.AspNetCore.Authorization;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints;
[Authorize]
[Route("color")]
public class ColorGetAll3Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<ColorGetAll3Request>
    .WithResult<MyPagedList<ColorGetAll3Response>>
{
    [HttpGet("filter")]
    public override async Task<MyPagedList<ColorGetAll3Response>> HandleAsync([FromQuery] ColorGetAll3Request request, CancellationToken cancellationToken = default)
    {
        // Kreiranje osnovnog query-a
        var query = db.Colors
            .AsQueryable();

        // Primjena filtera na osnovu naziva grada
        if (!string.IsNullOrWhiteSpace(request.Q))
        {

            query = query.Where(c => c.Name.ToLower().Contains(request.Q)

           );

        }

        // Filter by Hex Code if provided
        if (!string.IsNullOrWhiteSpace(request.Hex_Code))
        {
            query = query.Where(c => c.Hex_Code.ToLower() == request.Hex_Code.ToLower());
        }


        // Projektovanje u rezultatni tip
        var projectedQuery = query.Select(c => new ColorGetAll3Response
        {
            ID = c.ID,
            Name = c.Name,
            Hex_Code = c.Hex_Code,
        });

        // Kreiranje paginiranog odgovora sa filterom
        var result = await MyPagedList<ColorGetAll3Response>.CreateAsync(projectedQuery, request, cancellationToken);


        return result;
    }
    public class ColorGetAll3Request : MyPagedRequest //naslijeđujemo
    {
        public string? Q { get; set; } = string.Empty;

        public string? Hex_Code { get; set; }

    }

    public class ColorGetAll3Response
    {
        public required int ID { get; set; }
        public string? Name { get; set; }
        public string? Hex_Code { get; set; }


    }
}
