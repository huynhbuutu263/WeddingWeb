# Coding Skill

Purpose: Write fast, idiomatic C# 12/.NET 8 code.
Scope: Syntax, async, concurrency, and nullability.

Rules:
- Use `file-scoped` namespaces.
- Enable `<Nullable>enable</Nullable>`.
- Use `async/await` and pass `CancellationToken` for all I/O.
- Use primary constructors for dependency injection.
- Return standard HTTP status codes in controllers.

Workflow:
1. Write interface.
2. Implement class using primary constructors.
3. Wire up async execution paths.

Examples:
- Good: `public class MyHandler(IAppDbContext context) : IRequestHandler<Command>`
- Bad: Generating repetitive constructor boilerplate.
