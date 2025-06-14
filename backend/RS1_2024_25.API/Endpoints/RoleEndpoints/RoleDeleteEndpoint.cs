using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.RoleEndpoints
{
    [Route("roles")]
    public class RoleDeleteEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithoutResult
    {
        [HttpDelete("{id}")]
        public override async Task HandleAsync([FromRoute]int id,CancellationToken cancellationToken = default)
        {

            var isUsed = await db.AppUsersAll.AnyAsync(u => u.RoleID == id);

            if(isUsed)
            {
                throw new InvalidOperationException("Cannot delete role, currently in use...");
            }

            var role = await db.Roles.FirstOrDefaultAsync(x => x.ID == id);

            if (role == null)
            {
                throw new KeyNotFoundException("Role not found");
            }

            db.Remove(role);
            await db.SaveChangesAsync(cancellationToken);
                
           

        }


    }
}
