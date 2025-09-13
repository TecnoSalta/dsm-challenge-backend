# PWC.Challenge Architecture (Dev Version)

## 1. Visi�n general
El sistema **Car Rental** permite gestionar el alquiler de autos en el aeropuerto, ofreciendo:  
- Reservas con selecci�n de veh�culo y per�odo de alquiler.  
- Modificaci�n y cancelaci�n de reservas.  
- Control de disponibilidad de autos y mantenimiento preventivo.  
- Restricciones de negocio: cooldown de 1 d�a post-alquiler, servicio cada 2 meses, un auto por cliente a la vez.

El objetivo es mantener un c�digo organizado y escalable mediante **DDD + Clean Architecture**.

---

## 2. Estilo arquitect�nico

### Capas principales
| Capa            | Contenido principal                                     |
|-----------------|--------------------------------------------------------|
| **Domain**      | Entidades, Value Objects, Eventos de Dominio, Repositorios, Excepciones. |
| **Application** | Casos de uso, Commands, Queries, DTOs, Servicios de aplicaci�n. |
| **Infrastructure** | Persistencia (EF Core), repositorios, servicios externos, cache. |
| **API**         | Controllers, endpoints REST, inyecci�n de dependencias. |

### Convenciones
- Domain **no depende** de ninguna otra capa.  
- Application depende solo de Domain.  
- Infrastructure implementa interfaces de Domain/Application.  
- Eventos de dominio se disparan dentro de agregados y manejados por EventHandlers.  

---

## 3. Modelo de dominio

```mermaid
erDiagram
    CUSTOMERS ||--o{ RENTALS : "realiza"
    CARS ||--o{ RENTALS : "es_rentado_en"
    CARS ||--o{ SERVICES : "recibe"

    CUSTOMERS {
        uuid Id PK
        varchar FullName
        varchar Address
        varchar Email
        timestamptz CreatedAt
        text CreatedBy
        timestamptz UpdatedAt
        text UpdatedBy
        timestamptz DeletedAt
        text DeletedBy
        boolean IsDeleted
    }

    CARS {
        uuid Id PK
        varchar Type
        varchar Model
        numeric DailyRate
        text Status
        timestamptz CreatedAt
        text CreatedBy
        timestamptz UpdatedAt
        text UpdatedBy
        timestamptz DeletedAt
        text DeletedBy
        boolean IsDeleted
    }

    RENTALS {
        uuid Id PK
        uuid CustomerId FK
        uuid CarId FK
        text Status
        date StartDate
        date EndDate
        numeric TotalCost
        numeric DailyRate
        date ActualReturnDate
        timestamptz CreatedAt
        text CreatedBy
        timestamptz UpdatedAt
        text UpdatedBy
        timestamptz DeletedAt
        text DeletedBy
        boolean IsDeleted
    }

    SERVICES {
        uuid Id PK
        date Date
        integer DurationDays
        uuid CarId FK
    }
