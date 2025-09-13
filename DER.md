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


