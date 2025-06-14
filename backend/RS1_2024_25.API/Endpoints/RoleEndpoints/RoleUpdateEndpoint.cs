
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.RoleEndpoints
{

    [Route("roles")]
    public class RoleUpdateEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<RoleUpdatRequest>
        .WithResult<string>
    {

        [HttpPut]
        public override async Task<string> HandleAsync([FromBody] RoleUpdatRequest request,CancellationToken cancellationToken = default)
        {
            var role = await db.Roles.FirstOrDefaultAsync(x => x.ID == request.ID);

            if(role == null)
            {
                throw new KeyNotFoundException("Role not found.");
            }

            var normalizedName = request.Name.Trim();

            var isDuplicate = await db.Roles.AnyAsync(x=> x.ID != request.ID && x.Name.ToLower().Trim() == normalizedName.ToLower(),cancellationToken);

            if(isDuplicate)
            {
                throw new InvalidOperationException("Another role with the same name exists.");
            }

            role.Name = normalizedName;

            await db.SaveChangesAsync(cancellationToken);

            return $"Role {role.Name} updated sucessfully.";



        }



    }



    public class RoleUpdatRequest
    {
        public required int ID { get; set; }

        public required string Name { get; set; }
    }
}
