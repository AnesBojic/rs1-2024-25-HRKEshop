using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using Microsoft.EntityFrameworkCore;
using static RS1_2024_25.API.Endpoints.CategoryEndpoint.CategoryUpdateOrInsertEndpoint;

namespace RS1_2024_25.API.Endpoints.CategoryEndpoint
{
    [Authorize(Roles = "Admin")]
    [Route("category/UpdateOrInsert")]
    // UNCOMMENT THIS LINE TO ENABLE AUTHORIZATION
    public class CategoryUpdateOrInsertEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
       .WithRequest<CategoryUpdateOrInsertRequest>
       .WithActionResult<int>
    {
        [HttpPost]  // Using POST to support both create and update
        public override async Task<ActionResult<int>> HandleAsync(
            [FromBody] CategoryUpdateOrInsertRequest request,
            CancellationToken cancellationToken = default)
        {

            // Check if it's an insert or update operation
            bool isInsert = (request.ID == null || request.ID == 0);
            Category? category;

            if (isInsert)
            {
                category = new Category();


                db.Add(category);
            }
            else
            {
                // Update operation: retrieve the existing brand
                category = await db.Categories

                    .SingleOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

                if (category == null)
                {
                    return NotFound("Category not found");
                }
            }



            category.Name = request.Name;




            await db.SaveChangesAsync(cancellationToken);

            return Ok(category.ID);
        }

        public class CategoryUpdateOrInsertRequest
        {
            public int? ID { get; set; }
            public string Name { get; set; }


        }
    }
}
