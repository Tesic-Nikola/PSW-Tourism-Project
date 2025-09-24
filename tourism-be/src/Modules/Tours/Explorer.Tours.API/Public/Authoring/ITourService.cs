using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using FluentResults;

namespace Explorer.Tours.API.Public.Authoring;

public interface ITourService
{
    Result<TourDto> Create(TourDto tour);
    Result<TourDto> Update(TourDto tour);
    Result<TourDto> Get(long id);
    Result<List<TourDto>> GetByAuthor(long authorId);
    Result<List<TourDto>> GetByAuthorAndStatus(long authorId, int status);
    Result<TourDto> Publish(long id, long authorId);
    Result<TourDto> Cancel(long id, long authorId);
    Result Delete(long id);
}