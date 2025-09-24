using Explorer.API.Controllers.Author.Authoring;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Authoring;

[Collection("Sequential")]
public class TourQueryTests : BaseToursIntegrationTest
{
    public TourQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_tours_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetByAuthor().Result)?.Value as List<TourDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // Author -11 has 2 tours
        result.All(t => t.AuthorId == -11).ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_draft_tours_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Status 0 = Draft
        var result = ((ObjectResult)controller.GetByAuthorAndStatus(0).Result)?.Value as List<TourDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1); // Author -11 has 1 draft tour
        result.First().Status.ShouldBe(0); // Draft
        result.First().Name.ShouldBe("Nature Walk");
    }

    [Fact]
    public void Retrieves_published_tours_by_author()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act - Status 1 = Published
        var result = ((ObjectResult)controller.GetByAuthorAndStatus(1).Result)?.Value as List<TourDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1); // Author -11 has 1 published tour
        result.First().Status.ShouldBe(1); // Published
        result.First().Name.ShouldBe("Art Gallery Tour");
    }

    [Fact]
    public void Retrieves_specific_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.Get(-2).Result)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-2);
        result.Name.ShouldBe("Art Gallery Tour");
        result.AuthorId.ShouldBe(-11);
        result.Status.ShouldBe(1); // Published
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}