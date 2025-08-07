using FluentValidation.TestHelper;
using RS1_2024_25.API.Endpoints.ProductRatingEndpoints;
using RS1_2024_25.Tests.Testovi.Endpoints.EndpointTestBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS1_2024_25.Tests.Testovi.Validators.TProductRatingValidator
{
    public class ProductRatingAddOrUpdateValidatorTests :EndpointTestBase
    {
        private readonly ProductRatingAddOrUpdateValidator _validator;


        public ProductRatingAddOrUpdateValidatorTests()
        {
            _validator = new ProductRatingAddOrUpdateValidator();
        }

        [Fact]

        public async Task HandleAsync_MixOfTests()
        {
            var request = new ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest
            {
                ProductId = 1,
                Comment = "valid",
                Rating = 2


            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldNotHaveAnyValidationErrors();

            //WrongId

            request.ProductId = -1;

            result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(e => e.ProductId).WithErrorMessage("Not valid productId");

            //

            request.Rating = -1;

            result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(e => e.Rating).WithErrorMessage("It must be between 1 and 5");







        }



    }
}
