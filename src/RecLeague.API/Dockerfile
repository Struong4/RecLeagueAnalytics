# Stage 1 — Build
# Uses the full SDK image to restore packages and compile the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first so NuGet restore is cached by Docker
# (if only .cs files change, Docker reuses the cached restore layer)
COPY RecLeagueAnalytics.sln ./
COPY src/RecLeague.API/RecLeague.API.csproj                                         src/RecLeague.API/
COPY src/RecLeague.Application/RecLeague.Application.csproj                         src/RecLeague.Application/
COPY src/RecLeague.Infrastructure/RecLeague.Infrastructure.csproj                   src/RecLeague.Infrastructure/
COPY src/RecLeague.Domain/RecLeague.Domain.csproj                                   src/RecLeague.Domain/
COPY tests/RecLeague.Application.Tests/RecLeague.Application.Tests.csproj           tests/RecLeague.Application.Tests/

RUN dotnet restore RecLeagueAnalytics.sln

# Copy everything else and publish a Release build
COPY . .
RUN dotnet publish src/RecLeague.API/RecLeague.API.csproj -c Release -o /app/publish

# Stage 2 — Runtime
# Uses a smaller image that only has the .NET runtime, not the full SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# The API listens on port 8080 inside the container
EXPOSE 8080

ENTRYPOINT ["dotnet", "RecLeague.API.dll"]
