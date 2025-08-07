using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using System.Collections.Specialized;

namespace RS1_2024_25.API.Endpoints.OrderEndpoint
{
    public class OrderAddValidator : AbstractValidator<OrderAddEndpoint.OrderAddRequest>
    {
        public OrderAddValidator(ApplicationDbContext db)
        {

            RuleFor(x => x.ShippingAddress).NotEmpty().WithMessage("Shipping address cannot be empty")
                .MaximumLength(250).WithMessage("Maximum 250 characters");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item")
                .MustAsync(async(items,cancellation)=>
                {
                    var productSizeIds =items.Select(i=> i.ProductSizeId).ToList();
                    var countInDb = await db.ProductSizes.CountAsync(ps => productSizeIds.Contains(ps.ID), cancellation);
                    return countInDb == productSizeIds.Count;

                }).WithMessage("One ore more ProductSizeIds do not exist")
                .Must(items => items.Select(i => i.ProductSizeId).Distinct().Count() == items.Count).WithMessage("Duplicated ProductSizeIds are not allowed");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductSizeId).GreaterThan(0).WithMessage("ProductSizeId must be valid");
                items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be at least one per item.");
                items.RuleFor(i => i.Quantity).LessThanOrEqualTo(100).WithMessage("Quantity per item must not exceed 100.");
            });
        }


    }
    
   
}
