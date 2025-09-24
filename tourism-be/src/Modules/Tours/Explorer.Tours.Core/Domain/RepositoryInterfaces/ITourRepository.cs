using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    Tour Get(long id);
    List<Tour> GetByAuthor(long authorId);
    List<Tour> GetByStatus(TourStatus status);
    List<Tour> GetByAuthorAndStatus(long authorId, TourStatus status);
    void Delete(long id);
    bool Exists(long id);
}