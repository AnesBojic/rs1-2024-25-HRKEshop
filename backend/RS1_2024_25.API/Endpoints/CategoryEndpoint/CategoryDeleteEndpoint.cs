using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CategoryEndpoint;

[Authorize(Roles = "Admin")]
[Route("category/delete")]
public class ProductDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<int>
    .WithoutResult
{

    [HttpDelete("{id}")]
    public override async Task HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await db.Categories.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException("Category not found");



        db.CategoryAll.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
    }
}
