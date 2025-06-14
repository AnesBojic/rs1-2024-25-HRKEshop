using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;
using System.Security.Principal;

namespace RS1_2024_25.API.Endpoints.AppUserEndpoints
{
    [Authorize(Roles ="Admin")]
    [Route("appusers")]
    public class AppUserDeleteEndpoint(ApplicationDbContext db):MyEndpointBaseAsync
        .WithRequest<int>
        .WithoutResult
    {
        [HttpDelete("{id}")]
        public override async Task<ActionResult> HandleAsync([FromRoute] int id , CancellationToken cancellationToken=default)
        {
            
            //Cleanup , changing for tenantFiltering
            var user = await db.AppUsers.FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (user == null)
            {
                return NotFound("User not found");
            }
            db.Remove(user);
            await db.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
