using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using Microsoft.EntityFrameworkCore;
using static RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints.CategoriesProductsUpdateOrInsertEndpoint;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
namespace RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints;

[Authorize(Roles = "Admin")]
[Route("categoriesProducts/UpdateOrInsert")]
// UNCOMMENT THIS LINE TO ENABLE AUTHORIZATION
public class CategoriesProductsUpdateOrInsertEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
       .WithRequest<CategoriesProductsUpdateOrInsertRequest>
       .WithActionResult<int>
{
    [HttpPost]  // Using POST to support both create and update
    public override async Task<ActionResult<int>> HandleAsync(
        [FromBody] CategoriesProductsUpdateOrInsertRequest request,
        CancellationToken cancellationToken = default)
    {

        // Check if it's an insert or update operation
        bool isInsert = (request.ID == null || request.ID == 0);
        categories_products? categoryProduct;

        if (isInsert)
        {
            categoryProduct = new categories_products();


            db.Add(categoryProduct);
        }
        else
        {
            // Update operation: retrieve the existing brand
            categoryProduct = await db.CategoriesProducts

                .SingleOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

            if (categoryProduct == null)
            {
                return NotFound("CategoryProduct not found");
            }
        }



        categoryProduct.ProductId = request.ProductId;
        categoryProduct.CategoryId = request.CategoryId;




        await db.SaveChangesAsync(cancellationToken);

        return Ok(categoryProduct.ID);
    }

    public class CategoriesProductsUpdateOrInsertRequest
    {
        public int? ID { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }


    }
}

