using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using static RS1_2024_25.API.Endpoints.Auth1Endpoints.Auth1LoginEndpoint;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{

    [Route("auth1/login")]
    public class Auth1LoginEndpoint(AuthService authService,ApplicationDbContext db)
        : MyEndpointBaseAsync
        .WithRequest<Auth1LoginRequest>
        .WithActionResult<Auth1LoginResponse>

    {
        [HttpPost]
        public override async Task<ActionResult<Auth1LoginResponse>> HandleAsync(Auth1LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await db.AppUsersAll.Include(u=> u.Role).FirstOrDefaultAsync(x=> x.Email == request.Email);



            

            
            if (user == null)
                {

                return BadRequest(new ApiErrorResponse(ApiErrorCodes.InvalidCredentials,"Invalid credentials"));

            }

            if(user.isLocked())
            {
                var lockoutUntilLocal = user.LockoutUntil?.ToLocalTime();
                return BadRequest(new ApiErrorResponse(ApiErrorCodes.AccountLocked, $"Account is locked until {lockoutUntilLocal?.ToString("g")}"));
            }

            if(!user.VerifiyPassword(request.Password))
            {
                await db.SaveChangesAsync();
                return BadRequest(new ApiErrorResponse(ApiErrorCodes.InvalidCredentials,"Invalid credentials"));
            }


            if (user.EmailVerifiedAt == null)
            {
                return BadRequest(new ApiErrorResponse(ApiErrorCodes.EmailNotVerified,"Please verify your email before logging in"));
            }

            var token = authService.GenerateJwtToken(user);
            var refreshToken = authService.GenerateRefreshToken();

            user.RememberToken = refreshToken;
            await db.SaveChangesAsync();


            return Ok(new Auth1LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                Email = user.Email


            });


        }
        public class Auth1LoginRequest
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class Auth1LoginResponse
        {
            public required string Token { get; set; }

            public required string RefreshToken { get; set; }
            public required string Email { get; set; }
        }

    }

    
}
