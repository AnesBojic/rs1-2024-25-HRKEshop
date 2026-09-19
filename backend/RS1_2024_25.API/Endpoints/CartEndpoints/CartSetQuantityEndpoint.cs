using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("cart/items/set-quantity")]
    public class CartSetQuantityEndpoint(
        ApplicationDbContext db,
        IValidator<CartSetQuantityEndpoint.CartSetQuantityRequest> validator) : MyEndpointBaseAsync
        .WithRequest<CartSetQuantityEndpoint.CartSetQuantityRequest>
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpPut]
        public override async Task<ActionResult<CartGetEndpoint.CartResponse>> HandleAsync(
            [FromBody] CartSetQuantityRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }

            var userId = db.GetUserIdThrow();
            var cartItem = await db.CartItems
                .Include(c => c.ProductSize)
                .FirstOrDefaultAsync(c => c.AppUserId == userId && c.ProductSizeId == request.ProductSizeId, cancellationToken);

            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            if (request.Quantity > cartItem.ProductSize.Stock)
            {
                return BadRequest($"Not enough stock. Available: {cartItem.ProductSize.Stock}");
            }

            if (request.Quantity <= 0)
            {
                db.CartItemsAll.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
            }

            await db.SaveChangesAsync(cancellationToken);

            var items = await CartQueryHelper.GetCartItemsAsync(db, userId, cancellationToken);
            return Ok(CartQueryHelper.ToCartResponse(items));
        }

        public class CartSetQuantityRequest
        {
            public int ProductSizeId { get; set; }
            public int Quantity { get; set; }
        }
    }

    public class CartSetQuantityValidator : AbstractValidator<CartSetQuantityEndpoint.CartSetQuantityRequest>
    {
        public CartSetQuantityValidator()
        {
            RuleFor(x => x.ProductSizeId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        }
    }
}
