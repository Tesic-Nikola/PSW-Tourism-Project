using Explorer.Bookings.API.Public;
using Explorer.Bookings.Core.Domain.RepositoryInterfaces;
using Explorer.Bookings.Core.Mappers;
using Explorer.Bookings.Core.UseCases;
using Explorer.Bookings.Infrastructure.Database;
using Explorer.Bookings.Infrastructure.Database.Repositories;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Bookings.Infrastructure;

public static class BookingsStartup
{
    public static IServiceCollection ConfigureBookingsModule(this IServiceCollection services)
    {
        // Registers all profiles since it works on the assembly
        services.AddAutoMapper(typeof(BookingsProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<ITourPurchaseService, TourPurchaseService>();
        services.AddScoped<IBonusPointsService, BonusPointsService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IShoppingCartRepository, ShoppingCartDatabaseRepository>();
        services.AddScoped<ITourPurchaseRepository, TourPurchaseDatabaseRepository>();
        services.AddScoped<IBonusPointsRepository, BonusPointsDatabaseRepository>();

        services.AddDbContext<BookingsContext>(opt =>
            opt.UseNpgsql(DbConnectionStringBuilder.Build("bookings"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "bookings")));
    }
}