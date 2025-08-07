
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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

            if (!db.AppUsersAll.Any())
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


                    if (faker.Random.Bool(0.05f))
                    {
                        user.LockAccount(faker.Random.Int(1, 30));
                    }


                    users.Add(user);
                }

                await db.AppUsersAll.AddRangeAsync(users);
                await db.SaveChangesAsync(cancellationToken);
            }
            if (!db.ImagesAll.Any())
            {
                Console.WriteLine("Seeding images for users ? ");

                var users = await db.AppUsersAll.Take(10).ToListAsync();

                var images = new List<Image>();

                foreach (var user in users)
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
            }
            ;

            if (!db.BrandsAll.Any())
            {
                var faker = new Faker();
                var brands = new List<Brand>
                {
                    new Brand { Name = "Nike", TenantId =  1},
                    new Brand { Name = "Adidas",TenantId =  1 },
                    new Brand { Name = "Puma",TenantId =  1 },
                    new Brand { Name = "Under Armour",TenantId =  1 },
                    new Brand { Name = "Reebook",TenantId =  1 },

                };
                await db.BrandsAll.AddRangeAsync(brands);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;
            if (!db.SizeTypesAll.Any())
            {
                var sizeTypes = new List<SizeType>
                {
                    new SizeType{Name = "Men's clothing", TenantId = 1},
                    new SizeType {Name = "Women's clothing", TenantId = 1},
                    new SizeType {Name = "Kids' Clothing", TenantId = 1},
                    new SizeType{Name = "Shoes", TenantId = 1},
                    new SizeType {Name = "Toys", TenantId = 1},
                    new SizeType {Name = "Furniture", TenantId = 1},
                    new SizeType {Name = "Bicycles", TenantId = 1},
                    new SizeType {Name = "One Size Fits All", TenantId = 1}


                };
                await db.SizeTypesAll.AddRangeAsync(sizeTypes);
                await db.SaveChangesAsync(cancellationToken);
            }
            if (!db.SizesAll.Any())
            {
                var sizesToSeed = new List<Size>();
                var sizeTypes = await db.SizeTypesAll.ToListAsync();

                foreach (var st in sizeTypes)
                {
                    switch (st.Name)
                    {
                        case "Men's clothing":
                        case "Women's clothing":
                        case "Kids' Clothing":
                            sizesToSeed.AddRange(new[]
                            {
                    new Size { Value = "XS", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "S", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "M", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "L", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "XL", SizeTypeId = st.ID, TenantId = 1 }
                });
                            break;

                        case "Shoes":
                            sizesToSeed.AddRange(Enumerable.Range(36, 10).Select(i =>
                                new Size { Value = i.ToString(), SizeTypeId = st.ID, TenantId = 1 }));
                            break;

                        case "Toys":
                        case "Furniture":
                        case "Bicycles":
                            sizesToSeed.AddRange(new[]
                            {
                    new Size { Value = "Small", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "Medium", SizeTypeId = st.ID, TenantId = 1 },
                    new Size { Value = "Large", SizeTypeId = st.ID, TenantId = 1 }
                });
                            break;

                        case "One Size Fits All":
                            sizesToSeed.Add(new Size
                            {
                                Value = "One Size",
                                SizeTypeId = st.ID,
                                TenantId = 1
                            });
                            break;
                    }

                    


                }

                await db.SizesAll.AddRangeAsync(sizesToSeed);
                await db.SaveChangesAsync();
            }
            if(!db.ProductsAll.Any())
            {
                var faker = new Faker();
                var brands = await db.BrandsAll.ToListAsync();
                var colors = await db.Colors.ToListAsync();

                var products = new List<Product>();

                for (int i = 0; i < 30; i++)
                {
                    var product = new Product
                    {
                        Name = faker.Commerce.ProductName(),
                        Price = faker.Random.Float(20, 200),
                        ColorId = faker.PickRandom(colors).ID,
                        BrandId = faker.PickRandom(brands).ID,
                        Gender = faker.PickRandom<Gender>(),
                        TenantId = 1




                    };

                    products.Add(product);

                }

                await db.ProductsAll.AddRangeAsync(products);
                await db.SaveChangesAsync(cancellationToken);
                



            }

            if(!db.ProductsSizesAll.Any())
            {
                var faker = new Faker();
                //Dobit cemo sve iz tenantId=1 jer je postavljen
                var products = await db.ProductsAll.ToListAsync();
                var sizes = await db.SizesAll.ToListAsync();

                var productSizes = new List<ProductSize>();

                foreach (var product in products)
                {
                    var availableSizes = faker.PickRandom(sizes, faker.Random.Int(2, 5)).ToList();

                    foreach (var size in availableSizes)
                    {
                        var minPrice = (decimal)Math.Max(1, product.Price - 20);
                        productSizes.Add(new ProductSize
                        {
                            ProductId = product.ID,
                            SizeId = size.ID,
                            TenantId = 1,
                            Price = faker.Random.Decimal(minPrice,(decimal)product.Price+20),
                            Stock = faker.Random.Int(0,122)


                        });


                    }


                }
                await db.ProductsSizesAll.AddRangeAsync(productSizes);
                await db.SaveChangesAsync();



            }

            if (!db.CategoryAll.Any())
            {
                var category = new List<Category>
                {
                    new Category { Name = "T shirt" },
                    new Category { Name = "Shirt" },
                    new Category { Name = "Shorts" },
                    new Category { Name = "Pants" },
                    new Category { Name = "Sneakers" },
                   
                };
                await db.CategoryAll.AddRangeAsync(category);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;

            if (!db.Categories_ProductsAll.Any())
            {
                var categoryProduct = new List<categories_products>
                {
                    new categories_products { ProductId = 2, CategoryId = 5 },
                    new categories_products { ProductId = 3, CategoryId = 5 },
                    new categories_products {ProductId = 5, CategoryId = 5},
                  

                };
                await db.Categories_ProductsAll.AddRangeAsync(categoryProduct);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;

            await db.SaveChangesAsync(cancellationToken);


            return "Data generated successfully :D";

        }
    }
}
