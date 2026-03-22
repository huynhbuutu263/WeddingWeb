# Task Execution Plan: TSK-006

## 1. Goal
Initialize the database schema by switching from SQL Server to MySQL (MySQL Workbench 8.0) and generating the initial EF Core migration. This migration creates all physical tables from the domain entity model defined in TSK-003, TSK-004, and TSK-005.

## 2. Required Files

**Infrastructure Layer (Added):**
- `src/WeddingApp.Infrastructure/Data/Migrations/20260322104225_Initial.cs` — Up/Down migration logic
- `src/WeddingApp.Infrastructure/Data/Migrations/20260322104225_Initial.Designer.cs` — EF snapshot metadata
- `src/WeddingApp.Infrastructure/Data/Migrations/AppDbContextModelSnapshot.cs` — Model snapshot for future migrations

**Modified:**
- `src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj` — Switched `Microsoft.EntityFrameworkCore.SqlServer` → `Pomelo.EntityFrameworkCore.MySql 9.0.0`; added `Microsoft.EntityFrameworkCore.Design`
- `src/WeddingApp.Api/WeddingApp.Api.csproj` — Added `Microsoft.EntityFrameworkCore.Design` (required by the `dotnet ef` tooling as startup project)
- `src/WeddingApp.Infrastructure/DependencyInjection.cs` — Updated `UseSqlServer` → `UseMySql` with `MySqlServerVersion(8, 0, 0)`
- `src/WeddingApp.Api/appsettings.json` — Updated connection string to MySQL format

## 3. Implementation Steps

**Step 1:** Replace the SQL Server EF Core provider with Pomelo MySQL.
```bash
dotnet remove src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design --version 9.0.3
dotnet add src/WeddingApp.Api/WeddingApp.Api.csproj package Microsoft.EntityFrameworkCore.Design --version 9.0.3
```

**Step 2:** Update `DependencyInjection.cs` to use MySQL.
```csharp
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))));
```
> **Note:** Use `new MySqlServerVersion(...)` rather than `ServerVersion.AutoDetect()` so that `dotnet ef migrations` works at design-time without requiring a live database connection.

**Step 3:** Update `appsettings.json` with the MySQL connection string.
```json
"DefaultConnection": "Server=localhost;Port=3306;Database=WeddingApp;User=root;Password=yourpassword;"
```

**Step 4:** Generate the initial EF Core migration.
```bash
dotnet ef migrations add Initial \
  --project src/WeddingApp.Infrastructure \
  --startup-project src/WeddingApp.Api \
  --output-dir Data/Migrations
```

**Step 5 (on local MySQL 8.0 instance):** Apply the migration to create the physical schema.
```bash
dotnet ef database update \
  --project src/WeddingApp.Infrastructure \
  --startup-project src/WeddingApp.Api
```

## 4. Database Schema Created

The `Initial` migration creates the following tables:

| Table | Primary Key | Notes |
|-------|------------|-------|
| `Templates` | `Id` (GUID) | Stores card HTML templates |
| `Users` | `Id` (GUID) | Admin accounts with hashed passwords |
| `Guests` | `Id` (GUID) | Wedding card guest list entries |
| `WeddingCards` | `Id` (GUID) | FK → `Templates.Id`; unique index on `SlugUrl` |
| `CardImages` | `Id` (GUID) | FK → `WeddingCards.Id` with CASCADE delete |

**Key constraints:**
- `WeddingCards.SlugUrl` has a **unique index** (`IX_WeddingCards_SlugUrl`) to prevent duplicate share links.
- `CardImages` has a **cascade delete** back to `WeddingCards`.
- All tables use `utf8mb4` character set for full Unicode support.
- All primary keys use `char(36)` (GUID) format.

## 5. Unit Test Specifications
No unit tests are required for this task. Migration correctness is validated by a successful `dotnet ef database update` execution against the live MySQL 8.0 instance.

## 6. Acceptance Criteria Verification

- ✅ `Pomelo.EntityFrameworkCore.MySql 9.0.0` is referenced in `WeddingApp.Infrastructure.csproj`
- ✅ `DependencyInjection.cs` uses `UseMySql` with `MySqlServerVersion(8, 0, 0)`
- ✅ `appsettings.json` contains a valid MySQL connection string
- ✅ Migration files generated in `src/WeddingApp.Infrastructure/Data/Migrations/`
- ✅ `dotnet build` succeeds with 0 errors
- ✅ All 5 tables are defined in the migration `Up()` method
- ✅ Unique index on `WeddingCards.SlugUrl` is present in the migration

## 7. Next Task Preview (TSK-007)

With the database schema established, TSK-007 sets up the MediatR + FluentValidation pipeline behavior so commands are automatically validated before reaching their handlers.
