using GeoLocator.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace GeoLocator.FunctionalTests;

public  class WebTestFixture : WebApplicationFactory<Program>
{
    private readonly string _environment = "Test";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        builder.ConfigureServices(services =>
        {
            services.AddScoped(sp =>
            {
                return new DbContextOptionsBuilder<GeoLocatorDbContext>()
                .UseInMemoryDatabase("GeoLocatorDbFunctionalTests")
                .UseApplicationServiceProvider(sp)
                .Options;
            });

            services.AddSingleton<IStartupFilter, CustomStartupFilter>();
        });

        return base.CreateHost(builder);
    }
}

public class CustomStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>();
            next(app);
        };
    }
}
