namespace Explorer.Bookings.API.Dtos;

public class BonusPointsDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public decimal AvailablePoints { get; set; }
    public DateTime LastUpdated { get; set; }
}