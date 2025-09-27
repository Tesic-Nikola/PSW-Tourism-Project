using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Bookings.Tests.Integration.TourPurchase;

[Collection("Sequential")]
public class TourPurchaseCommandTests : BaseBookingsIntegrationTest
{
    public TourPurchaseCommandTests(BookingsTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Checkout_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();
        var checkoutDto = new CheckoutDto
        {
            BonusPointsToUse = 0
        };

        // Simulate tourist info response for email
        SimulateTouristInfoResponse(scope, -21);

        // Act
        var result = await service.Checkout(-21, checkoutDto);

        // Assert - Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.TouristId.ShouldBe(-21);
        result.Value.Items.Count.ShouldBeGreaterThan(0);
        result.Value.Status.ShouldBe("Completed");

        // Assert - Database
        var storedPurchase = dbContext.TourPurchases.FirstOrDefault(tp => tp.Id == result.Value.Id);
        storedPurchase.ShouldNotBeNull();

        // Assert - Cart should be empty after checkout
        var cart = dbContext.ShoppingCarts.FirstOrDefault(sc => sc.TouristId == -21);
        cart?.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Checkout_WithBonusPoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();
        var checkoutDto = new CheckoutDto
        {
            BonusPointsToUse = 10.00m
        };

        SimulateTouristInfoResponse(scope, -21);

        // Act
        var result = await service.Checkout(-21, checkoutDto);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.BonusPointsUsed.ShouldBe(10.00m);
        result.Value.FinalPrice.ShouldBe(result.Value.TotalPrice - 10.00m);

        // Assert - Bonus points reduced in database
        var bonusPoints = dbContext.BonusPoints.FirstOrDefault(bp => bp.TouristId == -21);
        bonusPoints.ShouldNotBeNull();
        bonusPoints.AvailablePoints.ShouldBe(90.00m); // Started with 100, used 10
    }

    [Fact]
    public async Task Checkout_InsufficientBonusPoints()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseService>();
        var checkoutDto = new CheckoutDto
        {
            BonusPointsToUse = 200.00m // More than available (100)
        };

        // Act
        var result = await service.Checkout(-21, checkoutDto);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldContain("Insufficient bonus points");
    }

    [Fact]
    public async Task Checkout_EmptyCart()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourPurchaseService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();

        // Clear the cart first
        var cart = dbContext.ShoppingCarts.FirstOrDefault(sc => sc.TouristId == -21);
        if (cart != null)
        {
            cart.Items.Clear();
            dbContext.SaveChanges();
        }

        var checkoutDto = new CheckoutDto { BonusPointsToUse = 0 };

        // Act
        var result = await service.Checkout(-21, checkoutDto);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldContain("Shopping cart is empty");
    }

    private void SimulateTouristInfoResponse(IServiceScope scope, long touristId)
    {
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var responseEvent = new TouristInfoResponseEvent(
            Guid.NewGuid(), touristId, "tourist@test.com", "Test", "Tourist");
        Task.Run(async () => await eventBus.PublishAsync(responseEvent));
    }
}