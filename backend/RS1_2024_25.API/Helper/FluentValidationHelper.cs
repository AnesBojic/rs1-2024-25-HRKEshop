using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace RS1_2024_25.API.Helper
{
    public static class FluentValidationHelper
    {
        public static async Task<ActionResult?> TryValidateAsync<T>(IValidator<T> validator,T request, CancellationToken cancellationToken= default)
        {
            var result  = await validator.ValidateAsync(request, cancellationToken);
            if(!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary
                    (
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return new BadRequestObjectResult(errors);
            }

            return null;
        }


    }
}
