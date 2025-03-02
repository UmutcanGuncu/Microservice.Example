using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumers;

public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
{
    private readonly OrderAPIDbContext _context;

    public PaymentFailedEventConsumer(OrderAPIDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        Models.Entities.Order order = await _context.Orders.FirstOrDefaultAsync(x=>x.Id == context.Message.OrderId);
        order.OrderStatus = Models.Enums.OrderStatus.Failed;
        await _context.SaveChangesAsync();
    }
}