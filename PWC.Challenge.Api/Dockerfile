# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Instalar curl para el healthcheck
USER root
RUN apt-get update && apt-get install -y curl
USER $APP_UID

WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PWC.Challenge.Api/PWC.Challenge.Api.csproj", "PWC.Challenge.Api/"]
COPY ["PWC.Challenge.Application/PWC.Challenge.Application.csproj", "PWC.Challenge.Application/"]
COPY ["PWC.Challenge.Common/PWC.Challenge.Common.csproj", "PWC.Challenge.Common/"]
COPY ["PWC.Challenge.Domain/PWC.Challenge.Domain.csproj", "PWC.Challenge.Domain/"]
COPY ["PWC.Challenge.Infrastructure/PWC.Challenge.Infrastructure.csproj", "PWC.Challenge.Infrastructure/"]
RUN dotnet restore "./PWC.Challenge.Api/PWC.Challenge.Api.csproj"
COPY . .
WORKDIR "/src/PWC.Challenge.Api"
RUN dotnet build "./PWC.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PWC.Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD curl -f http://localhost:8080/healthz || exit 1

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PWC.Challenge.Api.dll"]