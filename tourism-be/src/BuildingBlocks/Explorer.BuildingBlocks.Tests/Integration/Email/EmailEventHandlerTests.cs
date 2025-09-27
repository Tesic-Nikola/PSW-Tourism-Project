using Explorer.BuildingBlocks.Core.Email;
using Explorer.BuildingBlocks.Core.Events;
using Explorer.BuildingBlocks.Infrastructure.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace Explorer.BuildingBlocks.Tests.Integration.Email;

[Collection("Sequential")]
public class EmailEventHandlerTests
{
    [Fact]
    public async Task EmailHandler_SmtpFailure()
    {
        // Arrange
        var failingEmailService = new FailingEmailService();
        var logger = NullLogger<EmailEventHandler>.Instance;
        var handler = new EmailEventHandler(failingEmailService, logger);

        var emailEvent = new EmailRequestedEvent("test@example.com", "Test Subject", "Test Body");

        // Act & Assert - Should not throw exception even when SMTP fails
        await handler.HandleAsync(emailEvent);

        // Verify SMTP was attempted
        failingEmailService.WasAttempted.ShouldBeTrue();
    }

    private class FailingEmailService : IEmailService
    {
        public bool WasAttempted { get; private set; }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            WasAttempted = true;
            throw new InvalidOperationException("SMTP connection failed - check your email configuration");
        }
    }
}