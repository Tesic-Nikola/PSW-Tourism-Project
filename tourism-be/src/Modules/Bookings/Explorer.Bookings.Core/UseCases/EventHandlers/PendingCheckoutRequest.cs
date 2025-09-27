using Explorer.Bookings.API.Dtos;

namespace Explorer.Bookings.Core.UseCases.EventHandlers;

public class PendingCheckoutRequest
{
    public long TouristId { get; set; }
    public CheckoutDto CheckoutDto { get; set; }
    public TourPurchaseDto PurchaseDto { get; set; }
    public TaskCompletionSource<TourPurchaseDto> CompletionSource { get; set; }
    public DateTime CreatedAt { get; set; }

    public PendingCheckoutRequest(long touristId, CheckoutDto checkoutDto, TourPurchaseDto purchaseDto)
    {
        TouristId = touristId;
        CheckoutDto = checkoutDto;
        PurchaseDto = purchaseDto;
        CompletionSource = new TaskCompletionSource<TourPurchaseDto>();
        CreatedAt = DateTime.UtcNow;
    }
}