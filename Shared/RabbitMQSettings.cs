namespace Shared;

static public class RabbitMQSettings // merkezi noktada ol
{
    public const string Stock_OrderCreatedEventQueue = "stock_order_created_event_queue";
    public const string Payment_StockReceivedEventQueue = "payment_stock_received_event_queue";
    public const string Order_PaymentCompletedEventQueue = "order_payment_completed_event_queue";
    public const string Order_StockNotReceivedEventQueue = "order_stock_not_received_event_queue";
    public const string Order_PaymentFailedEventQueue = "order_payment_failed_event_queue";
}