using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints
{
    [Authorize(Roles = "Admin")]
    [Route("brands/delete")]
    public class BrandDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithoutResult
    {

        [HttpDelete("{id}")]
        public override async Task HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var brand = await db.Brands.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (brand == null)
                throw new KeyNotFoundException("Brand not found");



            db.BrandsAll.Remove(brand);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
