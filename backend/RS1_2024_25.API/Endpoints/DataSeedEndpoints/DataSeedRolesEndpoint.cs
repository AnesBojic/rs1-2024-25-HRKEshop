using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Helper.Api;


namespace RS1_2024_25.API.Endpoints.DataSeedEndpoints
{
    [Route("data-seed-roles")]
    public class DataSeedRolesEndpoint(ApplicationDbContext db)
    : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<string>
    {
        [HttpPost]
        public override async Task<string> HandleAsync(CancellationToken cancellationToken = default)
        {
            if(db.Roles.Any())
            {
                return "Roles already generated.";
            }
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin"
                },
                new Role
                {
                    Name = "Manager"
                },
                new Role
                {
                    Name = "Customer"
                }
            };

            //Adding roles
            await db.AddRangeAsync(roles, cancellationToken);

            //Saving

            await db.SaveChangesAsync(cancellationToken);


            return "Roles data generated...";
        }


    }
}
