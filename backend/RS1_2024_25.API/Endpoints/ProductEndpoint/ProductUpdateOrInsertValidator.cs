using FluentValidation;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.StudentEndpoints;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;
public class ProductUpdateOrInsertValidator : AbstractValidator<ProductUpdateOrInsertEndpoint.ProductUpdateOrInsertRequest>
{
    public ProductUpdateOrInsertValidator(ApplicationDbContext dbContext)
    {


        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(1);


        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender has to be valid (Male, Female, Other).");

        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Name of the product has to have at least 3 characters.");

        //RuleFor(x => x.TenantId)
        //    .NotNull().WithMessage("TenantId is required");



    }
}
