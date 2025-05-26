using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using static RS1_2024_25.API.Endpoints.ColorEndpoints.ColorUpdateOrInsertEndpoint;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.SharedTables;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints;

[Route("colors")]
[MyAuthorization(isAdmin: true, isManager: false)]
// UNCOMMENT THIS LINE TO ENABLE AUTHORIZATION
public class ColorUpdateOrInsertEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
   .WithRequest<ColorUpdateOrInsertRequest>
   .WithActionResult<int>
{
    [HttpPost]  // Using POST to support both create and update
    public override async Task<ActionResult<int>> HandleAsync(
        [FromBody] ColorUpdateOrInsertRequest request,
        CancellationToken cancellationToken = default)
    {

        // Check if it's an insert or update operation
        bool isInsert = (request.ID == null || request.ID == 0);
        Color? color;

        if (isInsert)
        {
            color = new Color();


            db.Add(color);
        }
        else
        {
            // Update operation: retrieve the existing Color
            color = await db.Colors
                //.Include(x => x.User)
                .SingleOrDefaultAsync(x => x.ID == request.ID, cancellationToken);

            if (color == null)
            {
                return NotFound("Color not found");
            }
        }

        // Set common properties for both insert and update
        color.Name = request.Name;
        color.Hex_Code = request.Hex_Code;



        // Save changes to the database
        await db.SaveChangesAsync(cancellationToken);

        return Ok(color.ID); // Return the ID of the student
    }

    public class ColorUpdateOrInsertRequest
    {
        public int? ID { get; set; } // Nullable to allow null for insert operations
        public string Name { get; set; }
        public string Hex_Code { get; set; } 

    }
}