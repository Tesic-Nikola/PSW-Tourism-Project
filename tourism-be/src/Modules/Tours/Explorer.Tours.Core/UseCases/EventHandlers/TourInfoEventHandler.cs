using Explorer.BuildingBlocks.Core.Events;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.Extensions.Logging;

namespace Explorer.Tours.Core.UseCases.EventHandlers;

public class TourInfoEventHandler : IEventHandler<TourInfoRequestedEvent>
{
    private readonly ITourService _tourService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TourInfoEventHandler> _logger;

    public TourInfoEventHandler(ITourService tourService, IEventBus eventBus, ILogger<TourInfoEventHandler> logger)
    {
        _tourService = tourService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(TourInfoRequestedEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling tour info request for TourId: {TourId}", @event.TourId);

            var tourResult = _tourService.Get(@event.TourId);

            if (tourResult.IsSuccess)
            {
                var tour = tourResult.Value;
                var responseEvent = new TourInfoResponseEvent(
                    @event.RequestId,
                    @event.TourId,
                    @event.TouristId,
                    @event.Quantity,
                    tour.Name,
                    tour.Price,
                    tour.StartDate
                );

                await _eventBus.PublishAsync(responseEvent);
                _logger.LogInformation("Tour info response sent for TourId: {TourId}", @event.TourId);
            }
            else
            {
                var errorEvent = new TourInfoResponseEvent(
                    @event.RequestId,
                    @event.TourId,
                    @event.TouristId,
                    "Tour not found or not available"
                );

                await _eventBus.PublishAsync(errorEvent);
                _logger.LogWarning("Tour not found for TourId: {TourId}", @event.TourId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tour info request for TourId: {TourId}", @event.TourId);

            var errorEvent = new TourInfoResponseEvent(
                @event.RequestId,
                @event.TourId,
                @event.TouristId,
                "Internal error occurred while retrieving tour information"
            );

            await _eventBus.PublishAsync(errorEvent);
        }
    }
}