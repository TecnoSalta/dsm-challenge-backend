#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PWC.Challenge.Api/PWC.Challenge.Api.csproj", "PWC.Challenge.Api/"]
COPY ["PWC.Challenge.Application/PWC.Challenge.Application.csproj", "PWC.Challenge.Application/"]
COPY ["PWC.Challenge.Domain/PWC.Challenge.Domain.csproj", "PWC.Challenge.Domain/"]
COPY ["PWC.Challenge.Infrastructure/PWC.Challenge.Infrastructure.csproj", "PWC.Challenge.Infrastructure/"]
RUN dotnet restore "./PWC.Challenge.Api/PWC.Challenge.Api.csproj"
COPY . .
WORKDIR "/src/PWC.Challenge.Api"
RUN dotnet build "./PWC.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./PWC.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PWC.Challenge.Api.dll"]