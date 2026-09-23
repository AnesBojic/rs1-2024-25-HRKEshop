using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Enums;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;

[Authorize]
[Route("product")]
public class ProductDetailsEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<int>
    .WithActionResult<ProductDetailsEndpoint.ProductDetailsResponse>
{
    [HttpGet("{id}/details")]
    public override async Task<ActionResult<ProductDetailsResponse>> HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.ID == id, cancellationToken);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        var color = await db.Colors
            .Where(c => c.ID == product.ColorId)
            .Select(c => new { c.Name, c.Hex_Code })
            .FirstOrDefaultAsync(cancellationToken);

        var brandName = await db.BrandsAll
            .Where(b => b.ID == product.BrandId)
            .Select(b => b.Name)
            .FirstOrDefaultAsync(cancellationToken);

        var siblings = await db.Products
            .Where(p => p.Name == product.Name)
            .OrderBy(p => p.ColorId)
            .ToListAsync(cancellationToken);

        var productIds = siblings.Select(p => p.ID).Append(product.ID).Distinct().ToList();
        var colorIds = siblings.Select(p => p.ColorId).Distinct().ToList();

        var colors = await db.Colors
            .Where(c => colorIds.Contains(c.ID))
            .ToDictionaryAsync(c => c.ID, cancellationToken);

        var images = await db.ImagesAll
            .Where(img => productIds.Contains(img.ImageableId) && img.ImageableType.ToLower() == "products")
            .GroupBy(img => img.ImageableId)
            .Select(group => new
            {
                ProductId = group.Key,
                Url = group.OrderByDescending(img => img.UpdatedAt).Select(img => img.Url).FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.ProductId, x => x.Url, cancellationToken);

        var sizes = await db.ProductsSizesAll
            .Where(ps => ps.ProductId == product.ID)
            .Include(ps => ps.Size)
            .OrderBy(ps => ps.Size.Value)
            .Select(ps => new ProductDetailsSizeResponse
            {
                ProductSizeId = ps.ID,
                SizeName = ps.Size.Value,
                Price = ps.Price ?? (decimal)product.Price,
                Stock = ps.Stock
            })
            .ToListAsync(cancellationToken);

        return Ok(new ProductDetailsResponse
        {
            Id = product.ID,
            Name = product.Name,
            Price = product.Price,
            Gender = product.Gender,
            ColorId = product.ColorId,
            ColorName = color?.Name ?? "",
            ColorHex = color?.Hex_Code ?? "#cccccc",
            BrandId = product.BrandId,
            BrandName = brandName ?? "",
            ImageUrl = images.GetValueOrDefault(product.ID),
            Sizes = sizes,
            ColorVariants = siblings.Select(sibling =>
            {
                colors.TryGetValue(sibling.ColorId, out var siblingColor);
                return new ProductColorVariantResponse
                {
                    Id = sibling.ID,
                    Name = sibling.Name,
                    Price = sibling.Price,
                    ColorId = sibling.ColorId,
                    ColorName = siblingColor?.Name ?? "",
                    ColorHex = siblingColor?.Hex_Code ?? "#cccccc",
                    ImageUrl = images.GetValueOrDefault(sibling.ID),
                    IsCurrent = sibling.ID == product.ID
                };
            }).ToList()
        });
    }

    public class ProductDetailsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float Price { get; set; }
        public Gender Gender { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; } = "";
        public string ColorHex { get; set; } = "";
        public int BrandId { get; set; }
        public string BrandName { get; set; } = "";
        public string? ImageUrl { get; set; }
        public List<ProductDetailsSizeResponse> Sizes { get; set; } = new();
        public List<ProductColorVariantResponse> ColorVariants { get; set; } = new();
    }

    public class ProductDetailsSizeResponse
    {
        public int ProductSizeId { get; set; }
        public string SizeName { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }

    public class ProductColorVariantResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float Price { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; } = "";
        public string ColorHex { get; set; } = "";
        public string? ImageUrl { get; set; }
        public bool IsCurrent { get; set; }
    }
}
