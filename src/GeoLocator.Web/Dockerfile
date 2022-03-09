FROM mcr.microsoft.com/dotnet/sdk:6.0-focal as build
EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY src/GeoLocator.Web/*.csproj ./GeoLocator.Web/
COPY src/GeoLocator.Infrastructure/*.csproj ./GeoLocator.Infrastructure/
COPY src/GeoLocator.Core/*.csproj ./GeoLocator.Core/
COPY src/GeoLocator.Shared/*.csproj ./GeoLocator.Shared/

RUN dotnet restore "GeoLocator.Web/GeoLocator.Web.csproj"

COPY src/GeoLocator.Web/. ./GeoLocator.Web/
COPY src/GeoLocator.Infrastructure/. ./GeoLocator.Infrastructure/
COPY src/GeoLocator.Core/. ./GeoLocator.Core/
COPY src/GeoLocator.Shared/. ./GeoLocator.Shared/

RUN dotnet publish "GeoLocator.Web/GeoLocator.Web.csproj" -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "GeoLocator.Web.dll"]