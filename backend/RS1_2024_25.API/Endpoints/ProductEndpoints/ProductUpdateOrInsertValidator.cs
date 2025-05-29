using FluentValidation;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.StudentEndpoints;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;
public class ProductUpdateOrInsertValidator : AbstractValidator<ProductUpdateOrInsertEndpoint.ProductUpdateOrInsertRequest>
{
    public ProductUpdateOrInsertValidator(ApplicationDbContext dbContext)
    {

        // Validacija StudentNumber
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(1);


        // Validacija Gender
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Pol mora biti validan (Male, Female, Other).");


   
    }
}
