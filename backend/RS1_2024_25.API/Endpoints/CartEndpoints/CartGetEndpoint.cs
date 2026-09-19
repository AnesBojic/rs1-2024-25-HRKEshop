using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.CartEndpoints
{
    [Authorize(Roles = "Customer,Manager,Admin")]
    [Route("cart")]
    public class CartGetEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<CartGetEndpoint.CartResponse>
    {
        [HttpGet]
        public override async Task<ActionResult<CartResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userId = db.GetUserIdThrow();
            var items = await CartQueryHelper.GetCartItemsAsync(db, userId, cancellationToken);
            return Ok(CartQueryHelper.ToCartResponse(items));
        }

        public class CartResponse
        {
            public List<CartItemDto> Items { get; set; } = new();
            public int TotalQuantity { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public class CartItemDto
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public int ProductSizeId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string SizeName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineTotal { get; set; }
            public int Stock { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}
