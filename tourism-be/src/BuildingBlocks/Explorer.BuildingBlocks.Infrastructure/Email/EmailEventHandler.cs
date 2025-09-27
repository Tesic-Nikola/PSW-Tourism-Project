using Explorer.BuildingBlocks.Core.Email;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.Logging;

namespace Explorer.BuildingBlocks.Infrastructure.Email;

public class EmailEventHandler : IEventHandler<EmailRequestedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailEventHandler> _logger;

    public EmailEventHandler(IEmailService emailService, ILogger<EmailEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(EmailRequestedEvent @event)
    {
        try
        {
            await _emailService.SendEmailAsync(@event.To, @event.Subject, @event.Body, @event.IsHtml);
            _logger.LogInformation("Email event handled successfully for {To}", @event.To);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to handle email event for {To}", @event.To);
            // Don't rethrow - email failures shouldn't break the main flow
        }
    }
}