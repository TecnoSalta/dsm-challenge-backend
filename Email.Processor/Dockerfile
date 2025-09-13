#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
# Instalar curl para el healthcheck
USER root
RUN apt-get update && apt-get install -y curl
USER app
WORKDIR /home/site/wwwroot
EXPOSE 7071

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Email.Processor/Email.Processor.csproj", "Email.Processor/"]
RUN dotnet restore "Email.Processor/Email.Processor.csproj"
COPY . .
WORKDIR "/src/Email.Processor"
RUN dotnet build "Email.Processor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Email.Processor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot
CMD ["dotnet", "Email.Processor.dll"]