using AutoMapper;
using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;

namespace Explorer.Bookings.Core.UseCases;

public class TourPurchaseService : BaseService<TourPurchaseDto, TourPurchase>, ITourPurchaseService
{
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IBonusPointsRepository _bonusPointsRepository;

    public TourPurchaseService(ITourPurchaseRepository purchaseRepository,
        IShoppingCartRepository cartRepository,
        IBonusPointsRepository bonusPointsRepository,
        IMapper mapper) : base(mapper)
    {
        _purchaseRepository = purchaseRepository;
        _cartRepository = cartRepository;
        _bonusPointsRepository = bonusPointsRepository;
    }

    public Result<TourPurchaseDto> Checkout(long touristId, CheckoutDto checkoutDto)
    {
        try
        {
            // Get the shopping cart
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null || cart.IsEmpty())
                return Result.Fail(FailureCode.InvalidArgument).WithError("Shopping cart is empty");

            // Validate bonus points if being used
            decimal bonusPointsToUse = checkoutDto.BonusPointsToUse;
            if (bonusPointsToUse > 0)
            {
                var bonusPoints = _bonusPointsRepository.GetByTouristId(touristId);
                if (bonusPoints == null || !bonusPoints.HasSufficientPoints(bonusPointsToUse))
                    return Result.Fail(FailureCode.InvalidArgument).WithError("Insufficient bonus points");
            }

            // Convert cart items to purchase items
            var purchaseItems = cart.Items.Select(item =>
                new PurchaseItem(item.TourId, item.TourName, item.TourPrice, item.TourStartDate, item.Quantity)
            ).ToList();

            // Create the purchase
            var purchase = new TourPurchase(touristId, purchaseItems, bonusPointsToUse);
            purchase = _purchaseRepository.Create(purchase);

            // Use bonus points if any
            if (bonusPointsToUse > 0)
            {
                var bonusPoints = _bonusPointsRepository.GetByTouristId(touristId);
                // Update bonus points using reflection (since we moved logic to service layer)
                var updatedBonusPoints = new BonusPoints(bonusPoints.TouristId);
                typeof(BonusPoints).GetProperty("Id")?.SetValue(updatedBonusPoints, bonusPoints.Id);
                var availablePointsField = typeof(BonusPoints).GetField("AvailablePoints",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                availablePointsField?.SetValue(updatedBonusPoints, bonusPoints.AvailablePoints - bonusPointsToUse);
                var lastUpdatedField = typeof(BonusPoints).GetField("LastUpdated",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                lastUpdatedField?.SetValue(updatedBonusPoints, DateTime.UtcNow);
                _bonusPointsRepository.Update(updatedBonusPoints);
            }

            // Clear the shopping cart
            cart.Items.Clear();
            cart.MarkAsUpdated();
            _cartRepository.Update(cart);

            // TODO: Send email confirmation via events/messaging to BuildingBlocks Email Service
            // Need to get tourist email from Stakeholders module via cross-module communication
            // This will need to be implemented when email service is available

            return MapToDto(purchase);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
        catch (Exception e)
        {
            return Result.Fail(FailureCode.Internal).WithError(e.Message);
        }
    }

    public Result<PagedResult<TourPurchaseDto>> GetPurchaseHistory(long touristId, int page, int pageSize)
    {
        try
        {
            var result = _purchaseRepository.GetByTourist(touristId, page, pageSize);
            return MapToDto(result);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<TourPurchaseDto> GetPurchase(long purchaseId)
    {
        try
        {
            var purchase = _purchaseRepository.Get(purchaseId);
            if (purchase == null)
                return Result.Fail(FailureCode.NotFound).WithError("Purchase not found");

            return MapToDto(purchase);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<List<TourPurchaseDto>> GetPurchasesForTour(long tourId)
    {
        try
        {
            var purchases = _purchaseRepository.GetPurchasesForTour(tourId);
            return MapToDto(Result.Ok(purchases));
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }
}