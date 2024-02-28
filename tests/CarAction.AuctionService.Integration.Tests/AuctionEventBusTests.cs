using System.Net;
using System.Net.Http.Json;

using CarAction.AuctionService.Data;
using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Integration.Tests.Fixtures;
using CarAction.AuctionService.Integration.Tests.Utils;
using CarAction.Contracts.Auctions;

using MassTransit.Testing;

using Microsoft.Extensions.DependencyInjection;

namespace CarAction.AuctionService.Integration.Tests;

[Collection("Shared collection")]
public class AuctionEventBusTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly ITestHarness _testHarness;

    public AuctionEventBusTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _testHarness = factory.Services.GetTestHarness();
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
    {
        // Arrange
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(await _testHarness.Published.Any<AuctionCreated>());
    }

    // Run before every test in the class.
    // Nothing to do as the database is initialized at startup and then reinitialized at dispose.
    public Task InitializeAsync() => Task.CompletedTask;

    // Run after each test in the class
    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    private static CreateAuctionDTO GetAuctionForCreate()
    {
        return new CreateAuctionDTO
        {
            Make = "test",
            Model = "testModel",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
            ReservePrice = 10
        };
    }
}
