# TaskTrack Backend

ASP.NET Core Web API backend for PRN232 Assignment 1. The solution uses three projects:

- `TaskTrack.API`: controllers, Swagger, CORS, dependency injection, configuration.
- `TaskTrack.Service`: DTOs, validation, business rules.
- `TaskTrack.Repo`: EF Core database-first style models, DbContext, repositories.

## Prerequisites

- .NET 8 SDK
- PostgreSQL
- `dotnet-ef` if you need to scaffold again

## Database

The source schema is `../TaskManagementDB_Postgres.sql`. The implemented entities match the provided tables: `Department`, `Project`, `Task`, `Tag`, and `TaskTag`.

Expected database-first scaffold command:

```bash
dotnet ef dbcontext scaffold "<connection-string>" Npgsql.EntityFrameworkCore.PostgreSQL -o Models
```

The checked-in code does not contain real database passwords. Update local configuration before running.

## Configuration

Local development can use `TaskTrack.API/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManagementDB;Username=postgres;Password=CHANGE_ME"
}
```

Production supports environment variables:

- `DATABASE_URL`: Render PostgreSQL URL.
- `FRONTEND_URL`: deployed Vercel frontend URL.
- `ASPNETCORE_ENVIRONMENT`: use `Production` on Render.

Swagger is enabled through `Swagger:Enabled`.

## Run

```bash
dotnet restore
dotnet build
dotnet run --project TaskTrack.API
```

## API Endpoints

- `GET/POST/PUT/DELETE /api/departments`
- `GET /api/departments/{id}`
- `GET /api/departments/search?name=`
- `GET/POST/PUT/DELETE /api/projects`
- `GET /api/projects/{id}`
- `GET /api/projects/department/{departmentId}`
- `GET /api/projects/search?name=&status=&departmentId=`
- `GET/POST/PUT/DELETE /api/tasks`
- `GET /api/tasks/{id}`
- `GET /api/tasks/project/{projectId}`
- `GET /api/tasks/search?title=&status=&priority=&projectId=&tagId=`
- `GET/POST/PUT/DELETE /api/tags`

## Render Deployment

1. Create a PostgreSQL database on Render.
2. Run `TaskManagementDB_Postgres.sql` against that database.
3. Create a Render Web Service for `QE190088_SE19B_Ass1_BE`.
4. Set build command: `dotnet publish TaskTrack.API/TaskTrack.API.csproj -c Release -o out`.
5. Set start command: `dotnet out/TaskTrack.API.dll`.
6. Set `DATABASE_URL`, `FRONTEND_URL`, and `ASPNETCORE_ENVIRONMENT=Production`.

## Naming Note

The repository still uses `QE190088_SE19B` placeholders. Replace those folder/solution names with your actual student ID and class code before submission if required.

