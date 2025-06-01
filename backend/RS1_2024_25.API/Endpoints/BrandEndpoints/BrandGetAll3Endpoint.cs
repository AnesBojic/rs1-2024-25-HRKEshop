using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Helper;
using static RS1_2024_25.API.Endpoints.BrandEndpoints.BrandGetAll3Endpoint;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints;
//sa paging i sa filterom
[Route("brand")]
public class BrandGetAll3Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<BrandGetAll3Request>
    .WithResult<MyPagedList<BrandGetAll3Response>>
{
    [HttpGet("filter")]
    public override async Task<MyPagedList<BrandGetAll3Response>> HandleAsync([FromQuery] BrandGetAll3Request request, CancellationToken cancellationToken = default)
    {
        // Creating bassic query for Brands
        var query = db.Brands
            .AsQueryable();

       
        if (!string.IsNullOrWhiteSpace(request.Name))
        {

            query = query.Where(b => b.Name.ToLower().Contains(request.Name)

           );

        }

    


        // Projektovanje u rezultatni tip
        var projectedQuery = query.Select(c => new BrandGetAll3Response
        {
            ID = c.ID,
            Name = c.Name,
            
        });

        // Kreiranje paginiranog odgovora sa filterom
        var result = await MyPagedList<BrandGetAll3Response>.CreateAsync(projectedQuery, request, cancellationToken);


        return result;
    }
    public class BrandGetAll3Request : MyPagedRequest //inherits from MyPagedRequest for pagination
    {
        public string? Name { get; set; } = string.Empty;

       

    }

    public class BrandGetAll3Response
    {
        public required int ID { get; set; }
        public string? Name { get; set; }
        


    }
}
