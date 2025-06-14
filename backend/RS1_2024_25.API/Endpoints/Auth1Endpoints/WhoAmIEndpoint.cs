using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Helper.Api;
using System.Security.Claims;
using static RS1_2024_25.API.Endpoints.Auth1Endpoints.WhoAmIEndpoint;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{


    [Route("auth/me")]
    [Authorize]
    public class WhoAmIEndpoint: MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<Auth1WhoAmIResponse>
    {

        [HttpGet]
        public override async Task<ActionResult<Auth1WhoAmIResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if(email == null || role == null || id == null)
            {
                return Unauthorized("Token invalid or missing claims");
            }

            return Ok(new Auth1WhoAmIResponse
            {
                Id = int.Parse(id),
                Email = email,
                Role = role,
                FullName = name ?? "Unknown"


            });

            
        }
        public class Auth1WhoAmIResponse
        {
            public int Id { get; set; }

            public required string Email { get; set; }

            public required string FullName { get; set; }

            public required string Role { get; set; }




        }


    }

   
}
