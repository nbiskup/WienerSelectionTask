# Zadatak 2 - Sustav za vodjenje parkiralista

## Cilj sustava

Potrebno je osmisliti sustav za vodjenje garaze koji omogucuje naplatu parkiranja prema vremenu zauzetosti parkirnog mjesta, kontrolu ulaza i izlaza, pracenje slobodnih mjesta te mjesecni uvid u poslovanje garaze.

Sustav mora podrzavati placanje na naplatnim aparatima po katovima, ugovorne korisnike, zabranu placanja na izlazu, pravilo da korisnik ima 10 minuta za izlaz nakon placanja te posebnu akciju u kojoj su nenatkrivena mjesta u kisnim uvjetima 50% jeftinija ako je vozilo barem 33% parkiranog vremena bilo na kisi.

Najkriticniji dio sustava je naplata parkiranja. Funkcionalnosti poput kisne akcije i prikaza slobodnih mjesta po katu su korisne, ali ne smiju ugroziti stabilnost naplate, evidenciju ulaza/izlaza i osnovnu kontrolu pristupa.

## Kljucni dijelovi sustava

| Dio sustava | Odgovornost |
| --- | --- |
| Ulazna rampa | Identifikacija korisnika, provjera kapaciteta, otvaranje ulaza i kreiranje parking sesije. |
| Izlazna rampa | Provjera placanja, provjera roka od 10 minuta nakon placanja i zatvaranje parking sesije. |
| Naplatni aparat | Izracun cijene, provjera aktivne sesije, provedba placanja i izdavanje potvrde. |
| Backend/API | Centralna poslovna logika za sesije, naplatu, tarife, ugovore, kapacitete i izvjestaje. |
| Baza podataka | Trajna pohrana garaze, katova, mjesta, korisnika, sesija, uplata, tarifa i izvjestaja. |
| Modul za tarife | Pravila obracuna cijene, ugovorni modeli, popusti i buduce akcije. |
| Weather servis | Evidencija kisnih razdoblja za obracun popusta na nenatkrivenim mjestima. |
| Reporting modul | Mjesecni izvjestaji o prihodu, popunjenosti, ugovornim korisnicima i isplativosti akcija. |
| Admin sucelje | Upravljanje garazom, parkirnim mjestima, tarifama, ugovorima i pregled izvjestaja. |
| Display slobodnih mjesta | Prikaz ukupnog broja slobodnih mjesta i, po mogucnosti, slobodnih mjesta po katu. |

## Prioriteti zahtjeva

| Prioritet | Zahtjev | Obrazlozenje |
| --- | --- | --- |
| Kriticno | Naplata parkiranja | Prihod garaze i izlaz vozila ovise o ispravnoj naplati. |
| Kriticno | Evidencija ulaza i izlaza | Bez pouzdane sesije nije moguce tocno naplatiti parking. |
| Kriticno | Identitet korisnika | Klijent izricito trazi osiguranje identiteta korisnika garaze. |
| Visoko | Ukupan broj slobodnih mjesta | Potencijalnim korisnicima je vazno znati ima li mjesta u garazi. |
| Srednje | Broj slobodnih mjesta po katu | Korisno za navigaciju, ali nije primarni proces. |
| Srednje | Mjesecni izvjestaji | Vazno za vlasnika i poslovnu analizu rada garaze. |
| Nize | Kisna akcija | Korisna marketinska akcija, ali ne smije ugroziti osnovni proces naplate. |

## Kljucni procesi

1. Ulazak vozila u garazu i pokretanje parking sesije.
2. Evidencija zauzimanja parkirnog mjesta i azuriranje kapaciteta.
3. Obracun cijene prema trajanju parkiranja, tipu mjesta, tarifi i mogucim akcijama.
4. Placanje na naplatnom aparatu ili obracun prema ugovoru.
5. Izlazak iz garaze unutar 10 minuta nakon placanja.
6. Dodatna naplata ako korisnik zakasni s izlazom nakon placanja.
7. Prikupljanje podataka za mjesecne izvjestaje i poslovnu analizu.

## Otvorena pitanja i pretpostavke

Specifikacija ostavlja nekoliko detalja otvorenima. U nastavku su navedene pretpostavke koje se koriste za idejno rjesenje, uz pitanja koja bi trebalo potvrditi s klijentom prije detaljne implementacije.

