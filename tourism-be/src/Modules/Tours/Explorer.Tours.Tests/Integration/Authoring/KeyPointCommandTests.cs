using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class KeyPointCommandTests : BaseToursIntegrationTest
{
    public KeyPointCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newKeyPoint = new KeyPointDto
        {
            TourId = -1,
            Name = "Central Park",
            Description = "Beautiful central park area",
            Latitude = 45.2671,
            Longitude = 19.8335,
            ImageUrl = "https://example.com/park.jpg",
            Order = 1
        };

        // Act
        var result = ((ObjectResult)controller.Create(newKeyPoint).Result)?.Value as KeyPointDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newKeyPoint.Name);
        result.TourId.ShouldBe(newKeyPoint.TourId);
        result.Latitude.ShouldBe(newKeyPoint.Latitude);
        result.Longitude.ShouldBe(newKeyPoint.Longitude);
        result.Order.ShouldBe(newKeyPoint.Order);

        // Assert - Database
        var storedEntity = dbContext.KeyPoints.FirstOrDefault(kp => kp.Name == newKeyPoint.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.TourId.ShouldBe(newKeyPoint.TourId);
    }

    [Fact]
    public void Create_fails_invalid_coordinates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidKeyPoint = new KeyPointDto
        {
            TourId = -1,
            Name = "Test Point",
            Description = "Test description",
            Latitude = 91, // Invalid latitude (> 90)
            Longitude = 19.8335,
            ImageUrl = "https://example.com/test.jpg",
            Order = 1
        };

        // Act
        var result = (ObjectResult)controller.Create(invalidKeyPoint).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Create_fails_invalid_longitude()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidKeyPoint = new KeyPointDto
        {
            TourId = -1,
            Name = "Test Point",
            Description = "Test description",
            Latitude = 45.2671,
            Longitude = 181, // Invalid longitude (> 180)
            ImageUrl = "https://example.com/test.jpg",
            Order = 1
        };

        // Act
        var result = (ObjectResult)controller.Create(invalidKeyPoint).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Updates_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedKeyPoint = new KeyPointDto
        {
            Id = -1,
            TourId = -2,
            Name = "Updated Modern Art Museum",
            Description = "Updated famous museum with contemporary art",
            Latitude = 45.2680,
            Longitude = 19.8340,
            ImageUrl = "https://example.com/updated-museum.jpg",
            Order = 1
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedKeyPoint).Result)?.Value as KeyPointDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedKeyPoint.Name);
        result.Description.ShouldBe(updatedKeyPoint.Description);
        result.Latitude.ShouldBe(updatedKeyPoint.Latitude);
        result.Longitude.ShouldBe(updatedKeyPoint.Longitude);

        // Assert - Database
        var storedEntity = dbContext.KeyPoints.FirstOrDefault(kp => kp.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedKeyPoint.Name);
        storedEntity.Description.ShouldBe(updatedKeyPoint.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedKeyPoint = new KeyPointDto
        {
            Id = -1000,
            Name = "Test Point",
            Description = "Test description",
            Latitude = 45.2671,
            Longitude = 19.8335,
            ImageUrl = "https://example.com/test.jpg",
            Order = 1
        };

        // Act
        var result = (ObjectResult)controller.Update(updatedKeyPoint).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Deletes_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-7);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedKeyPoint = dbContext.KeyPoints.FirstOrDefault(kp => kp.Id == -7);
        storedKeyPoint.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = (ObjectResult)controller.Delete(-1000);

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    private static KeyPointController CreateController(IServiceScope scope)
    {
        return new KeyPointController(scope.ServiceProvider.GetRequiredService<IKeyPointService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}