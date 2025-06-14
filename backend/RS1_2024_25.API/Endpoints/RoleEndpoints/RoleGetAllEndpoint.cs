
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;

namespace RS1_2024_25.API.Endpoints.RoleEndpoints
{

    [Authorize]
    [Route("roles")]
    public class RoleGetAllEndpoint(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<RoleGetAllResponse>
    {
        [HttpGet]
        public override async Task<RoleGetAllResponse> HandleAsync(CancellationToken cancellationToken = default)
        {
            var roles = await db.Roles.Select(x => new RoleGetAllItem
            {
                ID = x.ID,
                Name = x.Name
            }).ToListAsync(cancellationToken);

            return new RoleGetAllResponse
            {
                Roles = roles,
                Message = $"Found {roles.Count} roles"

            };


        }





    }



    public class RoleGetAllResponse
    {
        public required List<RoleGetAllItem> Roles { get; set; }
         
         public string? Message { get; set; }
    }

    public class RoleGetAllItem
    {
        public required int ID { get; set; }
        public required string Name { get; set; }

    }
}
