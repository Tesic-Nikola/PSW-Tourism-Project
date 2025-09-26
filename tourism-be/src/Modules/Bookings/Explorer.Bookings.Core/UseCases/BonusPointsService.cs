using AutoMapper;
using Explorer.Bookings.API.Dtos;
using Explorer.Bookings.API.Public;
using Explorer.Bookings.Core.Domain;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using FluentResults;

namespace Explorer.Bookings.Core.UseCases;

public class BonusPointsService : BaseService<BonusPointsDto, BonusPoints>, IBonusPointsService
{
    private readonly IBonusPointsRepository _bonusPointsRepository;

    public BonusPointsService(IBonusPointsRepository bonusPointsRepository, IMapper mapper) : base(mapper)
    {
        _bonusPointsRepository = bonusPointsRepository;
    }

    public Result<BonusPointsDto> GetBonusPoints(long touristId)
    {
        try
        {
            var bonusPoints = _bonusPointsRepository.GetByTouristId(touristId);
            if (bonusPoints == null)
            {
                // Create new bonus points record if it doesn't exist
                bonusPoints = new BonusPoints(touristId);
                bonusPoints = _bonusPointsRepository.Create(bonusPoints);
            }
            return MapToDto(bonusPoints);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<BonusPointsDto> AwardBonusPoints(long touristId, decimal points, string reason)
    {
        try
        {
            if (points <= 0)
                return Result.Fail(FailureCode.InvalidArgument).WithError("Points to award must be greater than 0");
            if (string.IsNullOrWhiteSpace(reason))
                return Result.Fail(FailureCode.InvalidArgument).WithError("Reason is required");

            var bonusPoints = _bonusPointsRepository.GetByTouristId(touristId);
            if (bonusPoints == null)
            {
                bonusPoints = new BonusPoints(touristId);
                bonusPoints = _bonusPointsRepository.Create(bonusPoints);
            }

            var updatedBonusPoints = new BonusPoints(bonusPoints.TouristId);
            // Set the ID to maintain entity identity
            typeof(BonusPoints).GetProperty("Id")?.SetValue(updatedBonusPoints, bonusPoints.Id);
            // Update available points
            var availablePointsField = typeof(BonusPoints).GetField("AvailablePoints",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            availablePointsField?.SetValue(updatedBonusPoints, bonusPoints.AvailablePoints + points);
            // Update timestamp
            var lastUpdatedField = typeof(BonusPoints).GetField("LastUpdated",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            lastUpdatedField?.SetValue(updatedBonusPoints, DateTime.UtcNow);

            updatedBonusPoints = _bonusPointsRepository.Update(updatedBonusPoints);
            return MapToDto(updatedBonusPoints);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<BonusPointsDto> UseBonusPoints(long touristId, decimal points)
    {
        try
        {
            if (points <= 0)
                return Result.Fail(FailureCode.InvalidArgument).WithError("Points to use must be greater than 0");

            var bonusPoints = _bonusPointsRepository.GetByTouristId(touristId);
            if (bonusPoints == null)
                return Result.Fail(FailureCode.NotFound).WithError("Bonus points record not found");

            if (!bonusPoints.HasSufficientPoints(points))
                return Result.Fail(FailureCode.InvalidArgument).WithError("Insufficient bonus points");

            var updatedBonusPoints = new BonusPoints(bonusPoints.TouristId);
            // Set the ID to maintain entity identity
            typeof(BonusPoints).GetProperty("Id")?.SetValue(updatedBonusPoints, bonusPoints.Id);
            // Update available points
            var availablePointsField = typeof(BonusPoints).GetField("AvailablePoints",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            availablePointsField?.SetValue(updatedBonusPoints, bonusPoints.AvailablePoints - points);
            // Update timestamp
            var lastUpdatedField = typeof(BonusPoints).GetField("LastUpdated",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            lastUpdatedField?.SetValue(updatedBonusPoints, DateTime.UtcNow);

            updatedBonusPoints = _bonusPointsRepository.Update(updatedBonusPoints);
            return MapToDto(updatedBonusPoints);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }
}