using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using FluentResults;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class KeyPointService : BaseService<KeyPointDto, KeyPoint>, IKeyPointService
{
    private readonly IKeyPointRepository _keyPointRepository;

    public KeyPointService(IKeyPointRepository keyPointRepository, IMapper mapper) : base(mapper)
    {
        _keyPointRepository = keyPointRepository;
    }

    public Result<KeyPointDto> Create(KeyPointDto keyPointDto)
    {
        try
        {
            var keyPoint = MapToDomain(keyPointDto);
            var result = _keyPointRepository.Create(keyPoint);
            return MapToDto(result);
        }
        catch (ArgumentException e)
        {
            return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
        }
    }

    public Result<KeyPointDto> Update(KeyPointDto keyPointDto)
    {
        try
        {
            var keyPoint = MapToDomain(keyPointDto);
            var result = _keyPointRepository.Update(keyPoint);
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

    public Result<KeyPointDto> Get(long id)
    {
        try
        {
            var keyPoint = _keyPointRepository.Get(id);
            return MapToDto(keyPoint);
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
    }

    public Result<List<KeyPointDto>> GetByTour(long tourId)
    {
        var keyPoints = _keyPointRepository.GetByTour(tourId);
        return MapToDto(Result.Ok(keyPoints));
    }

    public Result Delete(long id)
    {
        try
        {
            _keyPointRepository.Delete(id);
            return Result.Ok();
        }
        catch (KeyNotFoundException e)
        {
            return Result.Fail(FailureCode.NotFound).WithError(e.Message);
        }
    }
}