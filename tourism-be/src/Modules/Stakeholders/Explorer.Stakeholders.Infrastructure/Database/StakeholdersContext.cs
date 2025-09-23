using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Interest> Interests { get; set; }
    public DbSet<PersonInterest> PersonInterests { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        ConfigureStakeholder(modelBuilder);
        ConfigureInterests(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }

    private static void ConfigureInterests(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interest>()
            .HasIndex(i => i.Name)
            .IsUnique();

        modelBuilder.Entity<PersonInterest>()
            .HasOne(pi => pi.Person)
            .WithMany()
            .HasForeignKey(pi => pi.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PersonInterest>()
            .HasOne(pi => pi.Interest)
            .WithMany()
            .HasForeignKey(pi => pi.InterestId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PersonInterest>()
            .HasIndex(pi => new { pi.PersonId, pi.InterestId })
            .IsUnique();
    }
}