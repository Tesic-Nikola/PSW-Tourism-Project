using AutoMapper;
using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;

namespace Explorer.Bookings.Core.UseCases;

public class ShoppingCartService : BaseService<ShoppingCartDto, ShoppingCart>, IShoppingCartService
{
    private readonly IShoppingCartRepository _cartRepository;

    public ShoppingCartService(IShoppingCartRepository cartRepository, IMapper mapper) : base(mapper)
    {
        _cartRepository = cartRepository;
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

    public Result<ShoppingCartDto> AddToCart(long touristId, AddToCartDto addToCartDto)
    {
        try
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                cart = _cartRepository.Create(cart);
            }

            // TODO: Get tour information from Tours module via events/messaging
            // For now, we'll need to receive tour data through events when a tour is added to cart
            // This is a placeholder until cross-module communication is implemented

            var existingItem = cart.Items.FirstOrDefault(i => i.TourId == addToCartDto.TourId);
            if (existingItem != null)
            {
                // Update quantity in existing item
                cart.Items.Remove(existingItem);
                var newItem = new CartItem(existingItem.TourId, existingItem.TourName,
                    existingItem.TourPrice, existingItem.TourStartDate,
                    existingItem.Quantity + addToCartDto.Quantity);
                cart.Items.Add(newItem);
            }
            else
            {
                // This will need to be populated from tour data received via events
                throw new InvalidOperationException("Tour information not available. Cross-module communication needed.");
            }

            cart.MarkAsUpdated();
            cart = _cartRepository.Update(cart);
            return MapToDto(cart);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
        catch (InvalidOperationException e)
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