using GeoLocator.Core.Configuration;
using GeoLocator.Core.Interfaces;
using GeoLocator.Core.Services;
using GeoLocator.Infrastructure;
using GeoLocator.Infrastructure.Caching;
using GeoLocator.Infrastructure.Logging;
using GeoLocator.Infrastructure.Services.IpApi;
using GeoLocator.Infrastructure.Services.IpStack;
using GeoLocator.Shared.HttpClients;
using GeoLocator.Web.Extensions;
using GeoLocator.Web.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Dependencies.ConfigureServices(builder.Configuration, builder.Services);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<IpStackOptions>(builder.Configuration.GetSection(IpStackOptions.IpStack));

var ipStackOptions = builder.Configuration.GetSection(IpStackOptions.IpStack).Get<IpStackOptions>();
var ipApiOptions = builder.Configuration.GetSection(IpApiOptions.IpApi).Get<IpApiOptions>();

if (ipStackOptions.Enabled && ipApiOptions.Enabled)
{
    throw new Exception("Cannot have mulitple 3rd party location API services enabled");
}

if (ipStackOptions.Enabled)
{
    builder.Services.AddHttpClient(NamedHttpClients.IpStackHttpClient, client =>
    {
        client.BaseAddress = new Uri(ipStackOptions.BaseUrl);
    });
    builder.Services.AddScoped<IIpLocationLookupService, IpStackService>();
}
else if (ipApiOptions.Enabled)
{
    builder.Services.AddHttpClient(NamedHttpClients.IpApiHttpClient, client =>
    {
        client.BaseAddress = new Uri(ipApiOptions.BaseUrl);
    });

    builder.Services.AddScoped<IIpLocationLookupService, IpApiService>();
}
else
{
    throw new Exception("No 3rd party location API services enabled");
}

builder.Services.AddScoped<ILocatorService, LocatorService>();
builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

builder.Services.RegisterRepositories();

builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection("CacheConfiguration"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCaching();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

await Dependencies.EnsureDatabaseCreated(app.Services, app.Configuration);

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
