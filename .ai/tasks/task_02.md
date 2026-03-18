# Task Execution Plan: TSK-002

## 1. Goal
The objective is to accurately map all project dependencies to adhere to the Clean Architecture rules (`architecture.skill.md`).
- Api depends on Application and Infrastructure.
- Infrastructure depends on Application.
- Application depends on Domain.
- Test projects reference their respective source projects.

## 2. Required Files
All `.csproj` files and the `WeddingApp.sln`.

## 3. Implementation Plan
Run the .NET CLI commands to embed the `.csproj` files into the solution and link project-to-project references.

**Terminal Commands:**
```bash
# Delete boilerplate C# files from TSK-001 (Best Practice cleanup)
rm src/WeddingApp.Domain/Class1.cs
rm src/WeddingApp.Application/Class1.cs
rm src/WeddingApp.Infrastructure/Class1.cs
rm src/WeddingApp.Api/WeatherForecast.cs
rm src/WeddingApp.Api/Controllers/WeatherForecastController.cs

# Add all projects to the solution:
dotnet sln WeddingApp.sln add src/WeddingApp.Domain/WeddingApp.Domain.csproj
dotnet sln WeddingApp.sln add src/WeddingApp.Application/WeddingApp.Application.csproj
dotnet sln WeddingApp.sln add src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj
dotnet sln WeddingApp.sln add src/WeddingApp.Api/WeddingApp.Api.csproj
dotnet sln WeddingApp.sln add tests/WeddingApp.Domain.UnitTests/WeddingApp.Domain.UnitTests.csproj
dotnet sln WeddingApp.sln add tests/WeddingApp.Application.UnitTests/WeddingApp.Application.UnitTests.csproj
dotnet sln WeddingApp.sln add tests/WeddingApp.Api.IntegrationTests/WeddingApp.Api.IntegrationTests.csproj

# Link Project References (Dependency Inversion Flow)
dotnet add src/WeddingApp.Application/WeddingApp.Application.csproj reference src/WeddingApp.Domain/WeddingApp.Domain.csproj
dotnet add src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj reference src/WeddingApp.Application/WeddingApp.Application.csproj
dotnet add src/WeddingApp.Api/WeddingApp.Api.csproj reference src/WeddingApp.Application/WeddingApp.Application.csproj src/WeddingApp.Infrastructure/WeddingApp.Infrastructure.csproj

# Link Test References
dotnet add tests/WeddingApp.Domain.UnitTests/WeddingApp.Domain.UnitTests.csproj reference src/WeddingApp.Domain/WeddingApp.Domain.csproj
dotnet add tests/WeddingApp.Application.UnitTests/WeddingApp.Application.UnitTests.csproj reference src/WeddingApp.Application/WeddingApp.Application.csproj
dotnet add tests/WeddingApp.Api.IntegrationTests/WeddingApp.Api.IntegrationTests.csproj reference src/WeddingApp.Api/WeddingApp.Api.csproj
```

## 4. Code Structure Specifications
No C# code editing required. Only modifying XML in the `.csproj` files remotely via the CLI.

## 5. Acceptance Criteria
Run `dotnet build`. If it compiles without circular dependency errors, the Clean Architecture hierarchy is intact.
