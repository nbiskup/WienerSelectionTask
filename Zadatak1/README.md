# Zadatak 1 - Partner Management

ASP.NET Core MVC web application for managing insurance partners and partner policies.

## Technology

- ASP.NET Core MVC, .NET 8
- Dapper Micro ORM
- SQL Server LocalDB with T-SQL schema script
- Bootstrap 4 UI
- jQuery for modal/AJAX policy entry

## Features

- Partner list sorted by `CreatedAtUtc` descending.
- Full name display from `FirstName` + `LastName`.
- Clickable partner row with Bootstrap modal details.
- New partner form with validation for all required fields.
- Partner policy entry through a Bootstrap modal dialog.
- Partner is marked with `*` when it has more than 5 policies or total policy amount greater than 5000.
- New partner row is visually highlighted after successful redirect.
- Database is created automatically on application startup.

## Run locally

Prerequisites:

- .NET 8 SDK/runtime or newer SDK with .NET 8 runtime installed.
- SQL Server LocalDB.

Commands:

```powershell
dotnet restore .\Zadatak1\PartnerManagement\PartnerManagement.csproj
dotnet run --project .\Zadatak1\PartnerManagement\PartnerManagement.csproj
```

Open the URL printed by `dotnet run` in the browser.

## Database

The application uses the connection strings from `PartnerManagement/appsettings.json` and creates the `PartnerManagementDb` LocalDB database if it does not exist.

The T-SQL schema is located at:

```text
Zadatak1/PartnerManagement/Data/Scripts/001_create_schema.sql
```