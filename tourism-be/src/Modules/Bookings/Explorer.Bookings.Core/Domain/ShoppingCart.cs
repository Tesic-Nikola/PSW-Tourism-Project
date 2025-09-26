using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Bookings.Core.Domain;

public class ShoppingCart : Entity
{
    public long TouristId { get; private set; }
    public List<CartItem> Items { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ShoppingCart(long touristId)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId");

        TouristId = touristId;
        Items = new List<CartItem>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private constructor for EF
    private ShoppingCart() { }

    public decimal GetTotalPrice()
    {
        return Items.Sum(i => i.TourPrice * i.Quantity);
    }

    public bool IsEmpty()
    {
        return !Items.Any();
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}