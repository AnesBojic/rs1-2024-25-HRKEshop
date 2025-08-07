using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Endpoints.ProductRatingEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Endpoints.TProductRatingEndpoint
{
    public class ProductRatingAddOrUpdateTests : EndpointTestBase
    {
        private readonly ProductRatingAddOrUpdateEndpoint _endpoint;
        private readonly ProductRatingAddOrUpdateValidator _validator;

        public ProductRatingAddOrUpdateTests()
        {
            _validator = new ProductRatingAddOrUpdateValidator();
            _endpoint = new ProductRatingAddOrUpdateEndpoint(_db, _validator);
            _endpoint.ControllerContext = new ControllerContext { HttpContext = _httpContext };

        }



        [Fact]
        public async Task HandleAsync_ShouldReturnNotfound_WhenProductIdIsInvalid()
        {

            //we will take big number

            var request = new ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest
            {
                ProductId = 5000,
                Rating = 5,


            };

            var response = await _endpoint.HandleAsync(request);


            var notFoundObjRes = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal("Product is not found", notFoundObjRes.Value);
        }
        [Fact]
        public async Task HandleAsync_ShouldFirstAddNewOne_ThenUpdate_WhenRequestIsValid()
        {
            var product = await _db.Products.FirstOrDefaultAsync();

            var requestForAdd = new ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest
            {
                ProductId = product.ID,
                Rating = 5,
                Comment = "Comment"

            };


            var response = await _endpoint.HandleAsync(requestForAdd);

            var OkObj = Assert.IsType<OkObjectResult>(response.Result);


            var responseFine = Assert.IsType<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateResponse>(OkObj.Value);

            Assert.Equal("Rating added succesfully", responseFine.Message);


            requestForAdd.Rating = 4;

            response = await _endpoint.HandleAsync(requestForAdd);

             OkObj = Assert.IsType<OkObjectResult>(response.Result);


             responseFine = Assert.IsType<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateResponse>(OkObj.Value);

            Assert.Equal("Rating updated succesfully", responseFine.Message);

           


        }






    }
}
