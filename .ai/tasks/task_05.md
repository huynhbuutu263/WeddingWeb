# Task Execution Plan: TSK-005

## 1. Goal
The objective is to configure Entity Framework Core within the Infrastructure layer, establishing the `AppDbContext` that maps every domain entity to its corresponding database table. Additionally, this task creates the `IAppDbContext` interface in the Application layer so that all higher-level code depends on an abstraction rather than a concrete EF class. A unique index on `WeddingCard.SlugUrl` is applied via Fluent API to guarantee that no two cards share the same public share link.

## 2. Required Files

**Application Layer (Create):**
- `src/WeddingApp.Application/Common/Interfaces/IAppDbContext.cs`

**Infrastructure Layer (Create):**
- `src/WeddingApp.Infrastructure/Data/AppDbContext.cs`

## 3. Implementation Plan

**Step 1:** Install the required NuGet package into the Infrastructure project.
```bash
dotnet add src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
```

**Step 2:** Install `Microsoft.EntityFrameworkCore` into the Application project (required only for the `DbSet<T>` type used inside the interface).
```bash
dotnet add src/WeddingApp.Application/WeddingApp.Application.csproj package Microsoft.EntityFrameworkCore
```

**Step 3:** Create `IAppDbContext.cs` in the Application layer. This interface exposes only the `DbSet<T>` properties and `SaveChangesAsync` — commands and queries in the Application layer depend on this abstraction.

**Step 4:** Create `AppDbContext.cs` in the Infrastructure layer. Inherit from `DbContext` and implement `IAppDbContext`. Override `OnModelCreating` to apply the unique index on `WeddingCard.SlugUrl`.

**Step 5:** Verify compilation succeeds.
```bash
dotnet build
```

## 4. Code Structure Specifications

**`src/WeddingApp.Application/Common/Interfaces/IAppDbContext.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<WeddingCard> WeddingCards { get; }
    DbSet<Template> Templates { get; }
    DbSet<User> Users { get; }
    DbSet<Guest> Guests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**`src/WeddingApp.Infrastructure/Data/AppDbContext.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<WeddingCard> WeddingCards => Set<WeddingCard>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Guest> Guests => Set<Guest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WeddingCard>(entity =>
        {
            entity.HasIndex(e => e.SlugUrl).IsUnique();
        });
    }
}
```

> **Design Notes:**
> - `AppDbContext` uses the C# primary constructor syntax (introduced in .NET 8) to reduce boilerplate.
> - Each `DbSet<T>` property delegates to `Set<T>()` so EF Core manages the set lifetime correctly.
> - The unique index on `WeddingCard.SlugUrl` enforces the database-level constraint that prevents two wedding cards from sharing the same public URL slug.
> - `IAppDbContext` lives in the Application layer so domain commands/queries can reference it without taking a dependency on EF Core concrete types.

## 5. Unit Test Specifications
No unit tests are required for this task. The `AppDbContext` requires a live database connection (or an in-memory provider) to validate its behavior, which is deferred to the Integration Test suite and the database migration step (TSK-006).

## 6. Acceptance Criteria Verification

Run `dotnet build` to confirm:
- ✅ `WeddingApp.Infrastructure.csproj` references `Microsoft.EntityFrameworkCore.SqlServer`.
- ✅ `WeddingApp.Application.csproj` references `Microsoft.EntityFrameworkCore` (for `DbSet<T>` in the interface).
- ✅ `AppDbContext` successfully compiles, inherits `DbContext`, and implements `IAppDbContext`.
- ✅ All four domain entity `DbSet<T>` properties are present: `WeddingCards`, `Templates`, `Users`, `Guests`.
- ✅ `OnModelCreating` configures a unique index on `WeddingCard.SlugUrl`.
- ✅ Zero compilation errors across the entire solution (`dotnet build` exits with code 0).

## 7. Next Task Preview (TSK-006)

With `AppDbContext` in place, the next step is to generate and apply the initial EF Core migration:
```bash
dotnet ef migrations add Initial --project src/WeddingApp.Infrastructure --startup-project src/WeddingApp.Api
dotnet ef database update --project src/WeddingApp.Infrastructure --startup-project src/WeddingApp.Api
```
This will physically create the SQL Server schema from the entity model defined here.
