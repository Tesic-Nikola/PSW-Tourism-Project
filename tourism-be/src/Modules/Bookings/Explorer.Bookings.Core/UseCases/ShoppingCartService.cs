using AutoMapper;
using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.Bookings.Core.UseCases.EventHandlers;
using Explorer.BuildingBlocks.Core.Events;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;

namespace Explorer.Bookings.Core.UseCases;

public class ShoppingCartService : BaseService<ShoppingCartDto, ShoppingCart>, IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IEventBus _eventBus;

    public ShoppingCartService(IShoppingCartRepository cartRepository, IEventBus eventBus, IMapper mapper) : base(mapper)
    {
        _cartRepository = cartRepository;
        _eventBus = eventBus;
    }

    public Result<ShoppingCartDto> GetCart(long touristId)
    {
        try
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                // Create new cart if it doesn't exist
                cart = new ShoppingCart(touristId);
                cart = _cartRepository.Create(cart);
            }
            return MapToDto(cart);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public async Task<Result<ShoppingCartDto>> AddToCart(long touristId, AddToCartDto addToCartDto)
    {
        try
        {
            // Create pending request
            var pendingRequest = new PendingCartRequest(touristId, addToCartDto);

            // Send event to get tour information
            var tourInfoEvent = new TourInfoRequestedEvent(addToCartDto.TourId, touristId, addToCartDto.Quantity);
            TourInfoResponseEventHandler.AddPendingRequest(tourInfoEvent.RequestId, pendingRequest);

            await _eventBus.PublishAsync(tourInfoEvent);

            // Wait for response with timeout
            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var cartDto = await pendingRequest.CompletionSource.Task.WaitAsync(cancellationTokenSource.Token);

            return Result.Ok(cartDto);
        }
        catch (OperationCanceledException)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError("Request timeout - tour information not available");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<ShoppingCartDto> RemoveFromCart(long touristId, long tourId)
    {
        try
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
                return Result.Fail(FailureCode.NotFound).WithError("Shopping cart not found");

            var item = cart.Items.FirstOrDefault(i => i.TourId == tourId);
            if (item != null)
            {
                cart.Items.Remove(item);
                cart.MarkAsUpdated();
                cart = _cartRepository.Update(cart);
            }

            return MapToDto(cart);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<ShoppingCartDto> UpdateCartItemQuantity(long touristId, long tourId, int quantity)
    {
        try
        {
            if (quantity <= 0)
                return Result.Fail(FailureCode.InvalidArgument).WithError("Quantity must be greater than 0");

            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
                return Result.Fail(FailureCode.NotFound).WithError("Shopping cart not found");

            var existingItem = cart.Items.FirstOrDefault(i => i.TourId == tourId);
            if (existingItem == null)
                return Result.Fail(FailureCode.NotFound).WithError("Tour not found in cart");

            // Remove old item and add new one with updated quantity
            cart.Items.Remove(existingItem);
            var updatedItem = new CartItem(existingItem.TourId, existingItem.TourName,
                existingItem.TourPrice, existingItem.TourStartDate, quantity);
            cart.Items.Add(updatedItem);

            cart.MarkAsUpdated();
            cart = _cartRepository.Update(cart);
            return MapToDto(cart);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result ClearCart(long touristId)
    {
        try
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
                return Result.Ok(); // Cart doesn't exist, nothing to clear

            cart.Items.Clear();
            cart.MarkAsUpdated();
            _cartRepository.Update(cart);
            return Result.Ok();
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }
}