using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class KeyPointQueryTests : BaseToursIntegrationTest
{
    public KeyPointQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_keypoints_by_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Get key points for tour -2 (Art Gallery Tour)
        var result = ((ObjectResult)controller.GetByTour(-2).Result)?.Value as List<KeyPointDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3); // Tour -2 has 3 key points
        result.All(kp => kp.TourId == -2).ShouldBeTrue();

        // Verify they are ordered correctly
        result.First().Order.ShouldBe(1);
        result.First().Name.ShouldBe("Modern Art Museum");
    }

    [Fact]
    public void Retrieves_empty_result_for_tour_without_keypoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Get key points for tour -1 (has no key points)
        var result = ((ObjectResult)controller.GetByTour(-1).Result)?.Value as List<KeyPointDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Retrieves_specific_keypoint()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as KeyPointDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Name.ShouldBe("Modern Art Museum");
        result.TourId.ShouldBe(-2);
        result.Order.ShouldBe(1);
        result.Latitude.ShouldBe(45.2671);
        result.Longitude.ShouldBe(19.8335);
        result.ImageUrl.ShouldBe("https://example.com/museum1.jpg");
        result.Description.ShouldBe("Famous museum with contemporary art");
    }

    [Fact]
    public void Get_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = (ObjectResult)controller.Get(-1000).Result;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(404);
    }

    [Fact]
    public void Keypoints_ordered_correctly()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Get key points for tour -2
        var result = ((ObjectResult)controller.GetByTour(-2).Result)?.Value as List<KeyPointDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        // Verify ordering
        result[0].Order.ShouldBe(1);
        result[0].Name.ShouldBe("Modern Art Museum");

        result[1].Order.ShouldBe(2);
        result[1].Name.ShouldBe("Gallery Center");

        result[2].Order.ShouldBe(3);
        result[2].Name.ShouldBe("Sculpture Park");
    }

    [Fact]
    public void Retrieves_keypoints_for_food_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Get key points for tour -5 (Food Tour)
        var result = ((ObjectResult)controller.GetByTour(-5).Result)?.Value as List<KeyPointDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // Tour -5 has 2 key points
        result.All(kp => kp.TourId == -5).ShouldBeTrue();

        // Verify specific points
        var restaurant = result.First(kp => kp.Order == 1);
        restaurant.Name.ShouldBe("Traditional Restaurant");
        restaurant.Description.ShouldBe("Family-owned restaurant since 1920");

        var market = result.First(kp => kp.Order == 2);
        market.Name.ShouldBe("Local Market");
        market.Description.ShouldBe("Fresh ingredients market");
    }

    [Fact]
    public void Retrieves_keypoints_with_all_required_fields()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetByTour(-2).Result)?.Value as List<KeyPointDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        foreach (var keyPoint in result)
        {
            keyPoint.Id.ShouldNotBe(0);
            keyPoint.TourId.ShouldBe(-2);
            keyPoint.Name.ShouldNotBeNullOrEmpty();
            keyPoint.Description.ShouldNotBeNullOrEmpty();
            keyPoint.Latitude.ShouldBeInRange(-90, 90);
            keyPoint.Longitude.ShouldBeInRange(-180, 180);
            keyPoint.ImageUrl.ShouldNotBeNullOrEmpty();
            keyPoint.Order.ShouldBeGreaterThan(0);
        }
    }

    private static KeyPointController CreateController(IServiceScope scope)
    {
        return new KeyPointController(scope.ServiceProvider.GetRequiredService<IKeyPointService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}