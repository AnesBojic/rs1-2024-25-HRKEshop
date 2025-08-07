using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Endpoints.Payloads;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{
    [Route("auth1/reset-password")]
    public class Auth1ResetPasswordEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<Auth1ResetPasswordEndpoint.ResetPasswordRequest>
        .WithActionResult<Auth1ResetPasswordEndpoint.ResetPasswordResponse>
    {
        [HttpPost]
        public async override Task<ActionResult<ResetPasswordResponse>> HandleAsync([FromBody]ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            
            if(string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Invalid data.");
            }
            

            var user = await db.AppUsersAll.FirstOrDefaultAsync(au => au.PasswordResetToken == request.Token &&
            au.PasswordResetTokenCreatedAt != null &&
            au.PasswordResetTokenCreatedAt.Value.AddMinutes(30) > DateTime.UtcNow);

            if(user == null)
            {
                return BadRequest("Invalid or expired token");
            }

            user.SetPassword(request.NewPassword);

            user.PasswordResetToken = null;
            user.PasswordResetTokenCreatedAt = null;

            await db.SaveChangesAsync(cancellationToken);

            return Ok(new ResetPasswordResponse
            {
                ID = user.ID,
                Message = "Password changed succesfully"
            });








        }








        public class ResetPasswordRequest
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }


        }
        public class ResetPasswordResponse : BaseResponse
        {

        }


    }
}
