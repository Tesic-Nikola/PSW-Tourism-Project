using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Bookings.Infrastructure.Database.Repositories;

public class TourPurchaseDatabaseRepository : ITourPurchaseRepository
{
    private readonly BookingsContext _dbContext;

    public TourPurchaseDatabaseRepository(BookingsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TourPurchase Create(TourPurchase purchase)
    {
        _dbContext.TourPurchases.Add(purchase);
        _dbContext.SaveChanges();
        return purchase;
    }

    public TourPurchase? Get(long id)
    {
        return _dbContext.TourPurchases
            .Include(tp => tp.Items)
            .FirstOrDefault(tp => tp.Id == id);
    }

    public PagedResult<TourPurchase> GetByTourist(long touristId, int page, int pageSize)
    {
        var query = _dbContext.TourPurchases
            .Include(tp => tp.Items)
            .Where(tp => tp.TouristId == touristId)
            .OrderByDescending(tp => tp.PurchaseDate);

        var task = query.GetPaged(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public List<TourPurchase> GetPurchasesForTour(long tourId)
    {
        return _dbContext.TourPurchases
            .Include(tp => tp.Items)
            .Where(tp => tp.Items.Any(item => item.TourId == tourId))
            .ToList();
    }

    public TourPurchase Update(TourPurchase purchase)
    {
        try
        {
            _dbContext.Update(purchase);
            _dbContext.SaveChanges();
            return purchase;
        }
        catch (Exception)
        {
            throw new KeyNotFoundException("Tour purchase not found");
        }
    }
}