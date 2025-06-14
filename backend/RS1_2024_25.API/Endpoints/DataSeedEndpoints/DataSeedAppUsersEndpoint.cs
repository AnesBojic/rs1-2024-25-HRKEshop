
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Helper.Api;
using Bogus;
using RS1_2024_25.API.Data.Models.SharedTables;

namespace RS1_2024_25.API.Endpoints.DataSeedEndpoints
{
    [Route("data-seed-users")]
    public class DataSeedAppUsersEndpoint(ApplicationDbContext db)
        :MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<string>
    {
        [HttpPost]
        public override async Task<string> HandleAsync(CancellationToken cancellationToken=default)
        {

            var webShopTenant = db.Tenants.FirstOrDefault(t => t.Name == "Hrke Shop");
            if(webShopTenant == null)
            {
                webShopTenant = new Tenant
                {
                    Name = "Hrke Shop",
                    DatabaseConnection = "DefaultConnection",
                    ServerAddress = "localhost"
                };
                await db.Tenants.AddAsync(webShopTenant);
                await db.SaveChangesAsync();
                
            }
            db.CurrentTenantId = webShopTenant.ID;



            if (db.AppUsers.Any())
            {
                return "Users already seeded..";
            }
            var roles = db.Roles.ToList();
            if(roles.Count == 0)
            {
                return "No roles found. Seed roles first!";
            }

            
            var rnd = new Random();
            var faker = new Faker("en");

            var users = new List<AppUser>();

            for(int i = 1;i <=10;i++)
            {
                var fakeUser = new Faker<AppUser>()
                    .RuleFor(u => u.Name, f => f.Name.FirstName())
                    .RuleFor(u => u.Surname, f => f.Name.LastName())
                    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name, u.Surname))
                    .RuleFor(u => u.Address, f => f.Address.StreetAddress())
                    .RuleFor(u => u.City, f => f.Address.City())
                    .RuleFor(u => u.ZipCode, f => f.Address.ZipCode())
                    .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("062-###-###"))
                    .RuleFor(u => u.EmailVerifiedAt, f => f.Date.Past())
                    .FinishWith((f, u) =>
                    {
                        u.RoleID = roles[rnd.Next(roles.Count)].ID;
                        u.SetPassword("test1234");
                    });
                users.Add(fakeUser.Generate());

            }
            await db.AddRangeAsync(users, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            return "10 users random seed.";
        }

    }
}
