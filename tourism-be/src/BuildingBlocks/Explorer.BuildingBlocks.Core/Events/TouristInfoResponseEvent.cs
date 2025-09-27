namespace Explorer.BuildingBlocks.Core.Events;

public class TouristInfoResponseEvent : BaseEvent
{
    public Guid RequestId { get; }
    public long TouristId { get; }
    public string Email { get; }
    public string Name { get; }
    public string Surname { get; }
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    public TouristInfoResponseEvent(Guid requestId, long touristId, string email, string name, string surname)
    {
        RequestId = requestId;
        TouristId = touristId;
        Email = email;
        Name = name;
        Surname = surname;
        IsSuccess = true;
        ErrorMessage = string.Empty;
    }

    public TouristInfoResponseEvent(Guid requestId, long touristId, string errorMessage)
    {
        RequestId = requestId;
        TouristId = touristId;
        IsSuccess = false;
        ErrorMessage = errorMessage;
        Email = string.Empty;
        Name = string.Empty;
        Surname = string.Empty;
    }
}