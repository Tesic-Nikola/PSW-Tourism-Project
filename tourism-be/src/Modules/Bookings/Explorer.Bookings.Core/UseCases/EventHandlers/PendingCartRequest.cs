using Explorer.Bookings.API.Dtos;

namespace Explorer.Bookings.Core.UseCases.EventHandlers;

public class PendingCartRequest
{
    public long TouristId { get; set; }
    public AddToCartDto AddToCartDto { get; set; }
    public TaskCompletionSource<ShoppingCartDto> CompletionSource { get; set; }
    public DateTime CreatedAt { get; set; }

    public PendingCartRequest(long touristId, AddToCartDto addToCartDto)
    {
        TouristId = touristId;
        AddToCartDto = addToCartDto;
        CompletionSource = new TaskCompletionSource<ShoppingCartDto>();
        CreatedAt = DateTime.UtcNow;
    }
}