using Explorer.Bookings.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;

namespace Explorer.Bookings.API.Public;

public interface ITourPurchaseService
{
    Result<TourPurchaseDto> Checkout(long touristId, CheckoutDto checkoutDto);
    Result<PagedResult<TourPurchaseDto>> GetPurchaseHistory(long touristId, int page, int pageSize);
    Result<TourPurchaseDto> GetPurchase(long purchaseId);
    Result<List<TourPurchaseDto>> GetPurchasesForTour(long tourId);
}