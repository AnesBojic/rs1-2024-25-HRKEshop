using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Helper;
using static RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints.CategoriesProductsGetAll3Endpoint;

namespace RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints;

[Authorize]
[Route("CategoriesProducts")]
public class CategoriesProductsGetAll3Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<CategoriesProductsGetAll3Request>
    .WithResult<MyPagedList<CategoriesProductsGetAll3Response>>
{
    [HttpGet("filter")]
    public override async Task<MyPagedList<CategoriesProductsGetAll3Response>> HandleAsync([FromQuery] CategoriesProductsGetAll3Request request, CancellationToken cancellationToken = default)
    {
        var query = db.CategoriesProducts.AsQueryable();

        // Filtriranje po CategoryId ako je specificirano
        if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
        {
            query = query.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        // Filtriranje po ProductId ako je specificirano
        if (request.ProductId.HasValue && request.ProductId.Value > 0)
        {
            query = query.Where(c => c.ProductId == request.ProductId.Value);
        }

        // Projektovanje u DTO/response
        var projectedQuery = query.Select(p => new CategoriesProductsGetAll3Response
        {
            ID = p.ID,
            CategoryId = p.CategoryId,
            ProductId = p.ProductId
        });

        // Vraćanje paginiranog rezultata
        var result = await MyPagedList<CategoriesProductsGetAll3Response>.CreateAsync(projectedQuery, request, cancellationToken);

        return result;
    }

    public class CategoriesProductsGetAll3Request : MyPagedRequest
    {
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
    }

    public class CategoriesProductsGetAll3Response
    {
        public required int ID { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
    }
}
