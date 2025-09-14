#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
# Instala las herramientas necesarias: npm para Azurite
USER root
RUN apt-get update && apt-get install -y npm
RUN npm install -g azurite
USER app
WORKDIR /home/site/wwwroot
EXPOSE 7071 10000 10001 10002

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
COPY start.sh /
RUN chmod +x /start.sh
ENV AzureWebJobsScriptRoot=/home/site/wwwroot
ENTRYPOINT ["/start.sh"]