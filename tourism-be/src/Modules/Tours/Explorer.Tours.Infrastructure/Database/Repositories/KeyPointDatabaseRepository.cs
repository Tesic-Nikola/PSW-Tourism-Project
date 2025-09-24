using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class KeyPointDatabaseRepository : IKeyPointRepository
{
    private readonly ToursContext _dbContext;

    public KeyPointDatabaseRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
    }

    public KeyPoint Create(KeyPoint keyPoint)
    {
        _dbContext.KeyPoints.Add(keyPoint);
        _dbContext.SaveChanges();
        return keyPoint;
    }

    public KeyPoint Update(KeyPoint keyPoint)
    {
        try
        {
            _dbContext.Update(keyPoint);
            _dbContext.SaveChanges();
            return keyPoint;
        }
        catch (Exception)
        {
            throw new KeyNotFoundException("KeyPoint not found");
        }
    }

    public KeyPoint Get(long id)
    {
        var keyPoint = _dbContext.KeyPoints.Find(id);
        if (keyPoint == null) throw new KeyNotFoundException("KeyPoint not found: " + id);
        return keyPoint;
    }

    public List<KeyPoint> GetByTour(long tourId)
    {
        return _dbContext.KeyPoints.Where(kp => kp.TourId == tourId).OrderBy(kp => kp.Order).ToList();
    }

    public void Delete(long id)
    {
        var keyPoint = Get(id);
        _dbContext.KeyPoints.Remove(keyPoint);
        _dbContext.SaveChanges();
    }

    public bool Exists(long id)
    {
        return _dbContext.KeyPoints.Any(kp => kp.Id == id);
    }

    public int CountByTour(long tourId)
    {
        return _dbContext.KeyPoints.Count(kp => kp.TourId == tourId);
    }
}