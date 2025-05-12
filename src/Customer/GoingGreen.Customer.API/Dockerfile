FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj", "src/Customer/GoingGreen.Customer.API/"]
RUN dotnet restore "src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj"
COPY . .
WORKDIR "/src/src/Customer/GoingGreen.Customer.API"
RUN dotnet build "GoingGreen.Customer.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GoingGreen.Customer.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.Customer.API.dll"]
