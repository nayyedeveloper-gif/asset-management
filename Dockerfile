# Use the official .NET 9.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["AMS.csproj", "./"]
RUN dotnet restore "AMS.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "AMS.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/ || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "AMS.dll"] 