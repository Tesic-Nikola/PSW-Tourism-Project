using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Bookings.Infrastructure.Database.Repositories;

public class ShoppingCartDatabaseRepository : IShoppingCartRepository
{
    private readonly BookingsContext _dbContext;

    public ShoppingCartDatabaseRepository(BookingsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ShoppingCart? GetByTouristId(long touristId)
    {
        return _dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .FirstOrDefault(sc => sc.TouristId == touristId);
    }

    public ShoppingCart Create(ShoppingCart cart)
    {
        _dbContext.ShoppingCarts.Add(cart);
        _dbContext.SaveChanges();
        return cart;
    }

    public ShoppingCart Update(ShoppingCart cart)
    {
        try
        {
            _dbContext.Update(cart);
            _dbContext.SaveChanges();
            return cart;
        }
        catch (Exception)
        {
            throw new KeyNotFoundException("Shopping cart not found");
        }
    }

    public void Delete(long touristId)
    {
        var cart = GetByTouristId(touristId);
        if (cart != null)
        {
            _dbContext.ShoppingCarts.Remove(cart);
            _dbContext.SaveChanges();
        }
    }
}