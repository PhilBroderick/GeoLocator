using GeoLocator.Core.Interfaces;
using GeoLocator.Infrastructure.Caching;
using GeoLocator.Infrastructure.Repository;

namespace GeoLocator.Web.Extensions
{
    public static class ServiceRegistration
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.Decorate<ILocationRepository, CachedLocationRepositoryDecorator>();
        }
    }
}
