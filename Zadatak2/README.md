## Big picture arhitektura

https://mermaid.live

```mermaid
flowchart LR
    User[Korisnik garaže] --> EntryGate[Ulazna rampa]
    EntryGate --> Backend[Parking sustav]
    Backend --> Database[(Baza podataka)]
    Backend --> PaymentMachine[Naplatni aparat]
    PaymentMachine --> Backend
    Backend --> ExitGate[Izlazna rampa]
```

## Proces ulaska, naplate i izlaska

```mermaid
flowchart TD
    Start([Vozilo dolazi na ulaz]) --> IdentifyEntry[Ocitaj identifikator korisnika]
    IdentifyEntry --> HasIdentity{Identitet je valjan?}
    HasIdentity -- Ne --> DenyEntry[Odbij ulaz i prikazi razlog]
    HasIdentity -- Da --> HasCapacity{Ima slobodnih mjesta?}

    HasCapacity -- Ne --> GarageFull[Prikazi da je garaza puna]
    HasCapacity -- Da --> ContractCheck{Korisnik ima vazeci ugovor?}

    ContractCheck -- Da --> OpenEntryContract[Otvori ulaznu rampu]
    ContractCheck -- Ne --> IssueTicket[Kreiraj parking sesiju]
    IssueTicket --> OpenEntryTicket[Otvori ulaznu rampu]

    OpenEntryContract --> AssignSpot[Dodijeli ili evidentiraj parkirno mjesto]
    OpenEntryTicket --> AssignSpot
    AssignSpot --> UpdateCapacity[Umanji broj slobodnih mjesta]
    UpdateCapacity --> ActiveSession[Parking sesija je aktivna]

    ActiveSession --> UserPays[Korisnik placa na naplatnom aparatu]
    UserPays --> CalculatePrice[Izracunaj cijenu prema trajanju, tarifi i akcijama]
    CalculatePrice --> PaymentSuccess{Naplata uspjesna?}
    PaymentSuccess -- Ne --> PaymentRetry[Omoguci ponovni pokusaj naplate]
    PaymentRetry --> UserPays
    PaymentSuccess -- Da --> MarkPaid[Oznaci sesiju kao placenu]

    MarkPaid --> SetGracePeriod[Postavi rok za izlaz: placeno vrijeme + 10 min]
    SetGracePeriod --> VehicleAtExit[Vozilo dolazi na izlaz]
    VehicleAtExit --> IdentifyExit[Ocitaj identifikator korisnika ili sesije]
    IdentifyExit --> ExitAllowed{Unutar roka za izlaz?}

    ExitAllowed -- Da --> OpenExit[Otvori izlaznu rampu]
    OpenExit --> CloseSession[Zatvori parking sesiju]
    CloseSession --> FreeSpot[Uvecaj broj slobodnih mjesta]
    FreeSpot --> End([Proces zavrsen])

    ExitAllowed -- Ne --> DenyExit[Odbij izlaz]
    DenyExit --> ExtraPayment[Uputi korisnika na naplatni aparat za doplatu]
    ExtraPayment --> UserPays
```

## Obracun cijene i kisnog popusta

```mermaid
flowchart TD
    Start([Pokreni obracun naplate]) --> LoadSession[Ucitaj parking sesiju]
    LoadSession --> ValidateSession{Sesija je aktivna?}
    ValidateSession -- Ne --> StopInvalid[Prekini obracun i prikazi gresku]
    ValidateSession -- Da --> LoadTariff[Ucitaj vazecu tarifu]

    LoadTariff --> CalculateDuration[Izracunaj trajanje parkiranja]
    CalculateDuration --> RoundHours[Zaokruzi obracunske sate prema pravilima tarife]
    RoundHours --> CheckSpotType{Mjesto je nenatkriveno?}

    CheckSpotType -- Ne --> CoveredPrice[Primijeni cijenu natkrivenog mjesta]
    CheckSpotType -- Da --> LoadWeather[Ucitaj zapise o kisi za vrijeme parkiranja]

    LoadWeather --> RainRatio[Izracunaj udio vremena provedenog na kisi]
    RainRatio --> RainThreshold{Kisa >= 33% parkiranog vremena?}

    RainThreshold -- Ne --> UncoveredRegular[Primijeni redovnu cijenu nenatkrivenog mjesta]
    RainThreshold -- Da --> RainDiscount[Primijeni kisni popust 50%]

    CoveredPrice --> BaseAmount[Izracunaj osnovni iznos]
    UncoveredRegular --> BaseAmount
    RainDiscount --> DiscountedAmount[Izracunaj umanjeni iznos]

    BaseAmount --> CreatePaymentPreview[Prikazi iznos korisniku]
    DiscountedAmount --> CreatePaymentPreview

    CreatePaymentPreview --> UserConfirms{Korisnik potvrduje placanje?}
    UserConfirms -- Ne --> CancelPayment[Odustani od naplate]
    UserConfirms -- Da --> ProcessPayment[Provedi naplatu na aparatu]

    ProcessPayment --> PaymentOk{Naplata uspjesna?}
    PaymentOk -- Ne --> PaymentFailed[Zabiljezi neuspjelu naplatu]
    PaymentOk -- Da --> SavePayment[Spremi uplatu i primijenjeni popust]
    SavePayment --> SetExitDeadline[Postavi rok za izlaz: 10 minuta od placanja]
    SetExitDeadline --> End([Obracun zavrsen])
```

## Mjesecni reporting i poslovna analiza

