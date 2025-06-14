using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;


namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    public class AppUserAddValidator : AbstractValidator<AppUserAddEndpoint.AppUserAddRequest>
    {
        public AppUserAddValidator(ApplicationDbContext dbContext)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name must be at most 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be valid")
                .MustAsync(async (request, email,cancellationToken) =>
                {
                    return !await dbContext.AppUsers.AnyAsync(u => u.Email == email,cancellationToken);

                }).WithMessage("A user with this email already exists.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");



        }


    }
}
