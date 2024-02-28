using System.Net;
using System.Net.Http.Json;

using CarAction.AuctionService.Data;
using CarAction.AuctionService.DTOs;
using CarAction.AuctionService.Integration.Tests.Fixtures;
using CarAction.AuctionService.Integration.Tests.Utils;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CarAction.AuctionService.Integration.Tests;

[Collection("Shared collection")]
public class AuctionControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;

    // We need an actual http client as we are going to call each controller using it.
    // This ensure the correct behavious and context.
    private readonly HttpClient _httpClient;
    private const string _gT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    // Run before every test in the class.
    // Nothing to do as the database is initialized at startup and then reinitialized at dispose.
    public Task InitializeAsync() => Task.CompletedTask;

    // Run after each test in the class
    public Task DisposeAsync()
    {
        // Need to use the current scope that the test is running so we can access the database, (it has scoped lifetime).
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAuctions_ShouldReturn3Auctions()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDTO>>("api/auctions");

        // Assert
        Assert.Equal(3, response.Count);
    }
    [Fact]
    public async Task GetAuctions_ShouldReturn3AuctionsAndOKStatusCode()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetAsync("api/auctions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = await response.Content.ReadFromJsonAsync<List<AuctionDTO>>();
        Assert.Equal(3, data.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetFromJsonAsync<AuctionDTO>($"api/auctions/{_gT_ID}");

        // Assert
        Assert.Equal("GT", response.Model);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturn404()
    {
        // arrange?

        // act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShouldReturn400()
    {
        // arrange?

        // act
        var response = await _httpClient.GetAsync($"api/auctions/notaguid");

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // arrange?
        var auction = new CreateAuctionDTO{  Make = "test" };

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // arrange?
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDTO>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // Arrange
        var auction = GetAuctionForCreate();
        auction.Color = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn204NoContent()
    {
        // Arrange
        var auction = new UpdateAuctionDTO {
            Color = "Blue"
         };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{_gT_ID}", auction);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // Arrange
        var auction = new UpdateAuctionDTO
        {
            Color = "Blue"
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("NotBob"));

        // Act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{_gT_ID}", auction);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }


    [Fact]
    public async Task DeleteAuction_WithValidUpdateDtoAndUser_ShouldReturn204NoContent()
    {
        // Arrange
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // Act
        var response = await _httpClient.DeleteAsync($"api/auctions/{_gT_ID}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // Arrange
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("NotBob"));

        // Act
        var response = await _httpClient.DeleteAsync($"api/auctions/{_gT_ID}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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