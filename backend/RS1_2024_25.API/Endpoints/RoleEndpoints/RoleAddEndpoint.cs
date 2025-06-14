using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.RoleEndpoints
{
    [Route("role/add")]
    public class RoleAddEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithRequest<RoleAddRequest>
        .WithResult<string>
    {
        [HttpPost]
        public override async Task<string> HandleAsync([FromBody] RoleAddRequest request,CancellationToken cancellationToken = default)
        {
            var exists = await db.Roles.AnyAsync(r=> r.Name.Trim() == request.Name.Trim());
            if(exists)
            {
                throw  new BadHttpRequestException("Role already exists..");
            }

            var role = new Role
            {
                Name = request.Name.Trim(),
            };

            db.Roles.Add(role);
            await db.SaveChangesAsync();



            return $"Role {role.Name} created.";




        }


    }

    public class RoleAddRequest
    {
        public required string Name { get; set; }
    }

    
}
