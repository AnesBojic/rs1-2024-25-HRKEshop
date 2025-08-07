using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using System.Security.Claims;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{
    [Route("auth/logout1")]
    [Authorize]
    public class Auth1LogoutEndpoint(ApplicationDbContext db) :MyEndpointBaseAsync
        .WithoutRequest
        .WithActionResult<string>
    {
        [HttpPost]
        public override async Task<ActionResult<string>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
            {
                return Unauthorized("Invalid token");
            }

            var userId = int.Parse(userIdStr);

            var user = await db.AppUsers.FirstOrDefaultAsync(u => u.ID == userId, cancellationToken);
            if (user == null)
            {
                return NotFound("User not found");
            }
            user.RememberToken = null;

            await db.SaveChangesAsync(cancellationToken);

            return Ok("Logged out successfully.");

        }

    }
}
