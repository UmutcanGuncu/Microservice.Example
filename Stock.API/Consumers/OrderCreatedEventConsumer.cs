using  MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using Stock.API.Services;

namespace Stock.API.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly MongoDBService _mongoDbService;
    IMongoCollection<Stock.API.Models.Entities.Stock> _stockCollection;
    private readonly ISendEndpointProvider _sendEndpoint;
    private readonly IPublishEndpoint _publishEndpoint;
    public OrderCreatedEventConsumer(MongoDBService mongoDbService, IMongoCollection<Models.Entities.Stock> stockCollection, ISendEndpointProvider sendEndpoint, IPublishEndpoint publishEndpoint)
    {
        _mongoDbService = mongoDbService;
        _stockCollection = _mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
        _sendEndpoint = sendEndpoint;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        List<bool> stockResult = new List<bool>();
        foreach (OrderItemMessage item in context.Message.OrderItems)
        { 
            stockResult.Add((await _stockCollection.FindAsync(stock => stock.ProductId == item.ProductId && stock.Count >= item.Count)).Any());
        }

        if (stockResult.TrueForAll(sr => sr.Equals(true)))
        {
            foreach (OrderItemMessage item in context.Message.OrderItems)
            {
                Models.Entities.Stock stock = await (await _stockCollection.FindAsync(s=>s.ProductId == item.ProductId)).FirstOrDefaultAsync();
                stock.Count -= item.Count;
                await _stockCollection.FindOneAndReplaceAsync(s=> s.ProductId == item.ProductId, stock);
            }

            StockReservedEvent stockReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                TotalPrice = context.Message.TotalPrice
            };
            ISendEndpoint sendEndpoint= await _sendEndpoint.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReceivedEventQueue}"));
            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            StockNotReservedEvent stockNotReservedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                Message = "Elimizde Stok Kalmadı "
            };
            await _publishEndpoint.Publish(stockNotReservedEvent);
        }
        Console.WriteLine($"Order {context.Message.OrderId} has been created");
        
    }
}