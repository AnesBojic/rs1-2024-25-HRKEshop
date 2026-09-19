using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("cart/items/{productSizeId:int}")]
    public class CartRemoveItemEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpDelete]
        public override async Task<ActionResult<CartGetEndpoint.CartResponse>> HandleAsync(
            [FromRoute] int productSizeId,
            CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();
            var cartItem = await db.CartItems
                .FirstOrDefaultAsync(c => c.AppUserId == userId && c.ProductSizeId == productSizeId, cancellationToken);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            db.CartItemsAll.Remove(cartItem);
            await db.SaveChangesAsync(cancellationToken);

            var items = await CartQueryHelper.GetCartItemsAsync(db, userId, cancellationToken);
            return Ok(CartQueryHelper.ToCartResponse(items));
        }
    }
}
