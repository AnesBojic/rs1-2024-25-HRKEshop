using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{

    [Route("auth1/resend-email")]
    public class Auth1ResendEmailEndpoint(ApplicationDbContext db,IEmailService emailService) : MyEndpointBaseAsync
        .WithRequest<Auth1ResendEmailEndpoint.ResendEmailRequest>
        .WithActionResult<Auth1ResendEmailEndpoint.ResendEmailResponse>
    {
        [HttpPost]
        public override async Task<ActionResult<ResendEmailResponse>> HandleAsync([FromBody]ResendEmailRequest request, CancellationToken cancellationToken = default)
        {

            var user = await db.AppUsersAll.FirstOrDefaultAsync(au => au.Email == request.Email,cancellationToken);


            if (user == null)
            {
                return BadRequest("User does not exist.");
            }
            if (user.EmailVerifiedAt != null)
                return BadRequest("Email already verified.");

            await emailService.SendEmailVerificationAsync(user, cancellationToken);


            return Ok(new ResendEmailResponse { Message = "It has been send to your inbox." });

        }
        public class ResendEmailRequest
        {
            public string Email { get; set; }
        }
        public class ResendEmailResponse : BaseResponse
        {

        }
    }
}
