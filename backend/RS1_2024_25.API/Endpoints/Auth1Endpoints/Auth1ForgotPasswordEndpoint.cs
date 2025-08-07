using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{

    [Route("auth1/forgot-password")]
    public class Auth1ForgotPasswordEndpoint(AuthService authService, ApplicationDbContext db,IEmailService emailService,IConfiguration config) :
        MyEndpointBaseAsync
        .WithRequest<Auth1ForgotPasswordEndpoint.Auth1ForgotPasswordEndpointRequest>
        .WithActionResult
    {
        [HttpPost]
        public override async Task<ActionResult> HandleAsync(Auth1ForgotPasswordEndpointRequest request, CancellationToken cancellationToken = default)
        {
            var user = await db.AppUsersAll.FirstOrDefaultAsync(au => au.Email == request.Email, cancellationToken);


            if (user != null)
            {
                var token = authService.GeneratePasswordResetToken();
                user.PasswordResetToken = token;
                user.PasswordResetTokenCreatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync(cancellationToken);


                var resetLink = $"{config["FrontendBaseUrl"]}/auth/reset-password?token={Uri.EscapeDataString(token)}";




                await emailService.SendAsync(
                    user.Email,
                    "Password Reset Request",
                    $"<p>You requested a password reset. Click this <a href='{resetLink}'>link</a> to reset your password </p>"
                    );


                

            }
            return Ok(new { message = "If the email exists, reset link is there." });


        }





        public class Auth1ForgotPasswordEndpointRequest
        {

            public required string Email { get; set; }
        }
    }
}
