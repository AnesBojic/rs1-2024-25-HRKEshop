using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    public class AppUserUpdateValidator : AbstractValidator<AppUserUpdateEndpoint.AppUserUpdateRequest>
    {

        public AppUserUpdateValidator(ApplicationDbContext dbContext)
        {

            RuleFor(x => x.Name)
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Surname)
                .MinimumLength(2).WithMessage("Surname must be at least 2 characters long")
                .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Surname));

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be valid!")
                .MustAsync(async (request, email, cancellationtoken) =>
                {

                    return !await dbContext.AppUsers.AnyAsync(u => u.Email == email && u.ID != request.ID);
                }).WithMessage("User with this email already exists")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Address)
                .MaximumLength(100).WithMessage("Address cant be longer than 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));
            RuleFor(x => x.City)
                .MaximumLength(50).WithMessage("City cant be longer than 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.City));
            RuleFor(x => x.ZipCode)
                .Matches(@"\d{4,10}$").WithMessage("ZipCode must be between 4 and 10 digits")
                .When(x => !string.IsNullOrWhiteSpace(x.ZipCode));
            RuleFor(x => x.RoleID)
                .MustAsync(async (request, roleId, cancellationtoken) => await dbContext.Roles.AnyAsync(r => r.ID == roleId, cancellationtoken))
                .WithMessage("RoleID must refer to an existing role")
                .When(x => x.RoleID.HasValue);






            
        }
    }
}
