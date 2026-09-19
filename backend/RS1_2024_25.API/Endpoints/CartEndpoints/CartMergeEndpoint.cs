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
    [Route("cart/merge")]
    public class CartMergeEndpoint(
        ApplicationDbContext db,
        IValidator<CartMergeEndpoint.CartMergeRequest> validator) : MyEndpointBaseAsync
        .WithRequest<CartMergeEndpoint.CartMergeRequest>
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<CartGetEndpoint.CartResponse>> HandleAsync(
            [FromBody] CartMergeRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }

            var userId = db.GetUserIdThrow();
            var incoming = request.Items
                .GroupBy(i => i.ProductSizeId)
                .Select(g => new { ProductSizeId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var productSizeIds = incoming.Select(i => i.ProductSizeId).ToList();
            var productSizes = await db.ProductSizes
                .Where(ps => productSizeIds.Contains(ps.ID))
                .ToDictionaryAsync(ps => ps.ID, cancellationToken);

            var existingItems = await db.CartItems
                .Where(c => c.AppUserId == userId)
                .ToDictionaryAsync(c => c.ProductSizeId, cancellationToken);

            foreach (var item in incoming)
            {
                if (!productSizes.TryGetValue(item.ProductSizeId, out var productSize))
                {
                    continue;
                }

                if (existingItems.TryGetValue(item.ProductSizeId, out var existing))
                {
                    var mergedQty = Math.Min(existing.Quantity + item.Quantity, Math.Max(productSize.Stock, 0));
                    if (mergedQty <= 0)
                    {
                        db.CartItemsAll.Remove(existing);
                    }
                    else
                    {
                        existing.Quantity = mergedQty;
                    }
                }
                else if (item.Quantity > 0 && productSize.Stock > 0)
                {
                    db.CartItemsAll.Add(new CartItem
                    {
                        AppUserId = userId,
                        ProductSizeId = item.ProductSizeId,
                        Quantity = Math.Min(item.Quantity, productSize.Stock)
                    });
                }
            }

            await db.SaveChangesAsync(cancellationToken);

            var items = await CartQueryHelper.GetCartItemsAsync(db, userId, cancellationToken);
            return Ok(CartQueryHelper.ToCartResponse(items));
        }

        public class CartMergeRequest
        {
            public List<MergeItem> Items { get; set; } = new();

            public class MergeItem
            {
                public int ProductSizeId { get; set; }
                public int Quantity { get; set; }
            }
        }
    }

    public class CartMergeValidator : AbstractValidator<CartMergeEndpoint.CartMergeRequest>
    {
        public CartMergeValidator()
        {
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductSizeId).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(100);
            });
        }
    }
}
