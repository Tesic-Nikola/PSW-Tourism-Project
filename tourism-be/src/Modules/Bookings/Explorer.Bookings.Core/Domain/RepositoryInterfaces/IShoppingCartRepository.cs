using Explorer.Bookings.Core.Domain;

namespace Explorer.Bookings.Core.Domain.RepositoryInterfaces;

public interface IShoppingCartRepository
{
    ShoppingCart? GetByTouristId(long touristId);
    ShoppingCart Create(ShoppingCart cart);
    ShoppingCart Update(ShoppingCart cart);
    void Delete(long touristId);
}