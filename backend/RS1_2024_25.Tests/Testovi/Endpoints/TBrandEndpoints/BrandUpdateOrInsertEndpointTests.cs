using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;
using RS1_2024_25.API.Endpoints.BrandEndpoints;
using RS1_2024_25.Tests.Services;
using Xunit;

public class BrandUpdateOrInsertEndpointTests
{
    [Fact]
    public async Task Insert_ShouldCreateNewBrand()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();
        var endpoint = new BrandUpdateOrInsertEndpoint(db);

        var request = new BrandUpdateOrInsertEndpoint.BrandUpdateOrInsertRequest
        {
            ID = null,
            Name = "Nike"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        int newId = (int)ok.Value;

        var brand = await db.BrandsAll.FindAsync(newId); // <- BrandsAll
        brand.Should().NotBeNull();
        brand.Name.Should().Be("Nike");
    }

    [Fact]
    public async Task Update_ShouldModifyExistingBrand()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();

        db.BrandsAll.Add(new Brand { ID = 55, Name = "OldBrand", TenantId = 1 });
        await db.SaveChangesAsync();

        var endpoint = new BrandUpdateOrInsertEndpoint(db);

        var request = new BrandUpdateOrInsertEndpoint.BrandUpdateOrInsertRequest
        {
            ID = 55,
            Name = "UpdatedBrand"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var updated = await db.BrandsAll.FindAsync(55);
        updated.Should().NotBeNull();
        updated.Name.Should().Be("UpdatedBrand");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenBrandDoesNotExist()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();
        var endpoint = new BrandUpdateOrInsertEndpoint(db);

        var request = new BrandUpdateOrInsertEndpoint.BrandUpdateOrInsertRequest
        {
            ID = 999,
            Name = "NonExistentBrand"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
