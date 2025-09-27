namespace Explorer.BuildingBlocks.Core.Events;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
    void Subscribe<T>(Func<T, Task> handler) where T : class;
}