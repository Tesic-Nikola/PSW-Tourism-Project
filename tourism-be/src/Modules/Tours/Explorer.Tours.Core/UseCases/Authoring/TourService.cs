using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using FluentResults;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : BaseService<TourDto, Tour>, ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IKeyPointRepository _keyPointRepository;

    public TourService(ITourRepository tourRepository, IKeyPointRepository keyPointRepository, IMapper mapper)
        : base(mapper)
    {
        _tourRepository = tourRepository;
        _keyPointRepository = keyPointRepository;
    }

    public Result<TourDto> Create(TourDto tourDto)
    {
        try
        {
            var tour = MapToDomain(tourDto);
            var result = _tourRepository.Create(tour);
            return MapToDto(result);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<TourDto> Update(TourDto tourDto)
    {
        try
        {
            var tour = MapToDomain(tourDto);
            var result = _tourRepository.Update(tour);
            return MapToDto(result);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<TourDto> Get(long id)
    {
        try
        {
            var tour = _tourRepository.Get(id);
            return MapToDto(tour);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
    }

    public Result<List<TourDto>> GetByAuthor(long authorId)
    {
        var tours = _tourRepository.GetByAuthor(authorId);
        return MapToDto(Result.Ok(tours));
    }

    public Result<List<TourDto>> GetByAuthorAndStatus(long authorId, int status)
    {
        var tourStatus = (TourStatus)status;
        var tours = _tourRepository.GetByAuthorAndStatus(authorId, tourStatus);
        return MapToDto(Result.Ok(tours));
    }

    public Result<TourDto> Publish(long id, long authorId)
    {
        try
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                return Result.Fail(FailureCode.Forbidden).WithError("You can only publish your own tours");

            var keyPointCount = _keyPointRepository.CountByTour(id);
            if (keyPointCount < 2)
                return Result.Fail(FailureCode.InvalidArgument).WithError("Tour must have at least 2 key points to be published");

            tour.Publish();
            var result = _tourRepository.Update(tour);
            return MapToDto(result);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<TourDto> Cancel(long id, long authorId)
    {
        try
        {
            var tour = _tourRepository.Get(id);

            if (tour.AuthorId != authorId)
                return Result.Fail(FailureCode.Forbidden).WithError("You can only cancel your own tours");

            tour.Cancel();
            var result = _tourRepository.Update(tour);
            return MapToDto(result);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result Delete(long id)
    {
        try
        {
            _tourRepository.Delete(id);
            return Result.Ok();
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
    }
}