
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using RS1_2024_25.API.Services;
using static RS1_2024_25.API.Endpoints.Auth1Endpoints.Auth1RefreshTokenEndpoint;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{
    [Route("auth/refresh")]
    public class Auth1RefreshTokenEndpoint(AuthService authService,ApplicationDbContext db):MyEndpointBaseAsync
        .WithRequest<Auth1RefreshTokenRequest>
        .WithActionResult<Auth1RefreshResponse>
    {

        [HttpPost]
        public override async Task<ActionResult<Auth1RefreshResponse>> HandleAsync(Auth1RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var user = await db.AppUsersAll.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

            if(user == null || user.RememberToken != request.RememberToken)
            {

                return Unauthorized("Invalid remember token");
            }

            var newJwt = authService.GenerateJwtToken(user);
            var newRememberToken = authService.GenerateRefreshToken();

            user.RememberToken = newRememberToken;
            await db.SaveChangesAsync();

            return Ok(new Auth1RefreshResponse
            {
                Token = newJwt,
                RememberToken = newRememberToken,
                Email = user.Email


            });



        }
        public class Auth1RefreshTokenRequest
        {
            public required string Email { get; set; }

            public required string RememberToken { get; set; }
        }

        public class Auth1RefreshResponse
        {
            public required string Token { get; set; }

            public required string RememberToken { get; set; }

            public required string Email { get; set; }


        }

    }

    
}
