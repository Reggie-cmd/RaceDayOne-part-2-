# RaceDay API notes

- Framework: .NET 8 ASP.NET Core Web API.
- Persistence: Entity Framework Core SQL Server provider.
- Authentication: secure HTTP-only cookie, role claims, organiser/participant policies.
- Public event/category reads do not require login. All profile, enrolment, results and management endpoints are protected according to the endpoint plan.
- Public registration can create a Participant. Organiser registration requires `OrganiserRegistrationCode` in configuration; change the placeholder and never commit a real code to a public repository.
- Cookie authentication is session-like and avoids exposing tokens to JavaScript. In Swagger, call login first; the browser stores the cookie for subsequent requests. Run over HTTPS.

## Database

The default connection string targets SQL Server LocalDB. Update `ConnectionStrings:RaceDay` in `appsettings.json` for your SQL Server instance. From the solution folder run:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project RaceDay.API
dotnet ef database update --project RaceDay.API
```

Review the generated migration against `RaceDay_Part1/docs/RaceDay_Database.sql`; do not apply both schema-creation paths to the same existing database without checking the current schema. EF Core Code First is the application's schema-management path.

## Run

```powershell
dotnet restore
dotnet run --project RaceDay.API
```
Open the HTTPS URL printed by the app followed by `/swagger`.
