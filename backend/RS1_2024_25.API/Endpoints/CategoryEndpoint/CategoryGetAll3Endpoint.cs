using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Helper;
using static RS1_2024_25.API.Endpoints.CategoryEndpoint.CategoryGetAll3Endpoint;

namespace RS1_2024_25.API.Endpoints.CategoryEndpoint;

//sa paging i sa filterom
[Authorize]
[Route("Category")]
public class CategoryGetAll3Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<CategoryGetAll3Request>
    .WithResult<MyPagedList<CategoryGetAll3Response>>
{
    [HttpGet("filter")]
    public override async Task<MyPagedList<CategoryGetAll3Response>> HandleAsync([FromQuery] CategoryGetAll3Request request, CancellationToken cancellationToken = default)
    {
        // Kreiranje osnovnog query-a
        var query = db.Categories
            .AsQueryable();

        // Primjena filtera na osnovu naziva grada
        if (!string.IsNullOrWhiteSpace(request.Q))
        {

            query = query.Where(p => p.Name.ToLower().Contains(request.Q)

           );

        }




        // Projektovanje u rezultatni tip
        var projectedQuery = query.Select(p => new CategoryGetAll3Response
        {
            ID = p.ID,
            Name = p.Name,
         
        });

        // Kreiranje paginiranog odgovora sa filterom
        var result = await MyPagedList<CategoryGetAll3Response>.CreateAsync(projectedQuery, request, cancellationToken);


        return result;
    }
    public class CategoryGetAll3Request : MyPagedRequest //naslijeđujemo
    {
        public string? Q { get; set; } = string.Empty;


    }

    public class CategoryGetAll3Response
    {
        public required int ID { get; set; }
        public string Name { get; set; }
       

    }
}

