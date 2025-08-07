using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.ProductSizeEndpoint
{
    public class ProductSizeAddValidator : AbstractValidator<ProductSizeAddEndpoint.ProductSizeAddRequest>
    {
        public ProductSizeAddValidator(ApplicationDbContext db)
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .MustAsync(async (id, cancellation) =>
                
                    await db.Products.AnyAsync(p => p.ID == id, cancellation)
                ).WithMessage("Product does not exist");

            RuleFor(x => x.SizeId)
                .GreaterThan(0)
                .MustAsync(async (id, cancellation) =>

                await db.Sizes.AnyAsync(s => s.ID == id, cancellation)

                ).WithMessage("Size does not exist.");


            RuleFor(x => x.Price)
                .GreaterThan(0)
                .When(x=> x.Price.HasValue)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock must be 0 or more.");
            
            
        }

    }
}
