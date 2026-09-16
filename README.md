# Employee Management App

A .NET 10 employee and project management system: two ASP.NET Core web APIs over a shared
domain, plus two console utilities for bulk data work.

[![CI](https://github.com/Menzi-Thami/EmployeeManagementApp/actions/workflows/ci.yml/badge.svg)](https://github.com/Menzi-Thami/EmployeeManagementApp/actions/workflows/ci.yml)

## What's in here

| Project | Type | Purpose |
|---|---|---|
| `Domain/` | library | `Employee`, `Project`, `JobTitle` — plain C#, no framework types |
| `Application/` | library | services and the repository interfaces they depend on |
| `Repositories/` | library | EF Core + Dapper implementations of those interfaces |
| `EmployeeApi/` | web API | the primary API |
| `EmployeeManagementApp/` | web API | an earlier API over the same domain |
| `BulkInsert/` | console | bulk-loads employee data with `SqlBulkCopy` |
| `EmployeeManagementConsoleApp.cs/` | console | interactive add-employee tool |
| `Tests/` | tests | xUnit + NSubstitute unit tests |

**On the two web APIs:** they are genuinely parallel — `EmployeeManagementApp.API` came
first and `EmployeeApi` replaced it, but both still build and neither has been retired.
Only `EmployeeApi` is in the solution; CI builds the other one explicitly so it can't rot
silently. Consolidating them is open work, not an oversight.

## Architecture

Dependencies point inward. The repository *interfaces* live in `Application`, and
`Repositories` implements them — so `Application` has no reference to the data layer, and a
test can substitute a repository without a database. `Domain` references nothing at all.

`NotFoundException` is translated to a `404` by middleware in both web apps rather than
being caught and turned into a null somewhere in the middle.

Mapping is hand-written. AutoMapper was removed deliberately — the v15 upgrade was breaking,
and the library moved to commercial licensing.

## Running it

Needs the [.NET 10 SDK](https://dotnet.microsoft.com/download) and SQL Server.

```bash
dotnet build EmployeeManagementApp/EmployeeManagementApp.sln
dotnet run --project EmployeeApi
```

Connection strings come from configuration (`appsettings.json` / user secrets), not source.
`MyDB.sql` in the repository root creates the schema.

> Note on the schema: the `Password` column is a plain `varchar(50)`. That is how the
> original script was written and it is kept here for fidelity — it is not how passwords
> should be stored. Hashing it is open work.

## Tests

```bash
dotnet test EmployeeManagementApp/EmployeeManagementApp.sln
```

## Licence

[MIT](LICENSE).
