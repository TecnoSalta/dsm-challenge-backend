# PWC.Challenge

Motor DB PostgresSQL version 10 (Dockerizado) 
Creacion de PWChallenge (Db)
Correr las migraciones de update database y luego
ejecutar /docs/create-sample-car-services.sql usando cliente sql (Dbeaver)

## ✅ Testing & Coverage

Ejecutar los tests con cobertura de código:

```bash
dotnet test PWC.Challenge.sln --collect:"XPlat Code Coverage" --results-directory ./CVR --logger "console;verbosity=detailed"
```

Generar reporte HTML de cobertura:

```bash
reportgenerator -reports:./CVR/**/coverage.cobertura.xml -targetdir:./CoverHtmlV2 -reporttypes:Html
```

El reporte quedará en **`./CoverHtmlV2/index.html`** y se puede abrir en cualquier navegador.  

---

## 🗄️ EF Core Commands

- **Crear migración inicial**  
  ```powershell
  Add-Migration Initial -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext -OutputDir Data/Migrations
  ```

- **Actualizar base de datos**  
  ```powershell
  Update-Database -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext
  ```

- **Eliminar última migración**  
  ```powershell
  Remove-Migration -StartupProject PWC.Challenge.Api -Project PWC.Challenge.Infrastructure -Context ApplicationDbContext
  ```
DOCKER--> Usar Docker Compose para levantar el proyecto debe estar instalado Docker Desktop.
---

## 🏗️ Project structure (Clean Architecture + DDD)

**PWC.Challenge (Solution)**  

- **PWC.Challenge.Common**  
  _Clase utilitaria y lógica común compartida._  

- **PWC.Challenge.Api**  
  _Proyecto `ASP.NET Web API`._  
  Contiene controladores, middlewares, configuración y referencia a:
  - `PWC.Challenge.Application`
  - `PWC.Challenge.Infrastructure`

- **PWC.Challenge.Domain**  
  _Core del dominio (DDD)._  
  - Entidades  
  - Value Objects  
  - Aggregates  
  - Eventos de dominio  

- **PWC.Challenge.Application**  
  _Capa de aplicación (CQRS/MediatR)._  
  - DTOs  
  - Commands / Queries  
  - Handlers  
  - Casos de uso  
  - Reglas de aplicación  

  Contiene referencia a:
  - `PWC.Challenge.Common`
  - `PWC.Challenge.Domain`

- **PWC.Challenge.Infrastructure**  
  _Persistencia, EF Core, repositorios, servicios externos._  
  Contiene referencia a:
  - `PWC.Challenge.Application`

---

## 🚀 Publish Commands

Publicar con configuración QA (Windows x64):  

```bash
dotnet publish -c Release -r win-x64 -o ./bin/Release/net8.0/publish-qa /p:EnvironmentName=QA
```

Otros entornos pueden configurarse cambiando `/p:EnvironmentName=QA`.  

---

## 🔒 Authentication & Authorization

- Autenticación vía **JWT Bearer**.  
- Autorización por **roles (Admin, Customer)** en los controladores con `[Authorize(Roles = "...")]`.  

---

## 📌 Endpoints principales

- **POST `/api/rentals`** → Crear reserva (`201 Created`)  
- **GET `/api/rentals/{id}`** → Obtener reserva por Id  
- **GET `/api/rentals`** → Listar reservas existentes  
- **PUT `/api/rentals/{id}`** → Actualizar reserva  
- **DELETE `/api/rentals/{id}`** → Cancelar reserva  
- **POST `/api/rentals/{id}/complete`** → Completar una reserva  
