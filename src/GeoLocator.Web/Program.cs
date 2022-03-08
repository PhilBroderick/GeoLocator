using GeoLocator.Core.Interfaces;
using GeoLocator.Core.Services;
using GeoLocator.Infrastructure.Logging;
using GeoLocator.Infrastructure.Services.IpStack;
using GeoLocator.Shared.HttpClients;
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

builder.Services.AddHttpContextAccessor();

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
