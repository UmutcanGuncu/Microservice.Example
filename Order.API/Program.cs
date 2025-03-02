using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderAPIDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionAdress"));
});
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentCompletedEventConsumer>();
    configure.AddConsumer<StockNotReservedEventConsumer>();
    configure.AddConsumer<PaymentFailedEventConsumer>();
    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMQ"]));
        cfg.ReceiveEndpoint($"{RabbitMQSettings.Order_PaymentCompletedEventQueue}",e=> e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        cfg.ReceiveEndpoint($"{RabbitMQSettings.Order_StockNotReceivedEventQueue}", e=> e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
        cfg.ReceiveEndpoint($"{RabbitMQSettings.Order_PaymentFailedEventQueue}", e=> e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});
var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();