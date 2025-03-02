namespace Shared.Events.Common;

public class PaymentCompletedEvent : IEvent
{
    public Guid OrderId { get; set; }
}