using Explorer.Bookings.API.Dtos;
using Explorer.BuildingBlocks.Core.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

namespace Explorer.Bookings.Core.UseCases.EventHandlers;

public class TouristInfoResponseEventHandler : IEventHandler<TouristInfoResponseEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<TouristInfoResponseEventHandler> _logger;
    private static readonly ConcurrentDictionary<Guid, PendingCheckoutRequest> _pendingRequests = new();

    public TouristInfoResponseEventHandler(IEventBus eventBus, ILogger<TouristInfoResponseEventHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public static void AddPendingRequest(Guid requestId, PendingCheckoutRequest request)
    {
        _pendingRequests[requestId] = request;
    }

    public static bool RemovePendingRequest(Guid requestId, out PendingCheckoutRequest request)
    {
        return _pendingRequests.TryRemove(requestId, out request);
    }

    public async Task HandleAsync(TouristInfoResponseEvent @event)
    {
        _logger.LogInformation("Handling tourist info response for RequestId: {RequestId}", @event.RequestId);

        if (!RemovePendingRequest(@event.RequestId, out var pendingRequest))
        {
            _logger.LogWarning("No pending checkout request found for RequestId: {RequestId}", @event.RequestId);
            return;
        }

        try
        {
            if (!@event.IsSuccess)
            {
                _logger.LogError("Failed to get tourist info: {ErrorMessage}", @event.ErrorMessage);
                pendingRequest.CompletionSource.SetResult(pendingRequest.PurchaseDto); // Complete without email
                return;
            }

            // Generate email content
            var emailSubject = "Tour Purchase Confirmation - Explorer";
            var emailBody = GenerateEmailBody(@event.Name, @event.Surname, pendingRequest.PurchaseDto);

            // Send email event
            var emailEvent = new EmailRequestedEvent(@event.Email, emailSubject, emailBody);
            await _eventBus.PublishAsync(emailEvent);

            _logger.LogInformation("Email sent for purchase {PurchaseId} to {Email}",
                pendingRequest.PurchaseDto.Id, @event.Email);

            // Complete the checkout
            pendingRequest.CompletionSource.SetResult(pendingRequest.PurchaseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing tourist info response for RequestId: {RequestId}", @event.RequestId);
            // Still complete the checkout even if email fails
            pendingRequest.CompletionSource.SetResult(pendingRequest.PurchaseDto);
        }
    }

    private static string GenerateEmailBody(string name, string surname, TourPurchaseDto purchase)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>Dear {name} {surname},</h2>");
        sb.AppendLine("<p>Thank you for your purchase! Here are the details of your tour booking:</p>");
        sb.AppendLine($"<p><strong>Purchase Date:</strong> {purchase.PurchaseDate:yyyy-MM-dd HH:mm}</p>");
        sb.AppendLine($"<p><strong>Total Price:</strong> ${purchase.TotalPrice:F2}</p>");

        if (purchase.BonusPointsUsed > 0)
        {
            sb.AppendLine($"<p><strong>Bonus Points Used:</strong> {purchase.BonusPointsUsed:F2}</p>");
            sb.AppendLine($"<p><strong>Final Price:</strong> ${purchase.FinalPrice:F2}</p>");
        }

        sb.AppendLine("<h3>Tour Details:</h3>");
        sb.AppendLine("<ul>");

        foreach (var item in purchase.Items)
        {
            sb.AppendLine($"<li><strong>{item.TourName}</strong>");
            sb.AppendLine($"<br>Start Date: {item.TourStartDate:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"<br>Price: ${item.TourPrice:F2}");
            sb.AppendLine($"<br>Quantity: {item.Quantity}</li>");
        }

        sb.AppendLine("</ul>");
        sb.AppendLine("<p>We look forward to seeing you on your tour!</p>");
        sb.AppendLine("<p>Best regards,<br>The Explorer Team</p>");

        return sb.ToString();
    }
}