# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/TierListAPI/TierListAPI.csproj", "API/TierListAPI/"]
COPY ["API/TierList.Application/TierList.Application.csproj", "API/TierList.Application/"]
COPY ["API/TierList.Domain/TierList.Domain.csproj", "API/TierList.Domain/"]
COPY ["API/TierList.Persistence.Postgres/TierList.Persistence.Postgres.csproj", "API/TierList.Persistence.Postgres/"]
RUN dotnet restore "./API/TierListAPI/TierListAPI.csproj"
COPY . .
WORKDIR "/src/API/TierListAPI"
RUN dotnet build "./TierListAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TierListAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TierListAPI.dll"]