| Tema | Pretpostavka / pitanje |
| --- | --- |
| Identifikacija korisnika | Pretpostavlja se da se korisnik identificira karticom, ticketom, QR kodom ili registarskom oznakom. Tocan mehanizam treba potvrditi. |
| Placanje na izlazu | Placanje na izlazu nije dopusteno. Izlazna rampa samo provjerava status placanja i rok za izlaz. |
| Ugovorni korisnici | Ugovorni korisnici mogu imati drugaciji model naplate, npr. mjesecni obracun, pretplatu ili dodijeljena prava pristupa. |
| Dodjela mjesta | Sustav moze pratiti tocno parkirno mjesto ili samo kat/zonu. Za kisni popust treba znati je li mjesto natkriveno. |
| Kisni podaci | Pretpostavlja se integracija s vremenskim servisom ili lokalnim senzorom kise. Potrebno je definirati izvor istine za kisne intervale. |
| Slobodna mjesta po katu | Prikaz po katu je pozeljan, ali nije kritican. Ukupan broj slobodnih mjesta ima veci prioritet. |
| Reporti | Potrebno je potvrditi zeljeni format izvjestaja, npr. dashboard, PDF, Excel ili automatska mjesecna e-mail dostava. |
| Vrste korisnika | Specifikacija spominje mogucnost vise vrsta korisnika, pa model treba omoguciti prosirenje bez promjene osnovne naplate. |

## Potencijalni problemi i rizici

| Rizik | Utjecaj | Predlozeno rjesenje |
| --- | --- | --- |
| Kvar naplatnog aparata | Korisnik ne moze platiti i ne moze izaci iz garaze. | Vise aparata po garazi, health-check aparata, fallback na drugi aparat i administrativni override uz audit log. |
| Nedostupnost payment providera | Naplata ne prolazi iako je sustav garaze ispravan. | Jasno stanje neuspjele naplate, ponovni pokusaj, evidentiranje transakcijskog statusa i zabrana otvaranja izlaza dok naplata nije potvrdena. |
| Nedostupnost centralnog backend-a | Ulaz, izlaz i naplata mogu stati. | Visoka dostupnost backend-a, lokalni cache minimalnih pravila na rampama/aparatima i sinkronizacija nakon oporavka. |
| Neispravan broj slobodnih mjesta | Korisnici dobivaju krivu informaciju o dostupnosti garaze. | Brojanje temeljiti na aktivnim sesijama, periodicna rekonsilijacija sa senzorima/rampama i rucna korekcija uz audit log. |
| Korisnik zakasni izaci nakon placanja | Nastaje spor oko dodatne naplate. | Jasno spremiti `PaidUntilUtc` i `ExitGraceUntilUtc`, prikazati rok na potvrdi i na izlazu traziti doplatu ako je rok istekao. |
| Nepouzdan weather servis | Kisni popust moze biti pogresno primijenjen. | Spremati vremenske uzorke u bazu, koristiti lokalni senzor ili pouzdan servis i omoguciti audit izracuna popusta. |
| Nejasan identitet korisnika | Tesko je dokazati tko koristi uslugu ili ugovor. | Uvesti jedinstveni access credential, povezati ga s korisnikom/ugovorom i logirati ulazne/izlazne dogadaje. |
| Zlouporaba kartice ili ticketa | Moguc neovlasteni izlaz ili prijenos prava pristupa. | Jedna aktivna sesija po credentialu, status credentiala, validacija na ulazu i izlazu te blokiranje sumnjivih stanja. |
| Promjena tarifa tijekom aktivne sesije | Moze nastati nejasan obracun cijene. | Verzionirati tarife i spremiti referencu na tarifu/ruleset koji vrijedi za obracun sesije. |
| GDPR i osobni podaci | Identitet korisnika i ugovori mogu ukljucivati osobne podatke. | Minimalna pohrana osobnih podataka, kontrola pristupa, audit log i definirani rokovi cuvanja podataka. |
| Mjesecni reporti koriste nepotpune podatke | Vlasnik dobiva pogresnu poslovnu sliku. | Reporte graditi iz zakljucanih uplata i zatvorenih sesija, oznaciti nepotpune podatke i cuvati agregirane rezultate. |
| Buduce akcije osim kisne | Hardkodirana kisna akcija otezava prosirenje. | Uvesti modul za pricing rules kako bi se kasnije dodale nove akcije bez promjene osnovnog procesa naplate. |

## Big picture arhitektura

```mermaid
flowchart LR
    User[Korisnik garaze] --> EntryGate[Ulazna rampa]
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
