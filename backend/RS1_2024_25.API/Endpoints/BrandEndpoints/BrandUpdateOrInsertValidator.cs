using FluentValidation;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.ColorEndpoints;

namespace RS1_2024_25.API.Endpoints.BrandEndpoints;
public class BrandUpdateOrInsertValidator : AbstractValidator<BrandUpdateOrInsertEndpoint.BrandUpdateOrInsertRequest>
{
    public BrandUpdateOrInsertValidator(ApplicationDbContext dbContext)
    {

       

        // Validation for name
        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Brand name has to have 3 or more characters");



    }
}
