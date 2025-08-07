using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.OrderEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.OrderEndpoints
{
    public class OrderAddEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderAddEndpoint _endpoint;
        private readonly OrderAddValidator _validator;

        public OrderAddEndpointTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _validator = new OrderAddValidator(_db);

            _endpoint = new OrderAddEndpoint(_db,_validator);

            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser();

            _endpoint.ControllerContext = new ControllerContext
            {

                HttpContext = accessor.HttpContext
            };
        }

        [Fact]

        public async Task HandleAsync_ShouldAddNewOrder_WhenRequestIsValid()
        {
            

            var request = new OrderAddEndpoint.OrderAddRequest
            {
                ShippingAddress = "Existing Address",
                Items =
                {
                    new OrderAddEndpoint.OrderAddRequest.Item{ProductSizeId = 1, Quantity =1},
                    new OrderAddEndpoint.OrderAddRequest.Item{ProductSizeId=2,Quantity=2}
                }


            };

            var response = await _endpoint.HandleAsync(request);

            var lastOrderInDb = await _db.Orders.OrderByDescending(o => o.ID).FirstOrDefaultAsync();


            Assert.NotNull(response);
            Assert.Equal(lastOrderInDb.ID, response.Value.ID);
            Assert.Equal($"Order created successfully! Order toal: {lastOrderInDb.TotalAmount}$", response.Value.Message);
        }
        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequestObjet_WhenQuantityIsOverStock()
        {
            //First will add productSize with quantity because baseseed is random

            var newProductSize = new ProductSize
            {
                SizeId = 1,
                Price = 1,
                ProductId = 1,
                Stock = 80,

            };
            //Tenanat is set with jwtAutenthicatedUser
            _db.ProductsSizesAll.Add(newProductSize);
            await _db.SaveChangesAsync();



            var request = new OrderAddEndpoint.OrderAddRequest
            {
                ShippingAddress = "Exsiting address",
                Items =
                {
                    new OrderAddEndpoint.OrderAddRequest.Item {ProductSizeId=newProductSize.ID,Quantity =82}
                }


            };




            var response = await _endpoint.HandleAsync(request);

            var BadRequest = Assert.IsType<BadRequestObjectResult>(response.Result);

            Assert.NotNull(BadRequest);

            var message = BadRequest.Value?.ToString();

            Assert.Contains("Not enough stock for ProductSize ID", message);




        }






    }
}
