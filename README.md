# WT.Ticketing

  
Testing Coverage:
dotnet test PWC.Challenge.sln --collect:"XPlat Code Coverage" --results-directory ./CVR --logger "console;verbosity=detailed"

reportgenerator -reports:./CVR/**/coverage.cobertura.xml -targetdir:./CoverHtmlV2 -reporttypes:Html
## EF Core Commands

  

    1. Create migration command: Add-Migration Initial -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext -OutputDir Data/Migrations

    2. Update database command:  Update-Database -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext


	Remove-Migration -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext

## Project structure (Clean Architecture + DDD)

  

    PWC.Challenge (Solución Visual Studio)

    - WT.Common (Proyecto de tipo Class Library)

    - PWC.Challenge.Api (Proyecto de tipo ASP.NET Web API)

        Contiene referencia a los proyectos: PWC.Challenge.Application y PWC.Challenge.Infrastructure

    - PWC.Challenge.Domain (Proyecto de tipo Class Library)

    - PWC.Challenge.Application (Proyecto de tipo Class Library)

        Contiene referencia a los proyectos: PWC.Challenge.Common y PWC.Challenge.Domain

    - PWC.Challenge.Infrastructure (Proyecto de tipo Class Library)

        Contiene referencia a los proyectos: PWC.Challenge.Application

    - PWC.Challenge.Worker (Proyecto de tipo ASP.NET Web API): Es utilizado para registrar consumidores de procesos gestionados por un message broker (RabbitMQ)

        Contiene referencia a los proyectos: PWC.Challenge.Application y PWC.Challenge.Infrastructure

  

## Publish commands

  

    - Windows compatibiity command with QA environment

        dotnet publish -c Release -r win-x64 -o ./bin/Release/net8.0/publish-qa /p:EnvironmentName=QA