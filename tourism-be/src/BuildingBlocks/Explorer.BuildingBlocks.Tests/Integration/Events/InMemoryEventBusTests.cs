using Explorer.BuildingBlocks.Core.Events;
using Explorer.BuildingBlocks.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace Explorer.BuildingBlocks.Tests.Integration.Events;

[Collection("Sequential")]
public class InMemoryEventBusTests
{
    [Fact]
    public async Task PublishEvent_WithHandlers()
    {
        // Arrange
        var logger = NullLogger<InMemoryEventBus>.Instance;
        var eventBus = new InMemoryEventBus(logger);

        var receivedEvents = new List<TestEvent>();
        eventBus.Subscribe<TestEvent>(async (evt) => {
            receivedEvents.Add(evt);
            await Task.CompletedTask;
        });

        var testEvent = new TestEvent("Test Message");

        // Act
        await eventBus.PublishAsync(testEvent);

        // Assert
        receivedEvents.Count.ShouldBe(1);
        receivedEvents.First().Message.ShouldBe("Test Message");
    }

    [Fact]
    public async Task EventHandler_ExceptionHandling()
    {
        // Arrange
        var logger = NullLogger<InMemoryEventBus>.Instance;
        var eventBus = new InMemoryEventBus(logger);

        var handlerExecuted = false;
        eventBus.Subscribe<TestEvent>(async (evt) => {
            handlerExecuted = true;
            throw new InvalidOperationException("Test exception");
        });

        var testEvent = new TestEvent("Test Message");

        // Act & Assert - Should not throw exception
        await eventBus.PublishAsync(testEvent);

        handlerExecuted.ShouldBeTrue();
    }

    private class TestEvent : BaseEvent
    {
        public string Message { get; }

        public TestEvent(string message)
        {
            Message = message;
        }
    }
}