namespace Explorer.BuildingBlocks.Core.Events;

public class EmailRequestedEvent : BaseEvent
{
    public string To { get; }
    public string Subject { get; }
    public string Body { get; }
    public bool IsHtml { get; }

    public EmailRequestedEvent(string to, string subject, string body, bool isHtml = true)
    {
        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }
}