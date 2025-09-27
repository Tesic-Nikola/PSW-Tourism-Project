using Explorer.Bookings.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Bookings.Tests;

public class BookingsTestFactory : BaseTestFactory<BookingsContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BookingsContext>));
        services.Remove(descriptor!);
        services.AddDbContext<BookingsContext>(SetupTestContext());

        return services;
    }
}