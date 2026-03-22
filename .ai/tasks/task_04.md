# Task Execution Plan: TSK-004

## 1. Goal
The objective is to define the two primary aggregate entities that drive the core user-facing feature of the application: `WeddingCard` (the shareable invitation page) and `Template` (the reusable visual layout). `WeddingCard` holds a foreign-key reference to `Template`, establishing a many-to-one relationship at the domain level. Both entities enforce invariants through constructor validation.

## 2. Required Files

**Domain Layer (Create):**
- `src/WeddingApp.Domain/Entities/WeddingCard.cs`
- `src/WeddingApp.Domain/Entities/Template.cs`

**Test Layer (Create):**
- `tests/WeddingApp.Domain.UnitTests/Entities/WeddingCardTests.cs`

## 3. Implementation Plan

No terminal scaffolding commands are required. All work is pure C# authoring inside the Domain project (which was created in TSK-001).

**Step 1:** Create `Template.cs` — a simple entity with `Name` and `HtmlStructure` properties. Include a private ORM constructor to satisfy Entity Framework Core.

**Step 2:** Create `WeddingCard.cs` — the central aggregate. It must store `Title`, `SlugUrl` (the unique human-readable identifier used in shared links), `EventDate`, and a `TemplateId` foreign key. Validate that `Title` and `SlugUrl` are never empty or whitespace.

**Step 3:** Write unit tests in `WeddingCardTests.cs` covering:
- Valid construction (all properties correctly assigned).
- Invalid construction (empty or null `Title` / `SlugUrl` both throw `ArgumentException`).

## 4. Code Structure Specifications

**`src/WeddingApp.Domain/Entities/Template.cs`**
```csharp
using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class Template : BaseEntity
{
    public string Name { get; private set; }
    public string HtmlStructure { get; private set; }

    // Required by ORM
    private Template() { Name = string.Empty; HtmlStructure = string.Empty; }

    public Template(string name, string htmlStructure)
    {
        Name = name;
        HtmlStructure = htmlStructure;
    }
}
```

**`src/WeddingApp.Domain/Entities/WeddingCard.cs`**
```csharp
using WeddingApp.Domain.Common;

namespace WeddingApp.Domain.Entities;

public class WeddingCard : BaseEntity
{
    public string Title { get; private set; }
    public string SlugUrl { get; private set; }
    public DateTime EventDate { get; private set; }
    public Guid TemplateId { get; private set; }
    public Template Template { get; private set; }

    // Required by ORM
    private WeddingCard() { Title = string.Empty; SlugUrl = string.Empty; Template = null!; }

    public WeddingCard(string title, string slugUrl, DateTime eventDate, Guid templateId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(slugUrl))
            throw new ArgumentException("SlugUrl cannot be empty.", nameof(slugUrl));

        Title = title;
        SlugUrl = slugUrl;
        EventDate = eventDate;
        TemplateId = templateId;
        Template = null!; // Populated by EF Core navigation loading
    }
}
```

> **Design Note:** `SlugUrl` is intended to be configured as a **unique index** in the database (TSK-005). It forms the basis of the public share link (e.g., `/cards/john-jane-wedding-2026`).

## 5. Unit Test Specifications

**`tests/WeddingApp.Domain.UnitTests/Entities/WeddingCardTests.cs`**
```csharp
using System;
using Xunit;
using WeddingApp.Domain.Entities;

namespace WeddingApp.Domain.UnitTests.Entities;

public class WeddingCardTests
{
    [Fact]
    public void Constructor_WithEmptyStrings_ShouldThrowArgumentException()
    {
        // Arrange
        var eventDate = DateTime.Now.AddMonths(6);
        var templateId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeddingCard("", "valid-slug", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard("Valid Title", "", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard(null, "valid-slug", eventDate, templateId));
        Assert.Throws<ArgumentException>(() => new WeddingCard("Valid Title", null, eventDate, templateId));
    }

    [Fact]
    public void Constructor_ValidInputs_ShouldInitializeProperly()
    {
        // Arrange
        var title = "John & Jane's Wedding";
        var slugUrl = "john-jane-wedding-2026";
        var eventDate = new DateTime(2026, 8, 15);
        var templateId = Guid.NewGuid();

        // Act
        var weddingCard = new WeddingCard(title, slugUrl, eventDate, templateId);

        // Assert
        Assert.Equal(title, weddingCard.Title);
        Assert.Equal(slugUrl, weddingCard.SlugUrl);
        Assert.Equal(eventDate, weddingCard.EventDate);
        Assert.Equal(templateId, weddingCard.TemplateId);
        Assert.NotEqual(Guid.Empty, weddingCard.Id);
    }
}
```

**Test count target:** 2 `[Fact]` tests (4 internal assertions in the first test, 5 in the second).

## 6. Acceptance Criteria Verification

Run `dotnet test tests/WeddingApp.Domain.UnitTests` to confirm:
- ✅ All 2 new tests pass (green), alongside the 4 tests from TSK-003.
- ✅ `WeddingCard` correctly stores `Title`, `SlugUrl`, `EventDate`, and `TemplateId`.
- ✅ `Template` navigation property exists on `WeddingCard` (ready for EF Core eager loading in TSK-005).
- ✅ Both entities enforce non-empty invariants via `ArgumentException`.
- ✅ `dotnet build` produces zero compilation errors in the Domain project.

## 7. Next Task Preview (TSK-005)

With the full domain model in place, the next step is to configure Entity Framework Core:
- Create `AppDbContext` in the Infrastructure layer.
- Register `DbSet<WeddingCard>`, `DbSet<Template>`, `DbSet<User>`, `DbSet<Guest>`.
- Apply a **unique index** on `WeddingCard.SlugUrl` via Fluent API to guarantee globally unique share links.
