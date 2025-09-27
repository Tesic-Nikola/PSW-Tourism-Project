using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Bookings.Tests.Integration.BonusPoints;

[Collection("Sequential")]
public class BonusPointsCommandTests : BaseBookingsIntegrationTest
{
    public BonusPointsCommandTests(BookingsTestFactory factory) : base(factory) { }

    [Fact]
    public void AwardBonusPoints_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBonusPointsService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();

        // Act
        var result = service.AwardBonusPoints(-21, 25.50m, "Tour cancellation compensation");

        // Assert - Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.AvailablePoints.ShouldBe(125.50m); // 100 + 25.50

        // Assert - Database
        var storedBonusPoints = dbContext.BonusPoints.FirstOrDefault(bp => bp.TouristId == -21);
        storedBonusPoints.ShouldNotBeNull();
        storedBonusPoints.AvailablePoints.ShouldBe(125.50m);
    }

    [Fact]
    public void UseBonusPoints_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBonusPointsService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();

        // Act
        var result = service.UseBonusPoints(-21, 30.00m);

        // Assert - Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.AvailablePoints.ShouldBe(70.00m); // 100 - 30

        // Assert - Database
        var storedBonusPoints = dbContext.BonusPoints.FirstOrDefault(bp => bp.TouristId == -21);
        storedBonusPoints.ShouldNotBeNull();
        storedBonusPoints.AvailablePoints.ShouldBe(70.00m);
    }

    [Fact]
    public void UseBonusPoints_InsufficientPoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBonusPointsService>();

        // Act - Try to use more points than available (100)
        var result = service.UseBonusPoints(-21, 150.00m);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldContain("Insufficient bonus points");
    }
}