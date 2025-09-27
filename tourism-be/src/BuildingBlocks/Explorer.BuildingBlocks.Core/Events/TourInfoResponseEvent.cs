namespace Explorer.BuildingBlocks.Core.Events;

public class TourInfoResponseEvent : BaseEvent
{
    public Guid RequestId { get; }
    public long TourId { get; }
    public long TouristId { get; }
    public int Quantity { get; }
    public string TourName { get; }
    public decimal TourPrice { get; }
    public DateTime TourStartDate { get; }
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    public TourInfoResponseEvent(Guid requestId, long tourId, long touristId, int quantity,
        string tourName, decimal tourPrice, DateTime tourStartDate)
    {
        RequestId = requestId;
        TourId = tourId;
        TouristId = touristId;
        Quantity = quantity;
        TourName = tourName;
        TourPrice = tourPrice;
        TourStartDate = tourStartDate;
        IsSuccess = true;
        ErrorMessage = string.Empty;
    }

    public TourInfoResponseEvent(Guid requestId, long tourId, long touristId, string errorMessage)
    {
        RequestId = requestId;
        TourId = tourId;
        TouristId = touristId;
        IsSuccess = false;
        ErrorMessage = errorMessage;
        TourName = string.Empty;
        TourPrice = 0;
        TourStartDate = DateTime.MinValue;
        Quantity = 0;
    }
}