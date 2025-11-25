using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Data.Models.SharedTables;
using RS1_2024_25.API.Endpoints.ColorEndpoints;
using RS1_2024_25.Tests.Services;
using Xunit;

public class ColorUpdateOrInsertEndpointTests
{
    [Fact]
    public async Task Insert_ShouldCreateNewColor()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();
        var endpoint = new ColorUpdateOrInsertEndpoint(db);

        var request = new ColorUpdateOrInsertEndpoint.ColorUpdateOrInsertRequest
        {
            ID = null,
            Name = "Blue",
            Hex_Code = "#0000FF"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        int newId = (int)ok.Value;

        var color = await db.Colors.FindAsync(newId);
        color.Should().NotBeNull();
        color.Name.Should().Be("Blue");
    }

    [Fact]
    public async Task Update_ShouldModifyExistingColor()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();

        db.Colors.Add(new Color { ID = 55, Name = "Old", Hex_Code = "#111111" });
        await db.SaveChangesAsync();

        var endpoint = new ColorUpdateOrInsertEndpoint(db);

        var request = new ColorUpdateOrInsertEndpoint.ColorUpdateOrInsertRequest
        {
            ID = 55,
            Name = "Updated",
            Hex_Code = "#222222"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var updated = await db.Colors.FindAsync(55);
        updated.Name.Should().Be("Updated");
        updated.Hex_Code.Should().Be("#222222");
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenColorDoesNotExist()
    {
        // Arrange
        var db = await TestApplication1DbContext.CreateAsync();
        var endpoint = new ColorUpdateOrInsertEndpoint(db);

        var request = new ColorUpdateOrInsertEndpoint.ColorUpdateOrInsertRequest
        {
            ID = 999,
            Name = "X",
            Hex_Code = "#FFFFFF"
        };

        // Act
        var result = await endpoint.HandleAsync(request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
