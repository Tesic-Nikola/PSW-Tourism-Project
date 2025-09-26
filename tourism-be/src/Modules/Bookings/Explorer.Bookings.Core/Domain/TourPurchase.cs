using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Bookings.Core.Domain;

public class TourPurchase : Entity
{
    public long TouristId { get; private set; }
    public List<PurchaseItem> Items { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal BonusPointsUsed { get; private set; }
    public decimal FinalPrice { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public PurchaseStatus Status { get; private set; }

    public TourPurchase(long touristId, List<PurchaseItem> items, decimal bonusPointsUsed = 0)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId");
        if (items == null || !items.Any()) throw new ArgumentException("Purchase must contain at least one item");
        if (bonusPointsUsed < 0) throw new ArgumentException("Bonus points used cannot be negative");

        TouristId = touristId;
        Items = items;
        TotalPrice = items.Sum(i => i.TourPrice * i.Quantity);
        BonusPointsUsed = bonusPointsUsed;
        FinalPrice = Math.Max(0, TotalPrice - bonusPointsUsed);
        PurchaseDate = DateTime.UtcNow;
        Status = PurchaseStatus.Completed;
    }

    // Private constructor for EF
    private TourPurchase() { }

    public void Cancel()
    {
        if (Status != PurchaseStatus.Completed)
            throw new InvalidOperationException("Only completed purchases can be cancelled");

        Status = PurchaseStatus.Cancelled;
    }
}

public enum PurchaseStatus
{
    Completed,
    Cancelled
}