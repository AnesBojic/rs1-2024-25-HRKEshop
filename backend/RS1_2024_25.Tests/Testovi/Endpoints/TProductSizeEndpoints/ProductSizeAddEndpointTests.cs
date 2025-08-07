using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.OrderEndpoint;
using RS1_2024_25.API.Endpoints.ProductSizeEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductSizeEndpoints
{
    public class ProductSizeAddEndpointTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductSizeAddEndpoint _endpoint;
        private readonly ProductSizeAddValidator _validator;



        public ProductSizeAddEndpointTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(role: "Manager");

            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();

            _validator = new ProductSizeAddValidator(_db);

            _endpoint = new ProductSizeAddEndpoint(_db, _validator);


            _endpoint.ControllerContext = new ControllerContext
            {
                HttpContext = accessor.HttpContext
            };
        }




        [Fact]
        public async Task Should_Return_OKObject_WithProductSizeAddResponse()
        {
            //looking for seeded products and size for valid request
            //also we will try for fallback of the price if its not provided

            var productPrice = await _db.Products.Select(p => new {p.ID,p.Price }).FirstAsync();
            var sizeId = await _db.Sizes.Select(s => s.ID).FirstAsync();


            var request = new ProductSizeAddEndpoint.ProductSizeAddRequest
            {
                ProductId = productPrice.ID,
                SizeId = sizeId,
                Stock = 5,


            };


            var response = await _endpoint.HandleAsync(request);

            var OkResponse = Assert.IsType<OkObjectResult>(response.Result);

            var ProductSizeResponse = Assert.IsType<ProductSizeAddEndpoint.ProductSizeAddResponse>(OkResponse.Value);

            Assert.NotNull(ProductSizeResponse);
            Assert.Equal("Product size for given product is succesfully created", ProductSizeResponse.Message);

            var productSizeLastAdded = await _db.ProductSizes.OrderByDescending(ps => ps.CreatedAt).FirstAsync();


            Assert.Equal((decimal)productPrice.Price, productSizeLastAdded.Price);
        }






    }
}
