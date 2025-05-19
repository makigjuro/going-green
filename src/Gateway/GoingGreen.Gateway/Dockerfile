# Use repository root as build context so all dependencies (e.g., Common projects) are available
# ---------- BASE IMAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080 8081

# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy entire repository to ensure shared libraries are present
COPY . .

# Restore using the full path to the project
RUN dotnet restore "src/Gateway/GoingGreen.Gateway/GoingGreen.Gateway.csproj"

# Build the project
RUN dotnet build "src/Gateway/GoingGreen.Gateway/GoingGreen.Gateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ---------- PUBLISH STAGE ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/Gateway/GoingGreen.Gateway/GoingGreen.Gateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

# ---------- FINAL IMAGE ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.Gateway.dll"]