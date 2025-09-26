using Explorer.Bookings.Core.Domain;

namespace Explorer.Bookings.Core.Domain.RepositoryInterfaces;

public interface IBonusPointsRepository
{
    BonusPoints? GetByTouristId(long touristId);
    BonusPoints Create(BonusPoints bonusPoints);
    BonusPoints Update(BonusPoints bonusPoints);
}