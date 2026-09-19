using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("cart/items")]
    public class CartAddOrUpdateEndpoint(
        ApplicationDbContext db,
        IValidator<CartAddOrUpdateEndpoint.CartAddOrUpdateRequest> validator) : MyEndpointBaseAsync
        .WithRequest<CartAddOrUpdateEndpoint.CartAddOrUpdateRequest>
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<CartGetEndpoint.CartResponse>> HandleAsync(
            [FromBody] CartAddOrUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }

            var userId = db.GetUserIdThrow();
            var productSize = await db.ProductSizes
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.ID == request.ProductSizeId, cancellationToken);

            if (productSize == null)
            {
                return NotFound("Product size not found");
            }

            var existing = await db.CartItems
                .FirstOrDefaultAsync(c => c.AppUserId == userId && c.ProductSizeId == request.ProductSizeId, cancellationToken);

            var newQuantity = existing == null
                ? request.Quantity
                : existing.Quantity + request.Quantity;

            if (newQuantity > productSize.Stock)
            {
                return BadRequest($"Not enough stock. Available: {productSize.Stock}");
            }

            if (existing == null)
            {
                db.CartItemsAll.Add(new CartItem
                {
                    AppUserId = userId,
                    ProductSizeId = request.ProductSizeId,
                    Quantity = request.Quantity
                });
            }
            else
            {
                existing.Quantity = newQuantity;
            }

            await db.SaveChangesAsync(cancellationToken);

            var items = await CartQueryHelper.GetCartItemsAsync(db, userId, cancellationToken);
            return Ok(CartQueryHelper.ToCartResponse(items));
        }

        public class CartAddOrUpdateRequest
        {
            public int ProductSizeId { get; set; }
            public int Quantity { get; set; } = 1;
        }
    }

    public class CartAddOrUpdateValidator : AbstractValidator<CartAddOrUpdateEndpoint.CartAddOrUpdateRequest>
    {
        public CartAddOrUpdateValidator()
        {
            RuleFor(x => x.ProductSizeId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }
}
