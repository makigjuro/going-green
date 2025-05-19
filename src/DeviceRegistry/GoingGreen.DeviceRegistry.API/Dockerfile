# ---------- BASE IMAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080 8081

# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .

RUN dotnet restore "src/DeviceRegistry/GoingGreen.DeviceRegistry.API/GoingGreen.DeviceRegistry.API.csproj"

RUN dotnet build "src/DeviceRegistry/GoingGreen.DeviceRegistry.API/GoingGreen.DeviceRegistry.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ---------- PUBLISH STAGE ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "src/DeviceRegistry/GoingGreen.DeviceRegistry.API/GoingGreen.DeviceRegistry.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

# ---------- FINAL IMAGE ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.DeviceRegistry.API.dll"]
