using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.Core.UseCases.EventHandlers;
using Explorer.Bookings.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Bookings.Tests.Integration.EventHandlers;

[Collection("Sequential")]
public class TourInfoResponseEventHandlerTests : BaseBookingsIntegrationTest
{
    public TourInfoResponseEventHandlerTests(BookingsTestFactory factory) : base(factory) { }

    [Fact]
    public async Task HandleTourInfoResponse_Success()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TourInfoResponseEvent>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingsContext>();

        var requestId = Guid.NewGuid();
        var pendingRequest = new PendingCartRequest(-21, new AddToCartDto { TourId = -4, Quantity = 1 });
        TourInfoResponseEventHandler.AddPendingRequest(requestId, pendingRequest);

        var successEvent = new TourInfoResponseEvent(
            requestId, -4, -21, 1, "Shopping District", 15.00m, DateTime.UtcNow.AddDays(10));

        // Act
        await handler.HandleAsync(successEvent);

        // Assert
        var completionTask = pendingRequest.CompletionSource.Task;
        completionTask.IsCompletedSuccessfully.ShouldBeTrue();

        var result = await completionTask;
        result.ShouldNotBeNull();
        result.Items.ShouldContain(item => item.TourId == -4);
    }

    [Fact]
    public async Task HandleTourInfoResponse_TourNotFound()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TourInfoResponseEvent>>();

        var requestId = Guid.NewGuid();
        var pendingRequest = new PendingCartRequest(-21, new AddToCartDto { TourId = -999, Quantity = 1 });
        TourInfoResponseEventHandler.AddPendingRequest(requestId, pendingRequest);

        var errorEvent = new TourInfoResponseEvent(requestId, -999, -21, "Tour not found");

        // Act
        await handler.HandleAsync(errorEvent);

        // Assert
        var completionTask = pendingRequest.CompletionSource.Task;
        completionTask.IsFaulted.ShouldBeTrue();
        completionTask.Exception?.InnerException?.Message.ShouldContain("Tour not found");
    }
}