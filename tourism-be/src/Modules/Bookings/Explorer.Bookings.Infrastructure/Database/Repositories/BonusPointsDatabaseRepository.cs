using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;

namespace Explorer.Bookings.Infrastructure.Database.Repositories;

public class BonusPointsDatabaseRepository : IBonusPointsRepository
{
    private readonly BookingsContext _dbContext;

    public BonusPointsDatabaseRepository(BookingsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BonusPoints? GetByTouristId(long touristId)
    {
        return _dbContext.BonusPoints
            .FirstOrDefault(bp => bp.TouristId == touristId);
    }

    public BonusPoints Create(BonusPoints bonusPoints)
    {
        _dbContext.BonusPoints.Add(bonusPoints);
        _dbContext.SaveChanges();
        return bonusPoints;
    }

    public BonusPoints Update(BonusPoints bonusPoints)
    {
        try
        {
            _dbContext.Update(bonusPoints);
            _dbContext.SaveChanges();
            return bonusPoints;
        }
        catch (Exception)
        {
            throw new KeyNotFoundException("Bonus points record not found");
        }
    }
}