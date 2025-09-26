using Explorer.Bookings.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Bookings.Infrastructure.Database;

public class BookingsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<TourPurchase> TourPurchases { get; set; }
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    public DbSet<BonusPoints> BonusPoints { get; set; }

    public BookingsContext(DbContextOptions<BookingsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bookings");

        ConfigureShoppingCart(modelBuilder);
        ConfigureTourPurchase(modelBuilder);
        ConfigureBonusPoints(modelBuilder);
    }

    private static void ConfigureShoppingCart(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShoppingCart>()
            .HasMany(sc => sc.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingCart>()
            .HasIndex(sc => sc.TouristId)
            .IsUnique();
    }

    private static void ConfigureTourPurchase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TourPurchase>()
            .Property(tp => tp.Status)
            .HasConversion<int>();

        modelBuilder.Entity<TourPurchase>()
            .HasMany(tp => tp.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TourPurchase>()
            .HasIndex(tp => tp.TouristId);

        modelBuilder.Entity<TourPurchase>()
            .HasIndex(tp => tp.PurchaseDate);
    }

    private static void ConfigureBonusPoints(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BonusPoints>()
            .HasIndex(bp => bp.TouristId)
            .IsUnique();

        modelBuilder.Entity<BonusPoints>()
            .Property(bp => bp.AvailablePoints)
            .HasPrecision(18, 2);
    }
}