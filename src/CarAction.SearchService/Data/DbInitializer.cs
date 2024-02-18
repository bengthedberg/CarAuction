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

        await SynchData(app);
    }

    private static async Task SynchData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();

        Console.WriteLine($"Items count: {items.Count}");

        if ( items.Count > 0 )
        {
            await DB.SaveAsync(items);
        }

    }
}
