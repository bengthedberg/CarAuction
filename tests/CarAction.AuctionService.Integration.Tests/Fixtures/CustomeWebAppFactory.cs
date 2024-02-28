using CarAction.AuctionService.Data;

using MassTransit;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

using WebMotions.Fake.Authentication.JwtBearer;

namespace CarAction.AuctionService.Integration.Tests.Fixtures;

/// <summary>
/// Shared accross all integration tests.
/// Basically creates a test instance of the auction web application and add test services to create
/// high value tests that can ensure that the web application works correctly.
///
/// The IAsyncLifetime comes from XUnit and provides calls that are executed at beginning and end of tests.
/// It is in these we will create and destroy our servcies.
/// </summary>
public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Postgres in a test container, as postgres does not have an in memory database.
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    // Since WebApplicationFactory use the Program.cs, it will load the same configuration used to
    // start the application.
    // This way, there's no need to duplicate the appsettings.json file in the test project.
    //
    // However, as this is a web application, it is possible to override the configuration.
    // The ConfigureWebHost method is used to override the configuration.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the DbContext that was setup in the Program class
            services.RemoveDbContext<AuctionDbContext>();

            // Add our own Test DbContext for testing, using the Postgres container.
            services.AddDbContext<AuctionDbContext>(options =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });

            // Fake RabbitMq using the MassTransit test harness.
            // This will remove the configure for MassTransit in Program class and
            services.AddMassTransitTestHarness();

            // Create database, run migrations and seed with test data.
            services.EnsureCreated<AuctionDbContext>();

            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                .AddFakeJwtBearer(opt =>
                {
                    opt.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                });
        });
    }

    Task IAsyncLifetime.DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}

internal class PostgresSqlContainer
{
}