```mermaid
flowchart TD
    Start([Pocetak mjesecne obrade]) --> SelectPeriod[Odredi izvjestajni mjesec]
    SelectPeriod --> LoadPayments[Ucitaj sve uspjesne naplate]
    SelectPeriod --> LoadSessions[Ucitaj sve parking sesije]
    SelectPeriod --> LoadCapacity[Ucitaj kapacitete garaze i katova]
    SelectPeriod --> LoadDiscounts[Ucitaj primijenjene kisne popuste]
    SelectPeriod --> LoadContracts[Ucitaj ugovorne korisnike i obracune]

    LoadPayments --> Revenue[Izracunaj ukupni prihod]
    LoadSessions --> Occupancy[Izracunaj prosjecnu i vrsnu popunjenost]
    LoadCapacity --> Occupancy
    LoadDiscounts --> RainCampaign[Izracunaj trosak i ucinak kisne akcije]
    LoadContracts --> ContractRevenue[Izracunaj prihod ugovornih korisnika]

    Revenue --> BuildReport[Slozi mjesecni izvjestaj]
    Occupancy --> BuildReport
    RainCampaign --> BuildReport
    ContractRevenue --> BuildReport

    BuildReport --> ComparePrevious[Usporedi s prethodnim mjesecom]
    ComparePrevious --> StoreReport[Spremi agregirane rezultate]
    StoreReport --> OwnerDashboard[Prikazi izvjestaj vlasniku garaze]
    OwnerDashboard --> ExportReport{Potreban export?}

    ExportReport -- Ne --> End([Izvjestaj zavrsen])
    ExportReport -- Da --> ExportPdf[Generiraj PDF ili Excel izvjestaj]
    ExportPdf --> End
```

## Idejni model baze podataka

```mermaid
erDiagram
    GARAGE ||--|{ FLOOR : contains
    FLOOR ||--|{ PARKING_SPOT : contains
    FLOOR ||--o{ PAYMENT_MACHINE : has
    PARKING_SPOT ||--o{ PARKING_SESSION : is_used_in
    CUSTOMER ||--o{ ACCESS_CREDENTIAL : owns
    CUSTOMER ||--o{ CONTRACT : signs
    CUSTOMER ||--o{ PARKING_SESSION : starts
    CONTRACT ||--o{ PARKING_SESSION : authorizes
    ACCESS_CREDENTIAL ||--o{ PARKING_SESSION : identifies
    PARKING_SESSION ||--o{ PAYMENT : is_paid_by
    PAYMENT_MACHINE ||--o{ PAYMENT : processes
    TARIFF ||--o{ PRICING_RULE : defines
    TARIFF ||--o{ PAYMENT : calculates
    PARKING_SESSION ||--o{ SESSION_WEATHER_SAMPLE : has
    WEATHER_OBSERVATION ||--o{ SESSION_WEATHER_SAMPLE : contributes_to
    GARAGE ||--o{ MONTHLY_REPORT : produces

    GARAGE {
        int GarageId PK
        string Name
        string Address
        int TotalCapacity
        bool IsActive
    }

    FLOOR {
        int FloorId PK
        int GarageId FK
        int LevelNumber
        int Capacity
        int AvailableSpots
    }

    PARKING_SPOT {
        int ParkingSpotId PK
        int FloorId FK
        string SpotCode
        bool IsCovered
        string Status
    }

    CUSTOMER {
        int CustomerId PK
        string CustomerType
        string DisplayName
        string IdentityReference
        string Email
        bool IsActive
    }

    ACCESS_CREDENTIAL {
        int AccessCredentialId PK
        int CustomerId FK
        string CredentialType
        string Identifier
        datetime IssuedAtUtc
        string Status
    }

    CONTRACT {
        int ContractId PK
        int CustomerId FK
        datetime ValidFromUtc
        datetime ValidToUtc
        string BillingModel
        string Status
    }

    PARKING_SESSION {
        int ParkingSessionId PK
        int ParkingSpotId FK
        int CustomerId FK
        int AccessCredentialId FK
        int ContractId FK
        datetime EntryAtUtc
        datetime PaidUntilUtc
        datetime ExitGraceUntilUtc
        datetime ExitAtUtc
        string Status
    }

    PAYMENT {
        int PaymentId PK
        int ParkingSessionId FK
        int PaymentMachineId FK
        int TariffId FK
        decimal Amount
        decimal DiscountAmount
        datetime PaidAtUtc
        string PaymentMethod
    }

    PAYMENT_MACHINE {
        int PaymentMachineId PK
        int FloorId FK
        string MachineCode
        string Status
    }

    TARIFF {
        int TariffId PK
        string Name
        decimal HourlyRateCovered
        decimal HourlyRateUncovered
        datetime ValidFromUtc
        datetime ValidToUtc
    }

    PRICING_RULE {
        int PricingRuleId PK
        int TariffId FK
        string RuleType
        decimal DiscountPercent
        string ConditionExpression
        bool IsActive
    }

    WEATHER_OBSERVATION {
        int WeatherObservationId PK
        datetime ObservedAtUtc
        bool IsRaining
        string Source
    }

    SESSION_WEATHER_SAMPLE {
        int ParkingSessionId FK
        int WeatherObservationId FK
        int RainMinutes
    }

    MONTHLY_REPORT {
        int MonthlyReportId PK
        int GarageId FK
        int Year
        int Month
        decimal TotalRevenue
        decimal AverageOccupancyPercent
        decimal RainDiscountAmount
    }
```
