
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Enums;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul1_Auth;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Helper.Api;
using System.ComponentModel.DataAnnotations.Schema;

namespace RS1_2024_25.API.Endpoints.DataSeedEndpoints
{
    [Route("data-seed-base")]
    public class DataSeedBaseSeed(ApplicationDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<string>
    {

        [HttpPost]
        public override async Task<string> HandleAsync(CancellationToken cancellationToken = default)
        {

            

            if (!db.Tenants.Any())
            {
                var tenants = new List<Tenant>
                {
                    new Tenant{Name="HRKEShop1" ,DatabaseConnection = "db_conn_HRKEShop1",ServerAddress ="192.168.2.22"},
                    new Tenant{Name="HRKEShop2" ,DatabaseConnection = "db_conn_HRKEShop2",ServerAddress ="192.168.2.23"},



                };

                await db.Tenants.AddRangeAsync(tenants);
                await db.SaveChangesAsync(cancellationToken);

            }
            

            
            if (!db.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role {Name="Admin"},
                    new Role {Name="Manager"},
                    new Role {Name="Customer"}
                };

                await db.Roles.AddRangeAsync(roles);
                await db.SaveChangesAsync(cancellationToken);

            }

            if(!db.AppUsersAll.Any())
            {
                var faker = new Faker();

                var users = new List<AppUser>();

                for (int i = 0; i < 50; i++)
                {
                    var user = new AppUser
                    {
                        RoleID = faker.PickRandom(new[] { 1, 2, 3 }),
                        TenantId = faker.PickRandom(new[] { 1, 2 }),
                        Name = faker.Name.FirstName(),
                        Surname = faker.Name.LastName(),
                        Email = faker.Internet.Email(),
                        EmailVerifiedAt = faker.Random.Bool(0.8f) ? DateTime.UtcNow : (DateTime?)null,
                        Address = faker.Address.StreetAddress(),
                        City = faker.Address.City(),
                        ZipCode = faker.Address.ZipCode(),
                        Phone = faker.Phone.PhoneNumber(),
                        FailedLoginAttempts = faker.Random.Int(0, 3),
                        LockoutUntil = null




                    };

                    user.SetPassword("test");


                    if(faker.Random.Bool(0.05f))
                    {
                        user.LockAccount(faker.Random.Int(1, 30));
                    }


                    users.Add(user);
                }

              await db.AppUsersAll.AddRangeAsync(users);
                await db.SaveChangesAsync(cancellationToken);
            }
            if(!db.ImagesAll.Any())
            {
                Console.WriteLine("Seeding images for users ? ");

                var users = await db.AppUsersAll.Take(10).ToListAsync();

                var images = new List<Image>();

                foreach(var user in users)
                {
                    images.Add(
                        new Image
                        {
                            Name = $"Profile pic for {user.Name}",
                            ImageableId = user.ID,
                            ImageableType = "users",
                            FilePath = $"fakepath/images/users/{Guid.NewGuid()}.jpg",
                            Url = $"/images/{Guid.NewGuid()}.jpg",
                            TenantId = user.TenantId


                        });


                    Console.WriteLine($"Seeding image for user: {user.Name}");


                }



                await db.ImagesAll.AddRangeAsync(images);
                await db.SaveChangesAsync();

            }

            if (!db.Colors.Any())
            {
                var colors = new List<Color>
                {
                    new Color { Name = "Red", Hex_Code = "#FF0000" },
                    new Color { Name = "Green", Hex_Code = "#00FF00" },
                    new Color { Name = "Blue", Hex_Code = "#0000FF" },
                    new Color { Name = "Black", Hex_Code = "#000000" },
                    new Color { Name = "White", Hex_Code = "#FFFFFF" },
                    new Color { Name = "Yellow", Hex_Code = "#FFFF00" },
                    new Color { Name = "Cyan", Hex_Code = "#00FFFF" },
                    new Color { Name = "Magenta", Hex_Code = "#FF00FF" }
                };
                await db.Colors.AddRangeAsync(colors);
                await db.SaveChangesAsync(cancellationToken);
            };

            if (!db.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand { Name = "Nike" },
                    new Brand { Name = "Adidas" },
                    new Brand { Name = "Puma" },
                    new Brand { Name = "Under Armour" },
                    new Brand { Name = "Reebook" },

                };
                await db.BrandsAll.AddRangeAsync(brands);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;

            if (!db.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Nike zoom", Price = 150, Gender = Gender.Male, ColorId = 4,  BrandId = 2 },
                    new Product { Name = "D Rose" , Price = 200, Gender = Gender.Male, ColorId = 5, BrandId = 3 },
                    new Product { Name = "Puma", Price = 100, Gender = Gender.Female, ColorId = 1, BrandId = 4 },
                    new Product { Name = "Curry 2", Price = 250, Gender = Gender.Female, ColorId = 3, BrandId = 5 },
                    


    };
                await db.ProductsAll.AddRangeAsync(products);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;





            await db.SaveChangesAsync(cancellationToken);


            return "Data generated successfully :D";

        }


    }
}
