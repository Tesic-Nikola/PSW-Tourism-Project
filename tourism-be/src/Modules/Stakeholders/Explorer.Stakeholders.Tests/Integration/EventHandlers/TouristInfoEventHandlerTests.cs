using Explorer.BuildingBlocks.Core.Events;
using Explorer.Stakeholders.Core.UseCases.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.EventHandlers;

[Collection("Sequential")]
public class TouristInfoEventHandlerTests : BaseStakeholdersIntegrationTest
{
    public TouristInfoEventHandlerTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public async Task HandleTouristInfoRequest_ValidTourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TouristInfoRequestedEvent>>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var requestEvent = new TouristInfoRequestedEvent(-21); // Tourist exists

        // Track response event
        TouristInfoResponseEvent? responseEvent = null;
        eventBus.Subscribe<TouristInfoResponseEvent>(async (evt) => {
            responseEvent = evt;
            await Task.CompletedTask;
        });

        // Act
        await handler.HandleAsync(requestEvent);

        // Assert
        responseEvent.ShouldNotBeNull();
        responseEvent.IsSuccess.ShouldBeTrue();
        responseEvent.TouristId.ShouldBe(-21);
        responseEvent.Email.ShouldBe("turista1@gmail.com");
        responseEvent.Name.ShouldBe("Pera");
        responseEvent.Surname.ShouldBe("Perić");
    }

    [Fact]
    public async Task HandleTouristInfoRequest_InvalidTourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TouristInfoRequestedEvent>>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var requestEvent = new TouristInfoRequestedEvent(-999); // Non-existent tourist

        // Track response event
        TouristInfoResponseEvent? responseEvent = null;
        eventBus.Subscribe<TouristInfoResponseEvent>(async (evt) => {
            responseEvent = evt;
            await Task.CompletedTask;
        });

        // Act
        await handler.HandleAsync(requestEvent);

        // Assert
        responseEvent.ShouldNotBeNull();
        responseEvent.IsSuccess.ShouldBeFalse();
        responseEvent.ErrorMessage.ShouldContain("Tourist not found");
    }
}