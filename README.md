![Build & Test](https://github.com/PhilBroderick/GeoLocator/actions/workflows/build-test.yaml/badge.svg)


# GeoLocator
Microservice API wrapper for 3rd party location by IP address services

# Supported Services
Currently only the following service is supported:
- [ipstack](https://ipstack.com/)

More services can be easily added and substituted in.

# Architecture
 
 This microservice very much follows the Clean architecture guidelines, containing the following projects:

 - GeoLocator.Core - core domain library containing entities/aggregate roots/interfaces
 - GeoLocator.Shared - any shared classes to be used by many projects
 - GeoLocator.Infrastructure - any external dependencies - persistence, caching, logging, 3rd party location-from-ip services
 - GeoLocator.Web - Composition root project along with Web API.

## API endpoints

This microservice provides 2 public API methods:

1. `/api/location` - retrieves the location information for the caller
2. `/api/location/{ipAddress}` - retrieves the location information for the provided ipAddress

## Caching/Persistence

Both caching and data persistence are utilised in this microservice. Currently `IMemoryCache` provided by ASP.NET Core is supported, along with SQL Server or InMemory database via EF Core.

Whenever a request is made, the cache/persistence is initially checked for a hit, and returned if so. If not present, the 3rd party service is queried and if successful, the cache/persistence updated to include the new hit.

HTTP-based response caching is also utilised to provide a further layer of caching in an attempt to reduce response times.


# Instructions

## Hosted Service

The application has been deployed to Azure, using the Terraform configuration defined in the Terraform folder.

This API is accessible at the following URL: https://geo-locator.azurewebsites.net/api/location

## Running locally

**Note**: this assumes Docker is installed locally

Running locally can be done via the `docker-compose.yaml` file. Before running, dev-certs need created to run the application over HTTPS.

### Dev Certs
Running the following in a CLI:
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\GeoLocator.Web.pfx -p password
dotnet dev-certs https --trust
```

### Running in docker
Once the dev certs have been created, the application can simply be run by running when in the root directory of the solution:
```bash
docker-compose up
```

and then navigating to `https://localhost:5000/swagger/index.html` in a browser or making a GET request to one of the API endpoint defined earlier.
