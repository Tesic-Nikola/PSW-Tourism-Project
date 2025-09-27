using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.Core.UseCases.EventHandlers;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Bookings.Tests.Integration.EventHandlers;

[Collection("Sequential")]
public class TouristInfoResponseEventHandlerTests : BaseBookingsIntegrationTest
{
    public TouristInfoResponseEventHandlerTests(BookingsTestFactory factory) : base(factory) { }

    [Fact]
    public async Task HandleTouristInfoResponse_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TouristInfoResponseEvent>>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var requestId = Guid.NewGuid();
        var purchaseDto = new TourPurchaseDto
        {
            Id = 1,
            TouristId = -21,
            Items = new List<PurchaseItemDto>
            {
                new() { TourName = "Test Tour", TourPrice = 25.00m, Quantity = 1 }
            }
        };
        var pendingRequest = new PendingCheckoutRequest(-21, new CheckoutDto(), purchaseDto);
        TouristInfoResponseEventHandler.AddPendingRequest(requestId, pendingRequest);

        var successEvent = new TouristInfoResponseEvent(
            requestId, -21, "test@example.com", "John", "Doe");

        // Track if email event was published
        bool emailEventPublished = false;
        eventBus.Subscribe<EmailRequestedEvent>(async (emailEvent) => {
            emailEventPublished = true;
            await Task.CompletedTask;
        });

        // Act
        await handler.HandleAsync(successEvent);

        // Assert
        emailEventPublished.ShouldBeTrue();
        var completionTask = pendingRequest.CompletionSource.Task;
        completionTask.IsCompletedSuccessfully.ShouldBeTrue();

        var result = await completionTask;
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }

    [Fact]
    public async Task HandleTouristInfoResponse_TouristNotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TouristInfoResponseEvent>>();

        var requestId = Guid.NewGuid();
        var purchaseDto = new TourPurchaseDto { Id = 1, TouristId = -999 };
        var pendingRequest = new PendingCheckoutRequest(-999, new CheckoutDto(), purchaseDto);
        TouristInfoResponseEventHandler.AddPendingRequest(requestId, pendingRequest);

        var errorEvent = new TouristInfoResponseEvent(requestId, -999, "Tourist not found");

        // Act
        await handler.HandleAsync(errorEvent);

        // Assert - Purchase should still complete even without email
        var completionTask = pendingRequest.CompletionSource.Task;
        completionTask.IsCompletedSuccessfully.ShouldBeTrue();

        var result = await completionTask;
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
    }
}