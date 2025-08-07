using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;
using static RS1_2024_25.API.Endpoints.AppUserEndpoints.AppUserUpdateEndpoint;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    [Authorize]
    [Route("appusers/profile")]
    public class AppUserUpdateEndpoint(ApplicationDbContext db,IValidator<AppUserUpdateRequest> validator,IAuthContext authContext) : MyEndpointBaseAsync
        .WithRequest<AppUserUpdateRequest>
        .WithActionResult
    {
        [HttpPatch]
        public override async Task<ActionResult> HandleAsync([FromBody] AppUserUpdateRequest request, CancellationToken cancellationToken = default)
        {
            

            var user = await db.AppUsers.FirstOrDefaultAsync(x=> x.ID == request.ID,cancellationToken );

            if(user == null)
            {
                return NotFound("User not found");
            }

            if(user.ID != authContext.AppUserId)
            {
                return Forbid();
            }

            var validationProblem = await FluentValidationHelper.TryValidateAsync(validator,request,cancellationToken);
            if(validationProblem != null)
            {
                return validationProblem;
            }



            user.Name = request.Name ?? user.Name;
            user.Surname = request.Surname ?? user.Surname;
            user.Email = request.Email ?? user.Email;
            user.Phone = request.Phone ?? user.Phone;
            user.Address = request.Address  ?? user.Address;
            user.City = request.City ?? user.City;
            user.ZipCode = request.ZipCode ?? user.ZipCode;
            
            await db.SaveChangesAsync(cancellationToken);

            return NoContent();

        }



        public class AppUserUpdateRequest
        {
            public required int ID { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }

            public string? Email { get; set; }

            public string? Phone { get; set; }

            public string? Address { get; set; }

            public string? City { get; set; }

            public string? ZipCode { get; set; }

            

        }
    }


    
}
