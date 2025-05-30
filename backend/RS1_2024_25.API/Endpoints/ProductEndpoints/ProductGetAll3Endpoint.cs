using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Helper;
using static RS1_2024_25.API.Endpoints.ProductEndpoints.ProductGetAll3Endpoint;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using System.ComponentModel.DataAnnotations.Schema;
using RS1_2024_25.API.Data.Enums;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;

//sa paging i sa filterom
[Route("product")]
public class ProductGetAll3Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<ProductGetAll3Request>
    .WithResult<MyPagedList<ProductGetAll3Response>>
{
    [HttpGet("filter")]
    public override async Task<MyPagedList<ProductGetAll3Response>> HandleAsync([FromQuery] ProductGetAll3Request request, CancellationToken cancellationToken = default)
    {
        // Kreiranje osnovnog query-a
        var query = db.Products
            .AsQueryable();

        // Primjena filtera na osnovu naziva grada
        if (!string.IsNullOrWhiteSpace(request.Q))
        {

            query = query.Where(p => p.Name.ToLower().Contains(request.Q) 
          
           );

        }

        // Filter by Gender if provided
        if (request.Gender.HasValue)
        {
            query = query.Where(p => p.Gender == request.Gender.Value);
        }

        // Filter by minimum price
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        // Filter by maximum price
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        if (request.ColorId.HasValue)
        {
            query = query.Where(c => c.ColorId == request.ColorId.Value);
        }

        if (request.BrandId.HasValue)
        {
            query = query.Where(c => c.BrandId == request.BrandId.Value);
        }



        // Projektovanje u rezultatni tip
        var projectedQuery = query.Select(p => new ProductGetAll3Response
        {
            ID = p.ID,
            Name = p.Name,
            Price = p.Price,
            Gender = p.Gender,
            ColorId = p.ColorId,
            BrandId = p.BrandId
        });

        // Kreiranje paginiranog odgovora sa filterom
        var result = await MyPagedList<ProductGetAll3Response>.CreateAsync(projectedQuery, request, cancellationToken);


        return result;
    }
    public class ProductGetAll3Request : MyPagedRequest //naslijeđujemo
    {
        public string? Q { get; set; } = string.Empty;

        public Gender? Gender { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public int? ColorId { get; set; }
        public int? BrandId { get; set; }

    }

    public class ProductGetAll3Response
    {
        public required int ID { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Gender Gender { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
       
    }
}
