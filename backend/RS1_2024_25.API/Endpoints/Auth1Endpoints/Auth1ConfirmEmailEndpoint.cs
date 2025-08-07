using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{
    [Route("auth1/confirm-email")]
    public class Auth1ConfirmEmailEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<Auth1ConfirmEmailEndpoint.ConfirmEmailRequest>
        .WithActionResult<string>
    {
        [HttpPost]
        public override async Task<ActionResult<string>> HandleAsync([FromBody]ConfirmEmailRequest request, CancellationToken cancellationToken = default)
        {
            var token = await db.EmailVerificationTokensAll.FirstOrDefaultAsync(x => x.Token == request.Token);

            if (token == null || token.ExpiresAtUtc < DateTime.UtcNow || token.Used)
                return BadRequest("Invalid or expired token");

            var user = await db.AppUsersAll.FirstOrDefaultAsync(x => x.ID == token.UserId);

            if (user == null) return BadRequest("User not found");

            user.EmailVerifiedAt = DateTime.UtcNow;

            token.Used = true;


            await db.SaveChangesAsync(cancellationToken);

            return Ok(new {message= "Email verified succesfully."});


        }








        public class ConfirmEmailRequest
        {
            public string Token { get; set; }
        }
    }
}
