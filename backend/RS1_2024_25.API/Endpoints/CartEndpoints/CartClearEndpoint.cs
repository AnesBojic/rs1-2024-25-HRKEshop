using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("cart")]
    public class CartClearEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpDelete]
        public override async Task<ActionResult<CartGetEndpoint.CartResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();
            var cartItems = await db.CartItems.Where(c => c.AppUserId == userId).ToListAsync(cancellationToken);

            if (cartItems.Count > 0)
            {
                db.CartItemsAll.RemoveRange(cartItems);
                await db.SaveChangesAsync(cancellationToken);
            }

            return Ok(new CartGetEndpoint.CartResponse());
        }
    }
}
