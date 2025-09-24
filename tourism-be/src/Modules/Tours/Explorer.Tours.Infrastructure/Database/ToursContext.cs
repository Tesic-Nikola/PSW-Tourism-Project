using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<KeyPoint> KeyPoints { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        ConfigureTour(modelBuilder);
        ConfigureKeyPoint(modelBuilder);
    }

    private static void ConfigureTour(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tour>()
            .Property(t => t.Status)
            .HasConversion<int>();

        modelBuilder.Entity<Tour>()
            .Property(t => t.Difficulty)
            .HasConversion<int>();

        modelBuilder.Entity<Tour>()
            .Property(t => t.Category)
            .HasConversion<int>();
    }

    private static void ConfigureKeyPoint(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyPoint>()
            .HasIndex(kp => new { kp.TourId, kp.Order })
            .IsUnique();
    }
}