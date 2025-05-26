using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Enums;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;
using System.ComponentModel.DataAnnotations.Schema;
using static RS1_2024_25.API.Endpoints.ProductEndpoints.ProductGetAll1Endpoint;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;

[Route("products")]
public class ProductGetAll1Endpoint(ApplicationDbContext db) : MyEndpointBaseAsync
 .WithoutRequest
 .WithResult<ProductGetAll1Response[]>
{
    [HttpGet("all")]
    public override async Task<ProductGetAll1Response[]> HandleAsync(CancellationToken cancellationToken = default)
    {
        var result = await db.Products
                        .Select(b => new ProductGetAll1Response                     
                        {
                            ID = b.ID,
                            Name = b.Name,
                            Price = b.Price,
                            Gender = b.Gender,
                            ColorId = b.ColorId,
                            BrandId = b.BrandId,
                            TenantId = b.TenantId
                       

                        })
                        .ToArrayAsync(cancellationToken);

        return result;
    }

    public class ProductGetAll1Response
    {
        public required int ID { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
        public Gender Gender { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public required int TenantId { get; set; }


        

    }
}