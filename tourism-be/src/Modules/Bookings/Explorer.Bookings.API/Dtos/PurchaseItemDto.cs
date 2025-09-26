namespace Explorer.Bookings.API.Dtos;

public class PurchaseItemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public string TourName { get; set; }
    public decimal TourPrice { get; set; }
    public DateTime TourStartDate { get; set; }
    public int Quantity { get; set; }
}