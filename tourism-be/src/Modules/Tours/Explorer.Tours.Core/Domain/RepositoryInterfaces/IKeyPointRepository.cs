using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IKeyPointRepository
{
    KeyPoint Create(KeyPoint keyPoint);
    KeyPoint Update(KeyPoint keyPoint);
    KeyPoint Get(long id);
    List<KeyPoint> GetByTour(long tourId);
    void Delete(long id);
    bool Exists(long id);
    int CountByTour(long tourId);
}