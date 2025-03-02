using MongoDB.Driver;

namespace Stock.API.Services;

public class MongoDBService
{ 
    readonly IMongoDatabase _database;
    
    public MongoDBService(IConfiguration configuration)
    {
        try
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase("StockAPI");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
           
        }
        
    }

    public IMongoCollection<T> GetCollection<T>() => _database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());

}