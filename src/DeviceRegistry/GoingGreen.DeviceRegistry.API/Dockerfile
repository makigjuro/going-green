FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DeviceRegistry/DeviceRegistry.API/DeviceRegistry.API.csproj", "src/DeviceRegistry/DeviceRegistry.API/"]
RUN dotnet restore "src/DeviceRegistry/DeviceRegistry.API/DeviceRegistry.API.csproj"
COPY . .
WORKDIR "/src/src/DeviceRegistry/DeviceRegistry.API"
RUN dotnet build "DeviceRegistry.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DeviceRegistry.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeviceRegistry.API.dll"]
