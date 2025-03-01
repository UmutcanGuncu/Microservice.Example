using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.Models.Enums;
using Order.API.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrdersController: ControllerBase
{
    private readonly OrderAPIDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;


    public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
    {
        Models.Entities.Order order = new()
        {
            Id = Guid.NewGuid(),
            BuyerId = model.BuyerId,
            Created = DateTime.Now,
            OrderStatus = OrderStatus.Suspended
        };
        order.OrderItems = model.OrderItems.Select(x=>  new OrderItem
        {
            ProductId = x.ProductId,
            Count = x.Count,
            Price = x.Price,
            Created = DateTime.Now,
            
        }).ToList();
        order.TotalPrice = order.OrderItems.Sum(x=> x.Price * x.Count);
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        OrderCreatedEvent createdEvent = new()
        {
            OrderId = order.Id,
            BuyerId = order.BuyerId,
            OrderItems = order.OrderItems.Select(x=> new OrderItemMessage()
            {
                ProductId = x.ProductId,
                Count = x.Count
            }).ToList()

        };
        await _publishEndpoint.Publish(createdEvent);
        return Ok();
    }
}