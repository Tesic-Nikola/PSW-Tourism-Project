using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Tour : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TourDifficulty Difficulty { get; private set; }
    public TourCategory Category { get; private set; }
    public decimal Price { get; private set; }
    public DateTime StartDate { get; private set; }
    public TourStatus Status { get; private set; }
    public long AuthorId { get; private set; }

    public Tour(string name, string description, TourDifficulty difficulty,
               TourCategory category, decimal price, DateTime startDate, long authorId)
    {
        Name = name;
        Description = description;
        Difficulty = difficulty;
        Category = category;
        Price = price;
        StartDate = startDate;
        Status = TourStatus.Draft;
        AuthorId = authorId;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        if (Price < 0) throw new ArgumentException("Invalid Price");
        if (StartDate <= DateTime.Now) throw new ArgumentException("Start date must be in the future");
        if (AuthorId == 0) throw new ArgumentException("Invalid AuthorId");
    }

    public void Publish()
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Only draft tours can be published");

        Status = TourStatus.Published;
    }

    public void Cancel()
    {
        if (Status != TourStatus.Published)
            throw new InvalidOperationException("Only published tours can be cancelled");

        var timeUntilStart = StartDate - DateTime.Now;
        if (timeUntilStart.TotalHours < 24)
            throw new InvalidOperationException("Tour can only be cancelled at least 24 hours before start");

        Status = TourStatus.Cancelled;
    }
}

public enum TourStatus
{
    Draft,
    Published,
    Cancelled
}

public enum TourDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum TourCategory
{
    Nature,
    Art,
    Sport,
    Shopping,
    Food
}