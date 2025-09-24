using Explorer.API.Controllers.Author.Authoring;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newTour = new TourDto
        {
            Name = "Test Tour",
            Description = "Test tour description",
            Difficulty = 1,
            Category = 0,
            Price = 29.99m,
            StartDate = DateTime.UtcNow.AddDays(30),
            AuthorId = -11
        };

        // Act
        var result = ((ObjectResult)controller.Create(newTour).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newTour.Name);
        result.Status.ShouldBe(0); // Draft

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Name == newTour.Name);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Updates_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedTour = new TourDto
        {
            Id = -1,
            Name = "Updated Nature Walk",
            Description = "Updated beautiful nature tour in the park",
            Difficulty = 1,
            Category = 0,
            Price = 30.00m,
            StartDate = DateTime.SpecifyKind(DateTime.Parse("2025-12-01 10:00:00"), DateTimeKind.Utc),
            Status = 0,
            AuthorId = -11
        };

        // Act
        var result = ((ObjectResult)controller.Update(updatedTour).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe(updatedTour.Name);
        result.Description.ShouldBe(updatedTour.Description);

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Name.ShouldBe(updatedTour.Name);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedTour = new TourDto
        {
            Id = -1000,
            Name = "Test Tour"
        };

        // Act
        var result = (ObjectResult)controller.Update(updatedTour).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Publishes_tour_with_enough_keypoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act - Tour -2 has 3 key points, should be publishable
        var result = ((ObjectResult)controller.Publish(-2).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(1); // Published

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -2);
        storedEntity.ShouldNotBeNull();
        ((int)storedEntity.Status).ShouldBe(1);
    }

    [Fact]
    public void Publish_fails_insufficient_keypoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Tour -1 has 0 key points, should not be publishable
        var result = (ObjectResult)controller.Publish(-1).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
    }

    [Fact]
    public void Cancel_published_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act - Tour -4 is published and in future
        var result = ((ObjectResult)controller.Cancel(-6).Result)?.Value as TourDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Status.ShouldBe(2); // Cancelled

        // Assert - Database
        var storedEntity = dbContext.Tours.FirstOrDefault(t => t.Id == -6);
        storedEntity.ShouldNotBeNull();
        ((int)storedEntity.Status).ShouldBe(2);
    }

    [Fact]
    public void Deletes_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (OkResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        // Assert - Database
        var storedTour = dbContext.Tours.FirstOrDefault(t => t.Id == -3);
        storedTour.ShouldBeNull();
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

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}