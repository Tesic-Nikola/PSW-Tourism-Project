using Explorer.Bookings.API.Dtos;
using FluentResults;

namespace Explorer.Bookings.API.Public;

public interface IShoppingCartService
{
    Result<ShoppingCartDto> GetCart(long touristId);
    Result<ShoppingCartDto> AddToCart(long touristId, AddToCartDto addToCartDto);
    Result<ShoppingCartDto> RemoveFromCart(long touristId, long tourId);
    Result<ShoppingCartDto> UpdateCartItemQuantity(long touristId, long tourId, int quantity);
    Result ClearCart(long touristId);
}