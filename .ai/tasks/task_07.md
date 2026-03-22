# Task Execution Plan: TSK-007

## 1. Goal
The objective is to establish a fully automated validation pipeline in the Application layer using MediatR's `IPipelineBehavior` and FluentValidation. A generic `ValidationBehavior<TRequest, TResponse>` intercepts every MediatR request before the handler executes, runs all registered validators for that request type, and throws a custom `ValidationException` if any rules fail. This ensures that no invalid command or query ever reaches a handler, centralising validation logic away from controllers.

## 2. Required Files

**Application Layer (Create):**
- `src/WeddingApp.Application/Common/Exceptions/ValidationException.cs`
- `src/WeddingApp.Application/Common/Exceptions/NotFoundException.cs`
- `src/WeddingApp.Application/Common/Behaviors/ValidationBehavior.cs`
- `src/WeddingApp.Application/DependencyInjection.cs`

**Test Layer (Create):**
- `tests/WeddingApp.Application.UnitTests/Behaviors/ValidationBehaviorTests.cs`

## 3. Implementation Plan

**Step 1:** Install required NuGet packages into the Application project.
```bash
dotnet add src/WeddingApp.Application/WeddingApp.Application.csproj package MediatR
dotnet add src/WeddingApp.Application/WeddingApp.Application.csproj package FluentValidation.DependencyInjectionExtensions
```

**Step 2:** Install MediatR and FluentValidation into the Application unit test project.
```bash
dotnet add tests/WeddingApp.Application.UnitTests/WeddingApp.Application.UnitTests.csproj package MediatR
dotnet add tests/WeddingApp.Application.UnitTests/WeddingApp.Application.UnitTests.csproj package FluentValidation
```

**Step 3:** Create `ValidationException.cs` — wraps a list of `ValidationFailure` objects into a structured dictionary of field → error messages.

**Step 4:** Create `NotFoundException.cs` — a typed exception for use when an entity cannot be found by its key (used in query handlers).

**Step 5:** Create `ValidationBehavior.cs` — the MediatR pipeline behaviour. If no validators are registered for a request type, it skips validation and calls `next` directly. Otherwise it runs all validators in parallel, collects failures, and throws `ValidationException` if any exist.

**Step 6:** Create `DependencyInjection.cs` — an extension method that registers MediatR, all FluentValidation validators, and the `ValidationBehavior` pipeline with the ASP.NET Core DI container.

**Step 7:** Register the Application services in `src/WeddingApp.Api/Program.cs`.
```csharp
builder.Services.AddApplication();
```

**Step 8:** Write unit tests for `ValidationBehavior`.

## 4. Code Structure Specifications

**`src/WeddingApp.Application/Common/Exceptions/ValidationException.cs`**
```csharp
namespace WeddingApp.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
```

**`src/WeddingApp.Application/Common/Exceptions/NotFoundException.cs`**
```csharp
namespace WeddingApp.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}
```

**`src/WeddingApp.Application/Common/Behaviors/ValidationBehavior.cs`**
```csharp
using FluentValidation;
using MediatR;

namespace WeddingApp.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
            throw new Exceptions.ValidationException(failures);

        return await next(cancellationToken);
    }
}
```

**`src/WeddingApp.Application/DependencyInjection.cs`**
```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WeddingApp.Application.Common.Behaviors;

namespace WeddingApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
```

> **Design Notes:**
> - `ValidationBehavior` uses the C# primary constructor syntax to inject `IEnumerable<IValidator<TRequest>>`. When no validators are registered for a given request type, ASP.NET Core's built-in DI container returns an empty enumerable, so the behavior short-circuits cleanly.
> - All validators in the Application assembly are scanned and registered automatically by `AddValidatorsFromAssembly`, so adding a new `AbstractValidator<TCommand>` requires no manual DI wiring.
> - `ValidationException.Errors` is keyed by property name, enabling API consumers to receive field-level error messages (e.g., `{ "Title": ["Title is required."] }`).
> - `NotFoundException` is provided here so the same exception type can be used across all future query handlers without importing Infrastructure.

## 5. Unit Test Specifications

**`tests/WeddingApp.Application.UnitTests/Behaviors/ValidationBehaviorTests.cs`**
```csharp
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WeddingApp.Application.Common.Behaviors;

namespace WeddingApp.Application.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required.");
        }
    }

    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");
        var nextCalled = false;

        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("ok"); }

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest("valid");
        var nextCalled = false;

        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("ok"); }

        // Act
        var result = await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>> { new TestValidator() };
        var behavior = new ValidationBehavior<TestRequest, string>(validators);
        var request = new TestRequest(string.Empty);

        Task<string> Next(CancellationToken ct) => Task.FromResult("should not reach");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<WeddingApp.Application.Common.Exceptions.ValidationException>(
            () => behavior.Handle(request, Next, CancellationToken.None));

        Assert.True(exception.Errors.ContainsKey("Value"));
    }
}
```

**Test count target:** 3 `[Fact]` tests covering: no validators (short-circuit), valid request (passes through), invalid request (throws `ValidationException` with correct error key).

## 6. Acceptance Criteria Verification

Run `dotnet test tests/WeddingApp.Application.UnitTests` to confirm:
- ✅ All 3 new `ValidationBehaviorTests` pass (green).
- ✅ `Handle_NoValidators_CallsNext` — next delegate is called and result is returned.
- ✅ `Handle_ValidRequest_CallsNext` — valid requests are not blocked by validators.
- ✅ `Handle_InvalidRequest_ThrowsValidationException` — `ValidationException.Errors` contains the failing field key (`"Value"`).
- ✅ `DependencyInjection.AddApplication()` compiles and wires MediatR + FluentValidation + `ValidationBehavior`.
- ✅ `dotnet build` produces zero compilation errors across the solution.

## 7. Next Task Preview (TSK-008)

With the Application validation pipeline complete, the next step is to surface these exceptions as structured HTTP responses. TSK-008 creates `ExceptionMiddleware` in the API layer that:
- Catches `ValidationException` → returns `400 Bad Request` with the field-level error dictionary.
- Catches `NotFoundException` → returns `404 Not Found` with the entity detail message.
- Catches all other exceptions → returns `500 Internal Server Error` without leaking stack traces.
