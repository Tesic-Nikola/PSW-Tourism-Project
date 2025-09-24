using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDatabaseRepository : ITourRepository
{
    private readonly ToursContext _dbContext;

    public TourDatabaseRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Tour Create(Tour tour)
    {
        _dbContext.Tours.Add(tour);
        _dbContext.SaveChanges();
        return tour;
    }

    public Tour Update(Tour tour)
    {
        try
        {
            _dbContext.Update(tour);
            _dbContext.SaveChanges();
            return tour;
        }
        catch (Exception)
        {
            throw new KeyNotFoundException("Tour not found");
        }
    }

    public Tour Get(long id)
    {
        var tour = _dbContext.Tours.Find(id);
        if (tour == null) throw new KeyNotFoundException("Tour not found: " + id);
        return tour;
    }

    public List<Tour> GetByAuthor(long authorId)
    {
        return _dbContext.Tours.Where(t => t.AuthorId == authorId).ToList();
    }

    public List<Tour> GetByStatus(TourStatus status)
    {
        return _dbContext.Tours.Where(t => t.Status == status).ToList();
    }

    public List<Tour> GetByAuthorAndStatus(long authorId, TourStatus status)
    {
        return _dbContext.Tours.Where(t => t.AuthorId == authorId && t.Status == status).ToList();
    }

    public void Delete(long id)
    {
        var tour = Get(id);
        _dbContext.Tours.Remove(tour);
        _dbContext.SaveChanges();
    }

    public bool Exists(long id)
    {
        return _dbContext.Tours.Any(t => t.Id == id);
    }
}