using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Bookings.Tests.Integration.ShoppingCart;

[Collection("Sequential")]
public class ShoppingCartCommandTests : BaseBookingsIntegrationTest
{
    public ShoppingCartCommandTests(BookingsTestFactory factory) : base(factory) { }

    [Fact]
    public async Task AddToCart_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();
        var addToCartDto = new AddToCartDto
        {
            TourId = -2,
            Quantity = 1
        };

        // Simulate tour info response event
        SimulateTourInfoResponse(scope, -2, "Art Gallery Tour", 35.00m);

        // Act
        var result = await service.AddToCart(-21, addToCartDto);

        // Assert - Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.TouristId.ShouldBe(-21);
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items.First().TourId.ShouldBe(-2);
        result.Value.Items.First().TourName.ShouldBe("Art Gallery Tour");
        result.Value.Items.First().Quantity.ShouldBe(1);

        // Assert - Database
        var storedCart = dbContext.ShoppingCarts.FirstOrDefault(sc => sc.TouristId == -21);
        storedCart.ShouldNotBeNull();
        storedCart.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task AddToCart_TourNotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();
        var addToCartDto = new AddToCartDto
        {
            TourId = -999,
            Quantity = 1
        };

        // Simulate tour not found response
        SimulateTourNotFoundResponse(scope, -999);

        // Act
        var result = await service.AddToCart(-21, addToCartDto);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldContain("Tour not found");
    }

    [Fact]
    public async Task AddToCart_UpdateQuantity()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // First add item
        var addToCartDto1 = new AddToCartDto { TourId = -2, Quantity = 1 };
        SimulateTourInfoResponse(scope, -2, "Art Gallery Tour", 35.00m);
        await service.AddToCart(-21, addToCartDto1);

        // Add same item again
        var addToCartDto2 = new AddToCartDto { TourId = -2, Quantity = 2 };
        SimulateTourInfoResponse(scope, -2, "Art Gallery Tour", 35.00m);

        // Act
        var result = await service.AddToCart(-21, addToCartDto2);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.Items.First().Quantity.ShouldBe(3); // 1 + 2 = 3
    }

    [Fact]
    public void RemoveFromCart_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Act - Remove tour -2 from cart -1 (which has it in test data)
        var result = service.RemoveFromCart(-21, -2);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldNotContain(item => item.TourId == -2);
    }

    [Fact]
    public void ClearCart_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IShoppingCartService>();

        // Act
        var result = service.ClearCart(-21);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    private void SimulateTourInfoResponse(IServiceScope scope, long tourId, string tourName, decimal price)
    {
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var responseEvent = new TourInfoResponseEvent(
            Guid.NewGuid(), tourId, -21, 1, tourName, price, DateTime.UtcNow.AddDays(30));
        Task.Run(async () => await eventBus.PublishAsync(responseEvent));
    }

    private void SimulateTourNotFoundResponse(IServiceScope scope, long tourId)
    {
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var errorEvent = new TourInfoResponseEvent(Guid.NewGuid(), tourId, -21, "Tour not found");
        Task.Run(async () => await eventBus.PublishAsync(errorEvent));
    }
}