using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPoint : Entity
{
    public long TourId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string ImageUrl { get; private set; }
    public int Order { get; private set; }

    public KeyPoint(long tourId, string name, string description,
                   double latitude, double longitude, string imageUrl, int order)
    {
        TourId = tourId;
        Name = name;
        Description = description;
        Latitude = latitude;
        Longitude = longitude;
        ImageUrl = imageUrl;
        Order = order;
        Validate();
    }

    private void Validate()
    {
        if (TourId == 0) throw new ArgumentException("Invalid TourId");
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        if (Latitude < -90 || Latitude > 90) throw new ArgumentException("Invalid Latitude");
        if (Longitude < -180 || Longitude > 180) throw new ArgumentException("Invalid Longitude");
        if (string.IsNullOrWhiteSpace(ImageUrl)) throw new ArgumentException("Invalid ImageUrl");
        if (Order < 1) throw new ArgumentException("Invalid Order");
    }
}