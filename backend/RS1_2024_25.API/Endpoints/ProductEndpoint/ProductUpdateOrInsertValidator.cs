using FluentValidation;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.StudentEndpoints;

namespace RS1_2024_25.API.Endpoints.ProductEndpoints;
public class ProductUpdateOrInsertValidator : AbstractValidator<ProductUpdateOrInsertEndpoint.ProductUpdateOrInsertRequest>
{
    public ProductUpdateOrInsertValidator(ApplicationDbContext dbContext)
    {


        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(5).WithMessage("Product has to be more expensive than 5 BAM");


        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender has to be valid (Male, Female, Other).");

        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Name of the product has to have at least 3 characters.");

        RuleFor(x => x)
            .Must(x => (x.CategoryId.HasValue && x.CategoryId > 0) || !string.IsNullOrWhiteSpace(x.NewCategoryName))
            .WithMessage("Select an existing category or enter a new category name.");

        RuleFor(x => x.NewCategoryName)
            .MinimumLength(2).WithMessage("Category name has to have at least 2 characters.")
            .MaximumLength(80).WithMessage("Category name can have at most 80 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.NewCategoryName));

        //RuleFor(x => x.TenantId)
        //    .NotNull().WithMessage("TenantId is required");



    }
}
