using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer")]
    [Route("cart/checkout")]
    public class CartCheckoutEndpoint(
        ApplicationDbContext db,
        IValidator<CartCheckoutEndpoint.CartCheckoutRequest> validator) : MyEndpointBaseAsync
        .WithRequest<CartCheckoutEndpoint.CartCheckoutRequest>
        .WithActionResult<CartCheckoutEndpoint.CartCheckoutResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<CartCheckoutResponse>> HandleAsync(
            [FromBody] CartCheckoutRequest request,
            CancellationToken cancellationToken = default)
        {
            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }

            var userId = db.GetUserIdThrow();

            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var cartItems = await db.CartItems
                    .Include(c => c.ProductSize)
                        .ThenInclude(ps => ps.Product)
                    .Where(c => c.AppUserId == userId)
                    .ToListAsync(cancellationToken);

                if (cartItems.Count == 0)
                {
                    return BadRequest("Cart is empty");
                }

                var order = new Order
                {
                    UserId = userId,
                    Address = request.ShippingAddress.Trim(),
                    OrderStatus = OrderStatus.Pending,
                    IsPaid = false,
                    Items = new List<OrderItem>()
                };

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Quantity > cartItem.ProductSize.Stock)
                    {
                        return BadRequest($"Not enough stock for {cartItem.ProductSize.Product.Name}");
                    }

                    var unitPrice = cartItem.ProductSize.Price ?? (decimal)cartItem.ProductSize.Product.Price;

                    order.Items.Add(new OrderItem
                    {
                        ProductSizeId = cartItem.ProductSizeId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = unitPrice
                    });

                    cartItem.ProductSize.Stock -= cartItem.Quantity;
                }

                order.TotalAmount = order.Items.Sum(i => i.TotalPrice);

                db.OrdersAll.Add(order);
                db.CartItemsAll.RemoveRange(cartItems);

                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Ok(new CartCheckoutResponse
                {
                    OrderId = order.ID,
                    TotalAmount = order.TotalAmount,
                    Message = $"Order created successfully. Total: {order.TotalAmount} BAM"
                });
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return StatusCode(500, "An error occurred while processing your order.");
            }
        }

        public class CartCheckoutRequest
        {
            public string ShippingAddress { get; set; } = string.Empty;
        }

        public class CartCheckoutResponse
        {
            public int OrderId { get; set; }
            public decimal TotalAmount { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }

    public class CartCheckoutValidator : AbstractValidator<CartCheckoutEndpoint.CartCheckoutRequest>
    {
        public CartCheckoutValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address cannot be empty")
                .MaximumLength(250).WithMessage("Maximum 250 characters");
        }
    }
}
