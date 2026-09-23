using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Enums;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using System.ComponentModel.DataAnnotations.Schema;
using static RS1_2024_25.API.Endpoints.ProductEndpoints.ProductUpdateOrInsertEndpoint;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;

[Authorize(Roles = "Admin")]
[Route("products/UpdateOrInsert")]
// UNCOMMENT THIS LINE TO ENABLE AUTHORIZATION
public class ProductUpdateOrInsertEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
   .WithRequest<ProductUpdateOrInsertRequest>
   .WithActionResult<int>
{
    [HttpPost]  // Using POST to support both create and update
    public override async Task<ActionResult<int>> HandleAsync(
        [FromBody] ProductUpdateOrInsertRequest request,
        CancellationToken cancellationToken = default)
    {

        // Check if it's an insert or update operation
        bool isInsert = (request.ID == null || request.ID == 0);
        Product? product;

        if (isInsert)
        {
            product = new Product();


            db.Add(product);
        }
        else
        {
            // Update operation: retrieve the existing brand
            product = await db.Products
                .Include(b => b.Brand)
                .Include(c => c.Color)
                .SingleOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

            if (product == null)
            {
                return NotFound("Product not found");
            }
        }



        product.Name = request.Name;
        product.Price = request.Price;
        product.Gender = request.Gender;
        product.ColorId = request.ColorId;
        product.BrandId = request.BrandId;
        var categoryId = await ResolveCategoryIdAsync(request, cancellationToken);
        if (categoryId == null)
        {
            return BadRequest("Select an existing category or enter a new category name.");
        }

        product.CategoryId = categoryId.Value;

        await db.SaveChangesAsync(cancellationToken);

        return Ok(product.ID);
    }

    private async Task<int?> ResolveCategoryIdAsync(ProductUpdateOrInsertRequest request, CancellationToken cancellationToken)
    {
        var newName = request.NewCategoryName?.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            var existing = await db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == newName.ToLower(), cancellationToken);

            if (existing != null)
            {
                return existing.ID;
            }

            var category = new Category { Name = newName };
            db.Add(category);
            await db.SaveChangesAsync(cancellationToken);
            return category.ID;
        }

        var categoryId = request.CategoryId ?? 0;
        var categoryExists = await db.Categories.AnyAsync(c => c.ID == categoryId, cancellationToken);
        if (!categoryExists)
        {
            return null;
        }

        return categoryId;
    }

    public class ProductUpdateOrInsertRequest
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Gender Gender { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? NewCategoryName { get; set; }
    }
}