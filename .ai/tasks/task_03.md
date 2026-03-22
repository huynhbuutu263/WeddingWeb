# Task Execution Plan: TSK-003

## 1. Goal
The objective is to define the core domain entities that serve as the foundation for the entire system. This task establishes the shared `BaseEntity` base class and creates two first-class domain entities: `User` (for admin/owner management) and `Guest` (for RSVP tracking per wedding card). All entities must follow strict Clean Architecture rules — zero external framework dependencies inside the Domain layer.

## 2. Required Files

**Domain Layer (Create):**
- `src/WeddingApp.Domain/Common/BaseEntity.cs`
- `src/WeddingApp.Domain/Entities/User.cs`
- `src/WeddingApp.Domain/Entities/Guest.cs`

**Test Layer (Create):**
- `tests/WeddingApp.Domain.UnitTests/Entities/UserTests.cs`

## 3. Implementation Plan

No terminal scaffolding commands are required. All work is pure C# authoring inside the Domain project.

**Step 1:** Create the `Common/` subdirectory inside `WeddingApp.Domain` and add `BaseEntity.cs`.

**Step 2:** Create the `Entities/` subdirectory and add `User.cs` with email validation and a `UpdatePassword` helper method.

**Step 3:** Add `Guest.cs` with a mandatory `Name` guard and an optional `Email` field (for RSVP communication).

**Step 4:** Write unit tests in `UserTests.cs` covering the happy path and all invalid constructor inputs using the AAA pattern.

## 4. Code Structure Specifications

**`src/WeddingApp.Domain/Common/BaseEntity.cs`**
```csharp
namespace WeddingApp.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
}
```

**`src/WeddingApp.Domain/Entities/User.cs`**
```csharp
using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    // Required by ORM
    private User() { Email = string.Empty; PasswordHash = string.Empty; FirstName = string.Empty; LastName = string.Empty; }

    public User(string email, string passwordHash, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

**`src/WeddingApp.Domain/Entities/Guest.cs`**
```csharp
using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class Guest : BaseEntity
{
    public Guid WeddingCardId { get; private set; }
    public string Name { get; private set; }
    public string? Email { get; private set; }
    public bool IsAttending { get; private set; }

    // Required by ORM
    private Guest() { Name = string.Empty; }

    public Guest(Guid weddingCardId, string name, string? email, bool isAttending)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        WeddingCardId = weddingCardId;
        Name = name;
        Email = email;
        IsAttending = isAttending;
    }
}
```

## 5. Unit Test Specifications

**`tests/WeddingApp.Domain.UnitTests/Entities/UserTests.cs`**
```csharp
using WeddingApp.Domain.Entities;

namespace WeddingApp.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesUserAndGeneratesId()
    {
        // Arrange
        var email = "admin@example.com";
        var hash = "hashedpass";

        // Act
        var user = new User(email, hash, "John", "Doe");

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_EmptyEmail_ThrowsArgumentException(string? invalidEmail)
    {
        // Arrange
        var hash = "hashedpass";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new User(invalidEmail, hash, "John", "Doe"));
    }
}
```

**Test count target:** 4 tests (1 `[Fact]` + 1 `[Theory]` with 3 inline data cases).

## 6. Acceptance Criteria Verification

Run `dotnet test tests/WeddingApp.Domain.UnitTests` to confirm:
- ✅ All 4 tests pass (green).
- ✅ `WeddingApp.Domain.csproj` has **no** `<PackageReference>` entries (pure C#, no framework coupling).
- ✅ `BaseEntity`, `User`, and `Guest` each use `private set` or `protected set` — no public mutators.
- ✅ `dotnet build` produces zero compilation errors in the Domain project.
