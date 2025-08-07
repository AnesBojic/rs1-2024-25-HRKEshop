using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.ProductSizeEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Validators.ProductSizeValidatorTests
{
    public class ProductSizeAddValidatorTests
    {
        private readonly ApplicationDbContext _db;
        private readonly ProductSizeAddValidator _validator;


        public ProductSizeAddValidatorTests()
        {
            var accessor = JwtTestHttpContextAccessorHelper.CreateWithJwtAuthenticatedUser(role:"Manager");


            _db = TestApplication1DbContext.CreateAsync(accessor).GetAwaiter().GetResult();

            _validator = new ProductSizeAddValidator(_db);
        }


        [Fact]

        public async Task Should_Not_Have_Any_Erros_With_Valid_Request()
        {
            //WE use data from seeded base

            var productId = await _db.Products.Select(p => p.ID).FirstAsync();
            var sizeId = await _db.Sizes.Select(s => s.ID).FirstAsync();





            var request = new ProductSizeAddEndpoint.ProductSizeAddRequest
            {
                ProductId = productId,
                SizeId = sizeId,
                Price = 5,
                Stock = 2
            };

            var responseValidationProblems = await _validator.TestValidateAsync(request);

            responseValidationProblems.ShouldNotHaveAnyValidationErrors();

        }

        [Fact]

        public async Task Should_Have_Error_ProductId_Does_Not_Exist_Invalid_Request()
        {
            var sizeId = await _db.Sizes.Select(s => s.ID).FirstAsync();

            var request = new ProductSizeAddEndpoint.ProductSizeAddRequest
            {
                ProductId = 5000,
                SizeId = sizeId,
                Price = 5,
                Stock = 3
            };

            var responseValidationProblems = await _validator.TestValidateAsync(request);

            responseValidationProblems.ShouldHaveValidationErrorFor(e => e.ProductId).WithErrorMessage("Product does not exist");

        }
        [Fact]

        public async Task Shoul_Have_Error_SizeId_Does_Not_Exist_Invalid_Request()
        {
            var productId = await _db.Products.Select(p => p.ID).FirstAsync();
            var request = new ProductSizeAddEndpoint.ProductSizeAddRequest
            {
                ProductId = productId,
                SizeId = 5000,
                Price = 5,
                Stock = 3
            };

            var responseValidationProblems = await _validator.TestValidateAsync(request);

            responseValidationProblems.ShouldHaveValidationErrorFor(e => e.SizeId).WithErrorMessage("Size does not exist.");



        }

        [Fact]

        public async Task Should_Have_Errros_StockAndPrice_When_Invalid_Request()
        {
            var productId = await _db.Products.Select(p => p.ID).FirstAsync();
            var sizeId = await _db.Sizes.Select(s => s.ID).FirstAsync();

            var request = new ProductSizeAddEndpoint.ProductSizeAddRequest
            {
                ProductId = productId,
                SizeId = sizeId,
                Price = -2,
                Stock = -2,

            };

            var responseValidationProblems = await _validator.TestValidateAsync(request);

            responseValidationProblems.ShouldHaveValidationErrorFor(e => e.Price).WithErrorMessage("Price must be greater than 0.");
            responseValidationProblems.ShouldHaveValidationErrorFor(e => e.Stock).WithErrorMessage("Stock must be 0 or more.");





        }


    }
}
