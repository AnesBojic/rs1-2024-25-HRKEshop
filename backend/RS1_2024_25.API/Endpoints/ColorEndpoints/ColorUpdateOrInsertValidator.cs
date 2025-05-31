using FluentValidation;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.ProductEndpoints;

namespace RS1_2024_25.API.Endpoints.ColorEndpoints;
public class ColorUpdateOrInsertValidator : AbstractValidator<ColorUpdateOrInsertEndpoint.ColorUpdateOrInsertRequest>
{
    public ColorUpdateOrInsertValidator(ApplicationDbContext dbContext)
    {

        // Validation for hex code
        RuleFor(x => x.Hex_Code)
            .Length(6).WithMessage("Hex Code has to have 6 characters");


        // Validation for name
        RuleFor(x => x.Name)
            .MinimumLength(3);



    }
}
