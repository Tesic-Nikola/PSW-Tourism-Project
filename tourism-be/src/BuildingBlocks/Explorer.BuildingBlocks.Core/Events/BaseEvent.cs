namespace Explorer.BuildingBlocks.Core.Events;

public abstract class BaseEvent
{
    public Guid Id { get; }
    public DateTime OccurredAt { get; }

    protected BaseEvent()
    {
        Id = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
    }
}