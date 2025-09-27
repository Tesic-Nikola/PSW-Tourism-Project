namespace Explorer.BuildingBlocks.Core.Events;

public class TouristInfoRequestedEvent : BaseEvent
{
    public long TouristId { get; }
    public Guid RequestId { get; }

    public TouristInfoRequestedEvent(long touristId)
    {
        TouristId = touristId;
        RequestId = Guid.NewGuid();
    }
}