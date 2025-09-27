using Explorer.BuildingBlocks.Core.Events;
using Explorer.Tours.Core.UseCases.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.EventHandlers;

[Collection("Sequential")]
public class TourInfoEventHandlerTests : BaseToursIntegrationTest
{
    public TourInfoEventHandlerTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public async Task HandleTourInfoRequest_ValidTour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TourInfoRequestedEvent>>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var requestEvent = new TourInfoRequestedEvent(-2, -21, 1); // Art Gallery Tour exists

        // Track response event
        TourInfoResponseEvent? responseEvent = null;
        eventBus.Subscribe<TourInfoResponseEvent>(async (evt) => {
            responseEvent = evt;
            await Task.CompletedTask;
        });

        // Act
        await handler.HandleAsync(requestEvent);

        // Assert
        responseEvent.ShouldNotBeNull();
        responseEvent.IsSuccess.ShouldBeTrue();
        responseEvent.TourId.ShouldBe(-2);
        responseEvent.TourName.ShouldBe("Art Gallery Tour");
        responseEvent.TourPrice.ShouldBe(35.00m);
    }

    [Fact]
    public async Task HandleTourInfoRequest_InvalidTour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TourInfoRequestedEvent>>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var requestEvent = new TourInfoRequestedEvent(-999, -21, 1); // Non-existent tour

        // Track response event
        TourInfoResponseEvent? responseEvent = null;
        eventBus.Subscribe<TourInfoResponseEvent>(async (evt) => {
            responseEvent = evt;
            await Task.CompletedTask;
        });

        // Act
        await handler.HandleAsync(requestEvent);

        // Assert
        responseEvent.ShouldNotBeNull();
        responseEvent.IsSuccess.ShouldBeFalse();
        responseEvent.ErrorMessage.ShouldContain("Tour not found");
    }
}