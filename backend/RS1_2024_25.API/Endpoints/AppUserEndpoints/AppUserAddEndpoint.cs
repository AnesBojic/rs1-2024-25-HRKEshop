using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;
using static RS1_2024_25.API.Endpoints.AppUserEndpoints.AppUserAddEndpoint;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    
    [Route("appusers/add")]
    public class AppUserAddEndpoint(ApplicationDbContext db,IEmailService emailService,IValidator<AppUserAddRequest> validator) : MyEndpointBaseAsync
        .WithRequest<AppUserAddRequest>
        .WithActionResult<AppUserAddResponse>
    {
        
        [HttpPost]
        public override async Task<ActionResult<AppUserAddResponse>> HandleAsync([FromBody] AppUserAddRequest request, CancellationToken cancellationToken = default)
        {

            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator, request, cancellationToken);
            if (validationProblem != null)
            {
                return validationProblem;
            }

            var newUser = new AppUser
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Phone = request.Phone,
                RoleID = 3,
                EmailVerifiedAt = null,
                
                


            };
            newUser.SetPassword(request.Password);
            db.AppUsersAll.Add(newUser);

            await db.SaveChangesAsync();

            await emailService.SendEmailVerificationAsync(newUser,cancellationToken);
            

            return new AppUserAddResponse
            {
                ID = newUser.ID,
                Message = "User created-- :D"
            };


        }

        public class AppUserAddRequest
        {
            public required string Name { get; set; }
            public required string Surname { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }

            public string? Phone { get; set; }


        }
        public class AppUserAddResponse
        {
            public int ID { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }

    
}
