using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Bookings.Core.Domain;

public class BonusPoints : Entity
{
    public long TouristId { get; private set; }
    public decimal AvailablePoints { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public BonusPoints(long touristId)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId");

        TouristId = touristId;
        AvailablePoints = 0;
        LastUpdated = DateTime.UtcNow;
    }

    // Private constructor for EF
    private BonusPoints() { }

    public bool HasSufficientPoints(decimal points)
    {
        return AvailablePoints >= points;
    }
}