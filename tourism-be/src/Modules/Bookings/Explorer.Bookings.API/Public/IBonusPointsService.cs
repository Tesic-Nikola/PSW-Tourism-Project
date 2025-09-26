using Explorer.Bookings.API.Dtos;
using FluentResults;

namespace Explorer.Bookings.API.Public;

public interface IBonusPointsService
{
    Result<BonusPointsDto> GetBonusPoints(long touristId);
    Result<BonusPointsDto> AwardBonusPoints(long touristId, decimal points, string reason);
    Result<BonusPointsDto> UseBonusPoints(long touristId, decimal points);
}