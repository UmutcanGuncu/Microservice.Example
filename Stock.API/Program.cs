using MassTransit;
using MongoDB.Driver;
using Shared;
using Stock.API.Consumers;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// MongoDB bağlantısı
builder.Services.AddSingleton<MongoDBService>();

// MassTransit ayarları
builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMQ"]));
        cfg.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

// MongoDBService üzerinden IMongoCollection<Stock> kaydetme
builder.Services.AddSingleton<IMongoCollection<Stock.API.Models.Entities.Stock>>(sp =>
{
    var mongoDBService = sp.GetRequiredService<MongoDBService>();
    return mongoDBService.GetCollection<Stock.API.Models.Entities.Stock>();  // MongoDBService'in GetCollection methoduyla koleksiyon alınır
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();