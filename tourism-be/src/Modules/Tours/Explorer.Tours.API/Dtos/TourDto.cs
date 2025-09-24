namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Difficulty { get; set; } // TourDifficulty enum as int
    public int Category { get; set; } // TourCategory enum as int
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public int Status { get; set; } // TourStatus enum as int
    public long AuthorId { get; set; }
}