namespace Explorer.BuildingBlocks.Core.Events;

public class TourInfoRequestedEvent : BaseEvent
{
    public long TourId { get; }
    public long TouristId { get; }
    public int Quantity { get; }
    public Guid RequestId { get; }

    public TourInfoRequestedEvent(long tourId, long touristId, int quantity)
    {
        TourId = tourId;
        TouristId = touristId;
        Quantity = quantity;
        RequestId = Guid.NewGuid();
    }
}