using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Data.SharedEnums;
using RS1_2024_25.API.Endpoints.OrderEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static RS1_2024_25.API.Endpoints.OrderEndpoint.OrderGetByIdEndpoint;

namespace RS1_2024_25.Tests.Testovi.Endpoints.OrderEndpoints
{
    public class OrderGetByIdEndpoinTests
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderGetByIdEndpoint _endpoint;

        public OrderGetByIdEndpoinTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();
            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();


            _endpoint = new OrderGetByIdEndpoint(_db);

            



            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext
            };
        }


        [Fact]

        public async Task HandleAsync_ShouldThrowNotFound_WhenOrderIdIsInvalid()
        {
            var requestId = 50000;

            var response = await _endpoint.HandleAsync(requestId);


            var badRequestObjectResult = Assert.IsType<NotFoundObjectResult>(response.Result);


            Assert.Equal("Order with this ID does not exist", badRequestObjectResult.Value);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnOkResponse_WhenRequestIsValid()
        {
            //Adding new order

            var userId = _db.GetUserIdThrow();
            var currentTenantId = _db.CurrentTenantId;


            //User was never in db just httpcontext accesor mock object, so we need to add it in this case cause its needed as navigation property
            if (!await _db.AppUsers.AnyAsync(u => u.ID == userId))
            {
                var user = new AppUser
                {
                    ID = userId,
                    Name = "Test",
                    Surname = "User",
                    Email = "asim_asim@gmail.com",

                };
                user.SetPassword("test");

                _db.AppUsersAll.Add(user);


                await _db.SaveChangesAsync();
            }



            var order = new Order
            {
                UserId = userId,
                IsPaid = false,
                OrderStatus = OrderStatus.Pending,
                Address = "Legit address",

                Items = new List<OrderItem>
                {

                    new OrderItem
                    {
                        ProductSizeId =1,
                        Quantity = 1,



                    }
                },




            };


            _db.OrdersAll.Add(order);
            await _db.SaveChangesAsync();






            var requestId = order.ID;

            var response = await _endpoint.HandleAsync(requestId);

            var OKObjectResult = Assert.IsType<OkObjectResult>(response.Result);

            var getByIdResponse = Assert.IsType<OrderGetByIdEndpointResponse>(OKObjectResult.Value);


            Assert.Equal(order.ID, getByIdResponse.OrderId);




        }


    }
}
