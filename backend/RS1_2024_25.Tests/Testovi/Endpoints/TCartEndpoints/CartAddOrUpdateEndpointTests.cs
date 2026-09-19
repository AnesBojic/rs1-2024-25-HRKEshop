using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.CartEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TCartEndpoints
{
    public class CartAddOrUpdateEndpointTests : EndpointTestBase
    {
        private readonly CartAddOrUpdateEndpoint _endpoint;

        public CartAddOrUpdateEndpointTests() : base("Customer")
        {
            var validator = new CartAddOrUpdateValidator();
            _endpoint = new CartAddOrUpdateEndpoint(_db, validator);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };
        }

        [Fact]
        public async Task HandleAsync_AddsItemToCart_WhenValidRequest()
        {
            var productSize = await _db.ProductSizes.FirstAsync(ps => ps.Stock > 0);

            var response = await _endpoint.HandleAsync(new CartAddOrUpdateEndpoint.CartAddOrUpdateRequest
            {
                ProductSizeId = productSize.ID,
                Quantity = 1
            });

            var ok = Assert.IsType<OkObjectResult>(response.Result);
            var cart = Assert.IsType<CartGetEndpoint.CartResponse>(ok.Value);

            Assert.Equal(1, cart.TotalQuantity);
            Assert.Contains(cart.Items, i => i.ProductSizeId == productSize.ID);
            Assert.True(await _db.CartItems.AnyAsync(c => c.ProductSizeId == productSize.ID));
        }

        [Fact]
        public async Task HandleAsync_IncrementsQuantity_WhenItemAlreadyInCart()
        {
            var productSize = await _db.ProductSizes.FirstAsync(ps => ps.Stock > 2);
            var userId = _db.GetUserIdThrow();

            _db.CartItemsAll.Add(new CartItem
            {
                AppUserId = userId,
                ProductSizeId = productSize.ID,
                Quantity = 1
            });
            await _db.SaveChangesAsync();

            var response = await _endpoint.HandleAsync(new CartAddOrUpdateEndpoint.CartAddOrUpdateRequest
            {
                ProductSizeId = productSize.ID,
                Quantity = 2
            });

            var ok = Assert.IsType<OkObjectResult>(response.Result);
            var cart = Assert.IsType<CartGetEndpoint.CartResponse>(ok.Value);
            var item = Assert.Single(cart.Items);

            Assert.Equal(3, item.Quantity);
        }

        [Fact]
        public async Task HandleAsync_ReturnsBadRequest_WhenQuantityExceedsStock()
        {
            var productSize = await _db.ProductSizes.FirstAsync(ps => ps.Stock >= 0);

            var response = await _endpoint.HandleAsync(new CartAddOrUpdateEndpoint.CartAddOrUpdateRequest
            {
                ProductSizeId = productSize.ID,
                Quantity = productSize.Stock + 5
            });

            Assert.IsType<BadRequestObjectResult>(response.Result);
        }
    }
}
