using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Enums;
using RS1_2024_25.API.Helper.Api;
using static RS1_2024_25.API.Endpoints.ProductEndpoints.ProductGetByIdEndpoint;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;

[Authorize]
[Route("product")]
public class ProductGetByIdEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<ProductGetByIdResponse>
{
    [HttpGet("{id}")]
    public override async Task<ActionResult<ProductGetByIdResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        var color = await db.Products
                            .Where(p => p.ID == id)
                            .Select(p => new ProductGetByIdResponse
                            {
                                ID = p.ID,
                                Name = p.Name,
                                Price = p.Price,
                                Gender = p.Gender,
                                ColorId = p.ColorId,
                                BrandId = p.BrandId,
                                CategoryId = p.CategoryId,
                                CategoryName = p.Category != null ? p.Category.Name : null,
                                TenantId = p.TenantId,
                                ImageUrl = db.ImagesAll
                                    .Where(img => img.ImageableId == p.ID && img.ImageableType.ToLower() == "products")
                                    .OrderByDescending(img => img.UpdatedAt)
                                    .Select(img => img.Url)
                                    .FirstOrDefault()
                            })
                            .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

        if (color == null)
        {
            throw new ArgumentException("Product not found");
        }


        return Ok(color);
    }

    public class ProductGetByIdResponse
    {
        public required int ID { get; set; }
        public required string Name { get; set; }
        public float Price { get; set; }
        public Gender Gender { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public required int TenantId { get; set; }
        public string? ImageUrl { get; set; }
    }
}