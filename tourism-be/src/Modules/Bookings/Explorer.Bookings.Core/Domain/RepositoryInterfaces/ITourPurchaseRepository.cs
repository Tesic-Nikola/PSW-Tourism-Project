using Explorer.Bookings.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Bookings.Core.Domain.RepositoryInterfaces;

public interface ITourPurchaseRepository
{
    TourPurchase Create(TourPurchase purchase);
    TourPurchase? Get(long id);
    PagedResult<TourPurchase> GetByTourist(long touristId, int page, int pageSize);
    List<TourPurchase> GetPurchasesForTour(long tourId);
    TourPurchase Update(TourPurchase purchase);
}