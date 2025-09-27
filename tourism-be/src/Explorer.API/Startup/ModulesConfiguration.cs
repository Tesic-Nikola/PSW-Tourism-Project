using Explorer.Bookings.Infrastructure;
using Explorer.BuildingBlocks.Core.Email;
using Explorer.BuildingBlocks.Core.Events;
using Explorer.BuildingBlocks.Infrastructure.Email;
using Explorer.BuildingBlocks.Infrastructure.Events;
using Explorer.Stakeholders.Infrastructure;
using Explorer.Tours.Infrastructure;

namespace Explorer.API.Startup;

public static class ModulesConfiguration
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        // Register BuildingBlocks services first
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IEventHandler<EmailRequestedEvent>, EmailEventHandler>();

        // Register modules
        services.ConfigureStakeholdersModule();
        services.ConfigureToursModule();
        services.ConfigureBookingsModule();

        // Subscribe to events after all modules are registered
        SetupEventSubscriptions(services);

        return services;
    }

    private static void SetupEventSubscriptions(IServiceCollection services)
    {
        // This will be called when the service provider is built
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        // Subscribe event handlers
        var tourInfoHandler = serviceProvider.GetRequiredService<IEventHandler<TourInfoRequestedEvent>>();
        eventBus.Subscribe<TourInfoRequestedEvent>(tourInfoHandler.HandleAsync);

        var touristInfoHandler = serviceProvider.GetRequiredService<IEventHandler<TouristInfoRequestedEvent>>();
        eventBus.Subscribe<TouristInfoRequestedEvent>(touristInfoHandler.HandleAsync);

        var tourInfoResponseHandler = serviceProvider.GetRequiredService<IEventHandler<TourInfoResponseEvent>>();
        eventBus.Subscribe<TourInfoResponseEvent>(tourInfoResponseHandler.HandleAsync);

        var touristInfoResponseHandler = serviceProvider.GetRequiredService<IEventHandler<TouristInfoResponseEvent>>();
        eventBus.Subscribe<TouristInfoResponseEvent>(touristInfoResponseHandler.HandleAsync);

        var emailHandler = serviceProvider.GetRequiredService<IEventHandler<EmailRequestedEvent>>();
        eventBus.Subscribe<EmailRequestedEvent>(emailHandler.HandleAsync);
    }
}