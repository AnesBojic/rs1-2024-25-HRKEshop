using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Endpoints.OrderEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.OrderEndpoints
{
    public class OrderCancelEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderCancelEndpoint _endpoint;


        public OrderCancelEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _endpoint = new OrderCancelEndpoint(_db);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();


            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext


            };

        }


        [Fact]
        public async Task HandleAsync_ShouldCancelOrder_WhenRequestIsValid()
        {
            //first create new order for current user defaultly has status pending


            var userId = _db.GetUserIdThrow();

            var newOrder = new Order
            {
                UserId = userId,
                Address = "Existing address",
                OrderStatus = OrderStatus.Pending,
                IsPaid = false,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductSizeId = 1,
                        Quantity = 5,
                        UnitPrice = 5
                    }


                }
            };

            _db.OrdersAll.Add(newOrder);
            await _db.SaveChangesAsync();

            var request = newOrder.ID;

            var response = await _endpoint.HandleAsync(request);

            var objectOkResponse = Assert.IsType<OkObjectResult>(response.Result);

            var objectCancelEndpointResponse = Assert.IsType<OrderCancelEndpoint.OrderCancelResponse>(objectOkResponse.Value);


            Assert.Equal(newOrder.ID, objectCancelEndpointResponse.ID);
            Assert.Equal("Order is successfully cancelled.", objectCancelEndpointResponse.Message);

        }
        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_WhenThereIsNoOrder()
        {

            var requestIdNotExisting = -2;
            
            var response = await _endpoint.HandleAsync(requestIdNotExisting);

            Assert.NotNull(response);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal("Order not found",notFoundResult.Value);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenOrderStatusIsWrong()
        {
            var userId = _db.GetUserIdThrow();


            var order = new Order
            {
                Address = "Existing address",
                OrderStatus = OrderStatus.Processing,
                UserId = userId,
                IsPaid = false,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductSizeId =1,Quantity =1,UnitPrice = 1}

                }


            };

            _db.OrdersAll.Add(order);
            await _db.SaveChangesAsync();


            var request = await _endpoint.HandleAsync(order.ID);

            Assert.NotNull(request);

            var badRequestObject = Assert.IsType<BadRequestObjectResult>(request.Result);

            Assert.Equal("Only pending orders can be cancelled", badRequestObject.Value);

        }
        





    }
}
