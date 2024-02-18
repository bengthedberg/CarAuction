using System.Text.Json;

using CarAction.SearchService.Models;

using MongoDB.Driver;
using MongoDB.Entities;

namespace CarAction.SearchService;

public class DbInitializer
{
    public static async Task InitializeDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb",
            MongoClientSettings
                .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));
        await DB.Index<Item>()
            .Key(i => i.Make, KeyType.Text)
            .Key(i => i.Model, KeyType.Text)
            .Key(i => i.Color, KeyType.Text)
            .CreateAsync();

        await SeedData();
    }

    private static async Task SeedData()
    {
        var count = await DB.CountAsync<Item>();
        if (count == 0)
        {
            Console.WriteLine("Seeding data...");

            var itemData = await File.ReadAllTextAsync("Data/auctions.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
            var result = await DB.SaveAsync(items);

            Console.WriteLine($"Inserted {result.InsertedCount} records");
            Console.WriteLine(value: $"Updated {result.ModifiedCount} records");
        }
        else
        {
            Console.WriteLine($"Found {count} records");
        }
    }
}
