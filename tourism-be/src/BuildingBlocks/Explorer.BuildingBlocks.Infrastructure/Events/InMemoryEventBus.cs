using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.Logging;

namespace Explorer.BuildingBlocks.Infrastructure.Events;

public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Func<object, Task>>> _handlers;
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _handlers = new Dictionary<Type, List<Func<object, Task>>>();
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event) where T : class
    {
        var eventType = typeof(T);

        if (!_handlers.ContainsKey(eventType))
        {
            _logger.LogWarning("No handlers registered for event type {EventType}", eventType.Name);
            return;
        }

        var handlers = _handlers[eventType];
        var tasks = handlers.Select(handler => HandleEventSafely(handler, @event));

        await Task.WhenAll(tasks);
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : class
    {
        var eventType = typeof(T);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Func<object, Task>>();
        }

        _handlers[eventType].Add(@event => handler((T)@event));
        _logger.LogInformation("Handler registered for event type {EventType}", eventType.Name);
    }

    private async Task HandleEventSafely(Func<object, Task> handler, object @event)
    {
        try
        {
            await handler(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event {EventType}", @event.GetType().Name);
        }
    }
}