using Explorer.BuildingBlocks.Core.Events;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.Extensions.Logging;

namespace Explorer.Stakeholders.Core.UseCases.EventHandlers;

public class TouristInfoEventHandler : IEventHandler<TouristInfoRequestedEvent>
{
    private readonly ICrudRepository<Person> _personRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<TouristInfoEventHandler> _logger;

    public TouristInfoEventHandler(ICrudRepository<Person> personRepository, IEventBus eventBus, ILogger<TouristInfoEventHandler> logger)
    {
        _personRepository = personRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task HandleAsync(TouristInfoRequestedEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling tourist info request for TouristId: {TouristId}", @event.TouristId);

            var person = _personRepository.Get(@event.TouristId);

            var responseEvent = new TouristInfoResponseEvent(
                @event.RequestId,
                @event.TouristId,
                person.Email,
                person.Name,
                person.Surname
            );

            await _eventBus.PublishAsync(responseEvent);
            _logger.LogInformation("Tourist info response sent for TouristId: {TouristId}", @event.TouristId);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Tourist not found for TouristId: {TouristId}", @event.TouristId);

            var errorEvent = new TouristInfoResponseEvent(
                @event.RequestId,
                @event.TouristId,
                "Tourist not found"
            );

            await _eventBus.PublishAsync(errorEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling tourist info request for TouristId: {TouristId}", @event.TouristId);

            var errorEvent = new TouristInfoResponseEvent(
                @event.RequestId,
                @event.TouristId,
                "Internal error occurred while retrieving tourist information"
            );

            await _eventBus.PublishAsync(errorEvent);
        }
    }
}