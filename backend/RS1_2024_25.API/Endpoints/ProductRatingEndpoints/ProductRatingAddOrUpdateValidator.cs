using FluentValidation;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.ProductRatingEndpoints
{
    public class ProductRatingAddOrUpdateValidator : AbstractValidator<ProductRatingAddOrUpdateEndpoint.ProductRatingAddOrUpdateRequest>
    {

        public ProductRatingAddOrUpdateValidator()
        {
            RuleFor(x=> x.Rating).NotEmpty().WithMessage("Required rating").InclusiveBetween(1,5).WithMessage("It must be between 1 and 5");
            RuleFor(x => x.ProductId).NotEmpty().GreaterThan(0).WithMessage("Not valid productId");
            RuleFor(x => x.Comment).MaximumLength(250).WithMessage("Maximum number of characters 250");



        }
    }   
}
