using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.RoleEndpoints
{

    [Route("roles")]
    public class RoleGetByIdEndpoint(ApplicationDbContext db): MyEndpointBaseAsync
        .WithRequest<int>
        .WithResult<RoleGetByIdResponse>
    {
        [HttpGet("{id}")]
        public override async Task<RoleGetByIdResponse> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var role = await db.Roles.FirstOrDefaultAsync(r => r.ID == id);

            if(role == null)
            {
                throw new KeyNotFoundException("Role not found");
            }
            var roleResponse = new RoleGetByIdResponse
            {
                ID = role.ID,
                Name = role.Name
            };

            return roleResponse;


        }
    }


    public class RoleGetByIdResponse
    {
        public required int ID { get; set; }
        public required string Name { get; set; }

    }



}
