using CarAction.AuctionService;
using CarAction.AuctionService.Consumers;
using CarAction.AuctionService.Data;
using CarAction.Contracts.Auctions;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(60);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedErrorConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionUpdatedErrorConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionDeletedErrorConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();
    x.AddConsumersFromNamespaceContaining<BidPlacedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseMessageRetry(r =>
        {
            r.Handle<RabbitMqConnectionException>();
            r.Interval(5, TimeSpan.FromSeconds(10));
        });

        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration["RabbitMq:Username"] ?? "guest");
            host.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false; // Its running on HTTP
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username"; // Specify what claim is the name of the user
    });

builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcAuctionService>();

var retryPolicy = Policy
    .Handle<NpgsqlException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

retryPolicy.ExecuteAndCapture(() => DbInitializer.InitializeDb(app));

app.Run();

// Give the WebApplicationFactory in the Integration Tests access to the Program class.
public partial class Program { }