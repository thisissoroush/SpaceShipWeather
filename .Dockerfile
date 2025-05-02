# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.sln .
COPY SpaceShipWeather.Api/*.csproj ./SpaceShipWeather.Api/
RUN dotnet restore

# Copy the rest of the files and build the app
COPY . .
WORKDIR /app/SpaceShipWeather.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create folder for SQLite DB
RUN mkdir -p /app/DataBase/Data

# Copy published output
COPY --from=build /app/publish .

# Set environment variable for SQLite location (optional, if used by IConfiguration)
ENV ConnectionStrings__Default="Data Source=/app/DataBase/Data/Weather.db"

# Expose port (adjust if needed)
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "SpaceShipWeather.Api.dll"]
