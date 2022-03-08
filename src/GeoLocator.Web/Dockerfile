FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/GeoLocator.Web/GeoLocator.Web.csproj", "src/GeoLocator.Web/"]
RUN dotnet restore "src/GeoLocator.Web/GeoLocator.Web.csproj"
COPY . .
WORKDIR "/src/src/GeoLocator.Web"
RUN dotnet build "GeoLocator.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeoLocator.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeoLocator.Web.dll"]