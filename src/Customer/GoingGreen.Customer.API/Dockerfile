# Use the repository root as the build context

# ---------- BASE IMAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080 8081

# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all source so shared projects (e.g. ServiceDefaults) are included
COPY . .

RUN dotnet restore "src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj"

RUN dotnet build "src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ---------- PUBLISH STAGE ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "src/Customer/GoingGreen.Customer.API/GoingGreen.Customer.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

# ---------- FINAL IMAGE ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.Customer.API.dll"]
