namespace Explorer.Bookings.API.Dtos;

public class AddToCartDto
{
    public long TourId { get; set; }
    public int Quantity { get; set; } = 1;
}