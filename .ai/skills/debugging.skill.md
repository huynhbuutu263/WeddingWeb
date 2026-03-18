# Debugging Skill

Purpose: Systematically repair failing code.
Scope: Compiler errors, test failures.

Rules:
- DO NOT guess. Read the specific exception and stack trace.
- Dependency failures -> Check `Program.cs` or `DependencyInjection.cs`.
- Database errors -> Verify EF Core migrations.

Workflow:
1. Identify exact file/line from error log.
2. Identify missing references/logic.
3. Implement exact fix.
4. Run `dotnet build` or `dotnet test`.

Examples:
- Scenario: "Handler not found" -> Fix MediatR `Assembly.GetExecutingAssembly()` registration.
