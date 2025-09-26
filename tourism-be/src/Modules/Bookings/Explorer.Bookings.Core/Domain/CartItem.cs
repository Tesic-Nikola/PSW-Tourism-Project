using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Bookings.Core.Domain;

public class CartItem : Entity
{
    public long TourId { get; private set; }
    public string TourName { get; private set; }
    public decimal TourPrice { get; private set; }
    public DateTime TourStartDate { get; private set; }
    public int Quantity { get; private set; }

    public CartItem(long tourId, string tourName, decimal tourPrice, DateTime tourStartDate, int quantity)
    {
        if (tourId == 0) throw new ArgumentException("Invalid TourId");
        if (string.IsNullOrWhiteSpace(tourName)) throw new ArgumentException("Invalid TourName");
        if (tourPrice < 0) throw new ArgumentException("Invalid TourPrice");
        if (tourStartDate <= DateTime.Now) throw new ArgumentException("Tour start date must be in the future");
        if (quantity <= 0) throw new ArgumentException("Invalid Quantity");

        TourId = tourId;
        TourName = tourName;
        TourPrice = tourPrice;
        TourStartDate = tourStartDate;
        Quantity = quantity;
    }

    // Private constructor for EF
    private CartItem() { }
}