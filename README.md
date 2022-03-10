![Build & Test](https://github.com/PhilBroderick/GeoLocator/actions/workflows/build-test.yaml/badge.svg)


# GeoLocator
Microservice API wrapper for 3rd party location by IP address services

# Supported Services
Currently only the following services are supported:
- [ipstack](https://ipstack.com/)
- [ip-api](https://ip-api.com/)

More services can be easily added and substituted in. *ip-api* is the preferred option, as it doesn't have a hard monthly limit on API calls like *ipstack* does.

# Architecture
 
## Design Decisions
This microservice very much follows the Clean architecture guidelines, containing the following projects:

 - `GeoLocator.Core` - core domain library containing entities/aggregate roots/interfaces
 - `GeoLocator.Shared` - any shared classes to be used by many projects
 - `GeoLocator.Infrastructure` - any external dependencies - persistence, caching, logging, 3rd party location-from-ip services
 - `GeoLocator.Web` - Composition root project along with Web API.

### Test Projects
The test projects have been organised based on the type of test (unit, integration, functional) so that they can be run in parallel/selectively in CI pipelines.

## API endpoints

This microservice provides a single public API method:

`/api/location`

If called with no parameters, it will attempt to retrieve the location information for the caller (cannot work when running locally).

It can also be called with the following parameter:

`/api/location?ipAddress={ipAddress}`

It will attempt to retrieve the location information for the provided `ipAddress`.

## Caching/Persistence

Both caching and data persistence are utilised in this microservice. Currently `IMemoryCache` provided by ASP.NET Core is supported, along with SQL Server or InMemory database via EF Core.

Whenever a request is made, the cache/persistence is initially checked for a hit, and returned if so. If not present, the 3rd party service is queried and if successful, the cache/persistence updated to include the new hit.

HTTP-based response caching is also utilised to provide a further layer of caching in an attempt to reduce response times.


# Instructions

## Hosted Service

The application has been deployed to Azure, using the Terraform configuration defined in the Terraform folder.

This API is accessible at the following URL: https://geo-locator.azurewebsites.net/api/location

There is a SwaggerUI also available at: https://geo-locator.azurewebsites.net/swagger/index.html

## Running locally

**Note**: this assumes Docker is installed locally

Running locally can be done via the `docker-compose.yaml` file. Before running, dev-certs need created to run the application over HTTPS.

### Dev Certs
Running the following in a CLI:
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\GeoLocator.Web.pfx -p password
dotnet dev-certs https --trust
```

### Switching 3rd party provider
**Note**: Only one provider can be enabled at a given time.

As there are multiple providers supported, they can be switched easily by setting the corresponding enabled flag to true in `appsettings.json`:

```json
"IpStack": {
    "Enabled": true
}

"IpApi": {
    "Enabled": true
}
```

### IpStack - Adding API key
An API key is required when using IpStack. An account should first be created [here](https://ipstack.com/product) (free is limied to 100 requests per month), and the access key added to the `appsettings.json` file, or alternatively by adding a ENV variable in `docker-compose.yaml` under the geolocator service:

```yaml
environment:
    - IpStack__AccessKey: <MY ACCESS KEY>
```

### Running in docker
Once the dev certs have been created, the application can simply be run by running when in the root directory of the solution:
```bash
docker-compose up
```

and then navigating to `https://localhost:5000/swagger/index.html` in a browser or making a GET request to one of the API endpoint defined earlier.

# Considerations/Enhancements

As with any piece of code/project, it is not perfect! There are some trade offs that were made, and there are certainly some parts that can be improved upon, including, but not limited to:

1. Caching - the current implementation uses the `IMemoryCache` provided in AspNetCore. Although this is sufficient for small/single server caching, it is not sufficient for app server farms, where non-sticky sessions are utilised. For that, some form of distributed cache should be used, such as Redis.
2. Location Lookup - currently a lookup in the persistence layer is only done against the IP address. This means that if the same IP address range is used, which will come from the same location, this will result in a new Location added to the `Locations` table. This is not ideal. Something such as the Longitude/Latitude could be used to also validate if the location exists when checking the persitence layer.
