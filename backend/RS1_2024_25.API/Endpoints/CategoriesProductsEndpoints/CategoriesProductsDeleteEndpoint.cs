using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CategoriesProductsEndpoints;

[Authorize(Roles = "Admin")]
[Route("categoriesProducts/delete")]
public class CategoriesProductsDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
    .WithRequest<int>
    .WithoutResult
{

    [HttpDelete("{id}")]
    public override async Task HandleAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoriesProducts = await db.CategoriesProducts.SingleOrDefaultAsync(x => x.ID == id, cancellationToken);

        if (categoriesProducts == null)
            throw new KeyNotFoundException("CategoryProducts was not found");



        db.Categories_ProductsAll.Remove(categoriesProducts);
        await db.SaveChangesAsync(cancellationToken);
    }
}
