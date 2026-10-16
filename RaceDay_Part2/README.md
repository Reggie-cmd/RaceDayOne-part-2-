# RaceDay — Part 2: RESTful API Development

**Technology:** C#, ASP.NET Core Web API (.NET 8), Entity Framework Core, SQL Server, Swagger/OpenAPI, xUnit, GitHub Actions.

## Overview
RaceDay supports running, walking and cycling events. Participants can view events, maintain their profile, enrol in categories and view their own results. Organisers can manage their own events/categories, view enrolments for their events and record results.

## Project structure
- `RaceDay.API/` — Web API, controllers, DTOs, EF Core entities and services.
- `RaceDay.Tests/` — unit tests for password handling, registration rules and enrolment validation.
- `RaceDay_Part1/docs/` — Part 1 ERD, API endpoint plan and SQL reference script.
- `.github/workflows/dotnet.yml` — restore, build and test on pushes and pull requests.

## Setup (Visual Studio)
1. Install Visual Studio 2022 with the **ASP.NET and web development** workload, .NET 8 SDK, and SQL Server/LocalDB.
2. Open `RaceDay.sln`.
3. In `RaceDay.API/appsettings.json`, update the `RaceDay` connection string for your SQL Server instance.
4. Change `OrganiserRegistrationCode` from its placeholder for local testing. Never publish a real secret in a public repository; use GitHub Secrets/environment configuration for deployment.
5. Open **Tools → NuGet Package Manager → Package Manager Console**, set Default project to `RaceDay.API`, then run `Add-Migration InitialCreate` and `Update-Database`. Alternatively, use `dotnet ef migrations add InitialCreate --project RaceDay.API` and `dotnet ef database update --project RaceDay.API` after installing `dotnet-ef`.
6. Run the `RaceDay.API` project. Browse to `/swagger` on the HTTPS address.
7. Run tests from Test Explorer or `dotnet test RaceDay.sln`.

## Database consistency
The EF Core model preserves the six Part 1 entities and their relationships: `Users`, `UserProfiles`, `Events`, `Categories`, `Enrolments`, and `Results`. The migration generated from the model is the Code First schema. Compare it with `RaceDay_Part1/docs/RaceDay_Database.sql` before using an existing Part 1 database, especially if that database already contains tables.

## Authentication and permissions
- Login creates an HTTP-only, secure authentication cookie.
- Public event and category reads are available without login.
- Participant endpoints are restricted to the signed-in participant and return only their enrolments/results.
- Organiser endpoints require the Organiser role and verify ownership of the target event.
- Public registration accepts `Participant` or `Organiser`, but organiser registration requires the configured organiser code.
- Do not store plain-text passwords. Passwords are hashed with ASP.NET Core's `PasswordHasher<User>`.

## Endpoint coverage
| Area | Endpoints |
|---|---|
| Authentication | `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout` |
| Profile | `GET /api/users/me`, `PUT /api/users/me` |
| Events | `GET /api/events`, `GET /api/events/{id}`, `POST /api/events`, `PUT /api/events/{id}`, `DELETE /api/events/{id}` |
| Categories | `GET /api/events/{eventId}/categories`, `POST /api/events/{eventId}/categories`, `PUT /api/categories/{id}`, `DELETE /api/categories/{id}` |
| Enrolments | `POST /api/enrolments`, `GET /api/enrolments/me`, `GET /api/enrolments/events/{eventId}`, `DELETE /api/enrolments/{id}` |
| Results | `POST /api/results`, `GET /api/results/me`, `GET /api/results/events/{eventId}`, `GET /api/results/{id}` |
| Health | `GET /health` |

## Test and CI
GitHub Actions restores dependencies, builds the solution and runs xUnit tests. After pushing, open the repository's **Actions** tab and capture a screenshot of a successful run for submission. A green run has not yet been verified in this environment because the .NET SDK is not installed here.

## Changelog
- Part 2 initial implementation: added ASP.NET Core Web API project, EF Core model matching the six Part 1 entities, cookie authentication, role/ownership checks, event/category/enrolment/result endpoints, Swagger, unit tests and GitHub Actions workflow.
- Before submission: add dated entries for your own fixes and testing, and include the actual repository URL, unlisted presentation link (if required), and screenshots.

## Academic integrity and evidence
Review and understand the code, run it locally, fix any errors, and follow your lecturer's rules for acknowledging AI assistance. Commit your own meaningful progress with accurate messages; do not fabricate commits, test results, screenshots, or a green CI badge.
