# Use multi-stage build to optimize image size
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Assessment/GoingGreen.Assessment.API/GoingGreen.Assessment.API.csproj", "src/Assessment/GoingGreen.Assessment.API/"]
COPY ["src/Assessment/GoingGreen.Assessment.Application/GoingGreen.Assessment.Application.csproj", "src/Assessment/GoingGreen.Assessment.Application/"]
COPY ["src/Common/GoingGreen.ServiceDefaults/GoingGreen.ServiceDefaults.csproj", "src/Common/GoingGreen.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "src/Assessment/GoingGreen.Assessment.API/GoingGreen.Assessment.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/src/Assessment/GoingGreen.Assessment.API"
RUN dotnet build "GoingGreen.Assessment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoingGreen.Assessment.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.Assessment.API.dll"]