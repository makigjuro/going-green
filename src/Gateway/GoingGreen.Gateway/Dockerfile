# ---------- BASE IMAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Optional: define a non-root user
# ARG APP_UID=1000
# RUN groupadd -g $APP_UID appgroup && useradd -r -u $APP_UID -g appgroup appuser
# USER appuser
WORKDIR /app
EXPOSE 8080 8081

# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG SERVICE_NAME
WORKDIR /src

# 1) Copy only the project file for cached restore
COPY ["GoingGreen.Gateway.csproj", "./"]
RUN dotnet restore "GoingGreen.Gateway.csproj"

# 2) Copy the rest of the source code
COPY . .

# 3) Build the project
RUN dotnet build "GoingGreen.Gateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build

# ---------- PUBLISH STAGE ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG SERVICE_NAME
RUN dotnet publish "GoingGreen.Gateway.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

# ---------- FINAL IMAGE ----------
FROM base AS final
ARG SERVICE_NAME
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoingGreen.Gateway.dll"]
