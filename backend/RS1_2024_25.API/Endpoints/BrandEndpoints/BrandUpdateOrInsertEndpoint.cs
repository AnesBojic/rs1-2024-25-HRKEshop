using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using static RS1_2024_25.API.Endpoints.BrandEndpoints.BrandUpdateOrInsertEndpoint;
using Microsoft.AspNetCore.Authorization;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints;

[Authorize(Roles = "Admin")]
[Route("brands/UpdateOrInsert")]

public class BrandUpdateOrInsertEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<BrandUpdateOrInsertRequest>
    .WithActionResult<int>
{
    [HttpPost]  // Using POST to support both create and update
    public override async Task<ActionResult<int>> HandleAsync(
        [FromBody] BrandUpdateOrInsertRequest request,
        CancellationToken cancellationToken = default)
    {

        // Check if it's an insert or update operation
        bool isInsert = (request.ID == null || request.ID == 0);
        Brand? brand;

        if (isInsert)
        {
            brand = new Brand();


            db.Add(brand);
        }
        else
        {
            // Update operation: retrieve the existing brand
            brand = await db.Brands
                //.Include(x => x.User)
                .SingleOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

            if (brand == null)
            {
                return NotFound("Brand not found");
            }
        }

        // Set common properties for both insert and update
        brand.Name = request.Name;
        


        // Save changes to the database
        await db.SaveChangesAsync(cancellationToken);

        return Ok(brand.ID); // Return the ID of the student
    }

    public class BrandUpdateOrInsertRequest
    {
        public int? ID { get; set; } // Nullable to allow null for insert operations
        public string Name { get; set; }
       

    }
}
