using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.Features;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.OrderEndpoint;
using RS1_2024_25.Tests.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RS1_2024_25.API.Endpoints.OrderEndpoint.OrderAddEndpoint.OrderAddRequest;

namespace RS1_2024_25.Tests.Testovi.Validators.OrderValidator
{
    public class OrderAddValidatorTests
    {
        private readonly ApplicationDbContext _db;
        private readonly OrderAddValidator _validator;

        public OrderAddValidatorTests()
        {
            _db = TestApplication1DbContext.CreateAsync().GetAwaiter().GetResult();

            _validator = new OrderAddValidator(_db);
        }


        [Fact]

        public async Task Should_Have_An_Error_When_ShippingAddressIsEmpty()
        {

            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items =
                {
                    new Item { ProductSizeId = 1, Quantity = 1}

                },
                ShippingAddress = ""



            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(e => e.ShippingAddress);
            result.ShouldHaveValidationErrorFor(e => e.ShippingAddress).WithErrorMessage("Shipping address cannot be empty");

        }
        [Fact]
        public async Task Should_Have_An_Error_When_ItemsAreEmpty()
        {
            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items = { },
                ShippingAddress = "Postojeca adresa"


            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(e => e.Items).WithErrorMessage("Order must contain at least one item");
        }
        [Fact]
        public async Task Should_Have_An_Error_When_ProductSizeId_Does_Not_Exist()
        {
            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items = { new Item { ProductSizeId = -5, Quantity = 1 } },
                ShippingAddress = "pOSTOJECA ADRESA"


            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(e => e.Items).WithErrorMessage("One ore more ProductSizeIds do not exist");
        }
        [Fact]

        public async Task Should_Have_An_Error_When_DuplicatedProductSizeId()
        {

            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items =
                {
                    new Item { ProductSizeId = 2, Quantity = 1 },
                    new Item { ProductSizeId = 2, Quantity = 1 }
                },
                ShippingAddress = "Existing address"

            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor(e => e.Items).WithErrorMessage("Duplicated ProductSizeIds are not allowed");

        }

        [Fact]
        public async Task Should_Have_An_Error_InvalidQuantity()
        {
            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items =
                {
                    new Item { ProductSizeId = 2,Quantity =-2 }
                },
                ShippingAddress = "Existing Address"
            };

            var result = await _validator.TestValidateAsync(request);
            result.ShouldHaveValidationErrorFor("Items[0].Quantity").WithErrorMessage("Quantity must be at least one per item.");

        }
        [Fact]
        public async Task Should_Not_Have_Any_Erros_Valid_Request()
        {
            var request = new OrderAddEndpoint.OrderAddRequest
            {
                Items =
                {
                    new Item{ProductSizeId=2,Quantity=1},
                    new Item{ProductSizeId=1,Quantity=1}
                },
                ShippingAddress = "Existing Adress"
               
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldNotHaveAnyValidationErrors();




        }
    }
}
