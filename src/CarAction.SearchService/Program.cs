using System.Net;

using CarAction.SearchService;

using MassTransit;

using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

// Ensure that the search service is started even though no successful response have been
// received from the Auction Service.
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Policy.Handle<TimeoutException>()
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10))
        .ExecuteAndCaptureAsync(async () => await DbInitializer.InitializeDb(app));

});

app.Run();

// Keep trying to get the data from the AuctionService, until we receive a successful response.
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));