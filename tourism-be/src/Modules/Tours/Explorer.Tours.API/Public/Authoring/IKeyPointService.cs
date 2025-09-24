using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using FluentResults;

namespace Explorer.Tours.API.Public.Authoring;

public interface IKeyPointService
{
    Result<KeyPointDto> Create(KeyPointDto keyPoint);
    Result<KeyPointDto> Update(KeyPointDto keyPoint);
    Result<KeyPointDto> Get(long id);
    Result<List<KeyPointDto>> GetByTour(long tourId);
    Result Delete(long id);
}