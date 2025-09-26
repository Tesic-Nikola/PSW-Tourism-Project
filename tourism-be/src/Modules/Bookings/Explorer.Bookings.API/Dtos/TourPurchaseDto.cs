namespace Explorer.Bookings.API.Dtos;

public class TourPurchaseDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public List<PurchaseItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public decimal BonusPointsUsed { get; set; }
    public decimal FinalPrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; }
}