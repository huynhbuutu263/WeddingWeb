# Platform Architecture Skill

Purpose: Enforce strict Clean Architecture boundaries.
Scope: Project structure and Dependency Injection.

Rules:
- Domain layer: NO external dependencies (No EF Core, no ASP.NET).
- Application layer: Depends ONLY on Domain.
- Infrastructure layer: Depends on Application (implements interfaces).
- API layer: Depends on Application and Infrastructure (for DI registration only).

Workflow:
1. Define entities in Domain.
2. Define interfaces and CQRS inside Application.
3. Implement interfaces in Infrastructure.
4. Expose routes in API.

Examples:
- Good: `public interface IImageStorage` in Application -> Implemented in Infrastructure.
- Bad: Using `Azure.Storage.Blobs` NuGet in Application or Domain.
