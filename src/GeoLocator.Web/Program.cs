using GeoLocator.Core.Configuration;
using GeoLocator.Core.Interfaces;
using GeoLocator.Core.Services;
using GeoLocator.Infrastructure.Caching;
using GeoLocator.Infrastructure.Data;
using GeoLocator.Infrastructure.Logging;
using GeoLocator.Infrastructure.Repository;
using GeoLocator.Infrastructure.Services.IpStack;
using GeoLocator.Shared.HttpClients;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<IpStackOptions>(builder.Configuration.GetSection(IpStackOptions.IpStack));

var ipStackOptions = builder.Configuration.GetSection(IpStackOptions.IpStack).Get<IpStackOptions>();

// Add services to the container.
builder.Services.AddHttpClient(NamedHttpClients.IpStackHttpClient, client =>
{
    client.BaseAddress = new Uri(ipStackOptions.IpStackBaseUrl);
});

builder.Services.AddScoped<IIpLocationLookupService, IpStackService>();
builder.Services.AddScoped<ILocatorService, LocatorService>();
builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.Decorate<ILocationRepository, CachedLocationRepositoryDecorator>();

builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection("CacheConfiguration"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<GeoLocatorDbContext>(options =>
{
    options.UseInMemoryDatabase("GeoLocator");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
