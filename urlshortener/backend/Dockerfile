# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY UrlShortener.API/UrlShortener.API.csproj ./UrlShortener.API/
RUN dotnet restore ./UrlShortener.API/UrlShortener.API.csproj

COPY UrlShortener.API/ ./UrlShortener.API/
RUN dotnet publish ./UrlShortener.API/UrlShortener.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create data directory for SQLite persistence
RUN mkdir -p /data && chmod 777 /data

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]
