
using Bogus;
using Microsoft.AspNetCore.Hosting;
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
using System.IO.Compression;
using System.Net;

namespace RS1_2024_25.API.Endpoints.DataSeedEndpoints
{
    [Route("data-seed-base")]
    public class DataSeedBaseSeed(ApplicationDbContext db, IWebHostEnvironment env) : MyEndpointBaseAsync
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
                    new Category { Name = "T shirt", TenantId = 1 },
                    new Category { Name = "Shirt", TenantId = 1 },
                    new Category { Name = "Shorts", TenantId = 1 },
                    new Category { Name = "Pants", TenantId = 1 },
                    new Category { Name = "Sneakers", TenantId = 1 },
                   
                };
                await db.CategoryAll.AddRangeAsync(category);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;

            if (!db.Categories_ProductsAll.Any())
            {
                var categoryProduct = new List<categories_products>
                {
                    new categories_products { ProductId = 2, CategoryId = 5, TenantId = 1 },
                    new categories_products { ProductId = 3, CategoryId = 5, TenantId = 1 },
                    new categories_products { ProductId = 5, CategoryId = 5, TenantId = 1 },
                  

                };
                await db.Categories_ProductsAll.AddRangeAsync(categoryProduct);
                await db.SaveChangesAsync(cancellationToken);
            }
            ;

            await db.SaveChangesAsync(cancellationToken);

            await EnsureCatalogForEveryTenantAsync(db, cancellationToken);
            await EnsureColorVariantsAsync(db, cancellationToken);
            await EnsureProductImagesAsync(db, env, cancellationToken);

            return "Data generated successfully :D";

        }

        private static async Task EnsureCatalogForEveryTenantAsync(ApplicationDbContext db, CancellationToken cancellationToken)
        {
            var tenantIds = await db.Tenants.Select(t => t.ID).OrderBy(id => id).ToListAsync(cancellationToken);
            if (tenantIds.Count < 2)
            {
                return;
            }

            var sourceTenantId = tenantIds[0];
            if (!await db.ProductsAll.AnyAsync(p => p.TenantId == sourceTenantId, cancellationToken))
            {
                return;
            }

            foreach (var tenantId in tenantIds.Skip(1))
            {
                if (await db.ProductsAll.AnyAsync(p => p.TenantId == tenantId, cancellationToken))
                {
                    continue;
                }

                var brandMap = new Dictionary<int, int>();
                foreach (var brand in await db.BrandsAll.Where(b => b.TenantId == sourceTenantId).ToListAsync(cancellationToken))
                {
                    var copy = new Brand { Name = brand.Name, TenantId = tenantId };
                    db.BrandsAll.Add(copy);
                    await db.SaveChangesAsync(cancellationToken);
                    brandMap[brand.ID] = copy.ID;
                }

                var sizeTypeMap = new Dictionary<int, int>();
                foreach (var sizeType in await db.SizeTypesAll.Where(s => s.TenantId == sourceTenantId).ToListAsync(cancellationToken))
                {
                    var copy = new SizeType { Name = sizeType.Name, TenantId = tenantId };
                    db.SizeTypesAll.Add(copy);
                    await db.SaveChangesAsync(cancellationToken);
                    sizeTypeMap[sizeType.ID] = copy.ID;
                }

                var sizeMap = new Dictionary<int, int>();
                foreach (var size in await db.SizesAll.Where(s => s.TenantId == sourceTenantId).ToListAsync(cancellationToken))
                {
                    if (!sizeTypeMap.TryGetValue(size.SizeTypeId, out var newSizeTypeId))
                    {
                        continue;
                    }

                    var copy = new Size
                    {
                        Value = size.Value,
                        SizeTypeId = newSizeTypeId,
                        TenantId = tenantId
                    };
                    db.SizesAll.Add(copy);
                    await db.SaveChangesAsync(cancellationToken);
                    sizeMap[size.ID] = copy.ID;
                }

                var productMap = new Dictionary<int, int>();
                foreach (var product in await db.ProductsAll.Where(p => p.TenantId == sourceTenantId).ToListAsync(cancellationToken))
                {
                    var copy = new Product
                    {
                        Name = product.Name,
                        Price = product.Price,
                        Gender = product.Gender,
                        ColorId = product.ColorId,
                        BrandId = brandMap.TryGetValue(product.BrandId, out var newBrandId) ? newBrandId : product.BrandId,
                        TenantId = tenantId
                    };
                    db.ProductsAll.Add(copy);
                    await db.SaveChangesAsync(cancellationToken);
                    productMap[product.ID] = copy.ID;
                }

                var productSizes = await db.ProductsSizesAll.Where(ps => ps.TenantId == sourceTenantId).ToListAsync(cancellationToken);
                foreach (var productSize in productSizes)
                {
                    if (!productMap.TryGetValue(productSize.ProductId, out var newProductId)
                        || !sizeMap.TryGetValue(productSize.SizeId, out var newSizeId))
                    {
                        continue;
                    }

                    db.ProductsSizesAll.Add(new ProductSize
                    {
                        ProductId = newProductId,
                        SizeId = newSizeId,
                        TenantId = tenantId,
                        Price = productSize.Price,
                        Stock = productSize.Stock
                    });
                }

                await db.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task EnsureColorVariantsAsync(ApplicationDbContext db, CancellationToken cancellationToken)
        {
            var colors = await db.Colors.OrderBy(c => c.ID).ToListAsync(cancellationToken);
            if (colors.Count < 2)
            {
                return;
            }

            var tenantIds = await db.Tenants.Select(t => t.ID).ToListAsync(cancellationToken);
            foreach (var tenantId in tenantIds)
            {
                var products = await db.ProductsAll.Where(p => p.TenantId == tenantId).ToListAsync(cancellationToken);
                foreach (var group in products.GroupBy(p => p.Name))
                {
                    var usedColors = group.Select(p => p.ColorId).ToHashSet();
                    if (usedColors.Count >= 3)
                    {
                        continue;
                    }

                    var source = group.OrderBy(p => p.ID).First();
                    var sourceSizes = await db.ProductsSizesAll
                        .Where(ps => ps.ProductId == source.ID)
                        .ToListAsync(cancellationToken);

                    foreach (var color in colors.Where(c => !usedColors.Contains(c.ID)).Take(3 - usedColors.Count))
                    {
                        var copy = new Product
                        {
                            Name = source.Name,
                            Price = source.Price,
                            Gender = source.Gender,
                            ColorId = color.ID,
                            BrandId = source.BrandId,
                            TenantId = tenantId
                        };
                        db.ProductsAll.Add(copy);
                        await db.SaveChangesAsync(cancellationToken);

                        foreach (var size in sourceSizes)
                        {
                            db.ProductsSizesAll.Add(new ProductSize
                            {
                                ProductId = copy.ID,
                                SizeId = size.SizeId,
                                TenantId = tenantId,
                                Price = size.Price,
                                Stock = size.Stock
                            });
                        }

                        await db.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }

        private static async Task EnsureProductImagesAsync(ApplicationDbContext db, IWebHostEnvironment env, CancellationToken cancellationToken)
        {
            var products = await db.ProductsAll.OrderBy(p => p.ID).ToListAsync(cancellationToken);
            if (products.Count == 0)
            {
                return;
            }

            var existingProductIds = await db.ImagesAll
                .Where(img => img.ImageableType.ToLower() == "products")
                .Select(img => img.ImageableId)
                .ToListAsync(cancellationToken);
            var alreadySeeded = existingProductIds.ToHashSet();

            var webRoot = string.IsNullOrWhiteSpace(env.WebRootPath)
                ? Path.Combine(env.ContentRootPath, "wwwroot")
                : env.WebRootPath;
            var folder = Path.Combine(webRoot, "images", "products");
            Directory.CreateDirectory(folder);

            var images = new List<Image>();
            foreach (var product in products)
            {
                if (alreadySeeded.Contains(product.ID))
                {
                    continue;
                }

                var fileName = $"product-{product.ID}.png";
                var fullPath = Path.Combine(folder, fileName);
                WriteProductPng(fullPath, product.ID, product.Name);

                images.Add(new Image
                {
                    Name = product.Name,
                    ImageableId = product.ID,
                    ImageableType = "products",
                    FilePath = fullPath,
                    Url = $"/images/products/{fileName}",
                    TenantId = product.TenantId
                });
            }

            if (images.Count == 0)
            {
                return;
            }

            await db.ImagesAll.AddRangeAsync(images, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        private static void WriteProductPng(string path, int productId, string name)
        {
            const int width = 320;
            const int height = 240;
            var hue = (productId * 47) % 360;
            var (r, g, b) = HsvToRgb(hue, 0.55, 0.85);
            var (sr, sg, sb) = HsvToRgb((hue + 28) % 360, 0.35, 0.96);

            var raw = new byte[height * (1 + width * 3)];
            var i = 0;
            for (var y = 0; y < height; y++)
            {
                raw[i++] = 0;
                for (var x = 0; x < width; x++)
                {
                    var inCard = x > 36 && x < width - 36 && y > 28 && y < height - 28;
                    var inItem = x > 96 && x < width - 96 && y > 58 && y < height - 52;
                    var stripe = inItem && ((x + y + productId) % 18) < 6;
                    byte pr = inItem ? (stripe ? (byte)Math.Min(255, r + 30) : r) : (inCard ? sr : (byte)236);
                    byte pg = inItem ? (stripe ? (byte)Math.Min(255, g + 30) : g) : (inCard ? sg : (byte)236);
                    byte pb = inItem ? (stripe ? (byte)Math.Min(255, b + 30) : b) : (inCard ? sb : (byte)236);

                    if (!inItem && inCard && y > height - 70 && y < height - 46 && x > 56 && x < width - 56)
                    {
                        var labelIndex = (x - 56) / 8;
                        var show = labelIndex < name.Length && (name[labelIndex] % 2 == (y % 2));
                        if (show)
                        {
                            pr = pg = pb = 70;
                        }
                    }

                    raw[i++] = pr;
                    raw[i++] = pg;
                    raw[i++] = pb;
                }
            }

            using var idat = new MemoryStream();
            using (var zlib = new ZLibStream(idat, CompressionLevel.Fastest, leaveOpen: true))
            {
                zlib.Write(raw, 0, raw.Length);
            }

            using var output = new FileStream(path, FileMode.Create, FileAccess.Write);
            output.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

            var ihdr = new byte[13];
            WriteInt(ihdr, 0, width);
            WriteInt(ihdr, 4, height);
            ihdr[8] = 8;
            ihdr[9] = 2;
            WriteChunk(output, "IHDR"u8, ihdr);
            WriteChunk(output, "IDAT"u8, idat.ToArray());
            WriteChunk(output, "IEND"u8, Array.Empty<byte>());
        }

        private static void WriteChunk(Stream output, ReadOnlySpan<byte> type, byte[] data)
        {
            Span<byte> length = stackalloc byte[4];
            WriteInt(length, 0, data.Length);
            output.Write(length);
            output.Write(type);
            output.Write(data);

            var crcSource = new byte[type.Length + data.Length];
            type.CopyTo(crcSource);
            data.CopyTo(crcSource.AsSpan(type.Length));
            Span<byte> crc = stackalloc byte[4];
            WriteInt(crc, 0, (int)Crc32(crcSource));
            output.Write(crc);
        }

        private static void WriteInt(Span<byte> buffer, int offset, int value)
        {
            var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
            bytes.CopyTo(buffer.Slice(offset, 4));
        }

        private static uint Crc32(byte[] data)
        {
            var crc = 0xFFFFFFFFu;
            foreach (var value in data)
            {
                crc ^= value;
                for (var bit = 0; bit < 8; bit++)
                {
                    var mask = (uint)-(int)(crc & 1);
                    crc = (crc >> 1) ^ (0xEDB88320u & mask);
                }
            }

            return ~crc;
        }

        private static (byte r, byte g, byte b) HsvToRgb(int hue, double saturation, double value)
        {
            var h = hue / 60.0;
            var c = value * saturation;
            var x = c * (1 - Math.Abs(h % 2 - 1));
            var m = value - c;
            double r1 = 0, g1 = 0, b1 = 0;
            switch ((int)h)
            {
                case 0: r1 = c; g1 = x; break;
                case 1: r1 = x; g1 = c; break;
                case 2: g1 = c; b1 = x; break;
                case 3: g1 = x; b1 = c; break;
                case 4: r1 = x; b1 = c; break;
                default: r1 = c; b1 = x; break;
            }

            return ((byte)((r1 + m) * 255), (byte)((g1 + m) * 255), (byte)((b1 + m) * 255));
        }
    }
}
