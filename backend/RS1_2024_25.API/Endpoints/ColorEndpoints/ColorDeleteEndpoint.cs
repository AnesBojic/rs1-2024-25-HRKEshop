using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Migrations;
using RS1_2024_25.API.Services;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints
{

    [MyAuthorization(isAdmin: true, isManager: false)]
    // UNCOMMENT THIS LINE TO ENABLE AUTHORIZATION
    [Route("colors")]
    public class ColorDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithoutResult
    {

        [HttpDelete("{id}")]
        public override async Task HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var color = await db.Colors.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (color == null)
                throw new KeyNotFoundException("Color not found");

       

            db.Colors.Remove(color);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

}
