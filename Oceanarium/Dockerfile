# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Oceanarium.csproj", "./"]
RUN dotnet restore "./Oceanarium.csproj"
COPY . .
RUN dotnet build "./Oceanarium.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN mkdir -p /data
COPY Data/Oceanarium.db /data/

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Oceanarium.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER root
RUN mkdir -p /data && chmod -R 777 /data
COPY --from=publish /app/publish .
COPY --from=publish /data /data
RUN chmod -R 777 /data/Oceanarium.db
USER $APP_UID

ENTRYPOINT ["dotnet", "Oceanarium.dll"]

