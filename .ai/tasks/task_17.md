# Task Execution Plan: TSK-017

## 1. Goal
The objective is to allow adding multiple gallery images to a specific `WeddingCard`. To support this, introduce a new Domain entity (`CardImage`) and establish a one-to-many relationship from `WeddingCard` to `CardImage`. Then, implement an Application layer Command (`AddGalleryImageCommand`) that accepts an image URL and a Card ID, saving the relationship to the database.

## 2. Required Files

**Domain Layer:**
- `src/WeddingApp.Domain/Entities/CardImage.cs` (New)
- `src/WeddingApp.Domain/Entities/WeddingCard.cs` (Modify)

**Application Layer:**
- `src/WeddingApp.Application/Cards/Commands/AddGalleryImageCommand.cs` (New)
- `src/WeddingApp.Application/Cards/Commands/AddGalleryImageCommandHandler.cs` (New)

**Test Layer:**
- `tests/WeddingApp.Domain.UnitTests/Entities/WeddingCardTests.cs` (Modify)
- `tests/WeddingApp.Application.UnitTests/Cards/AddGalleryImageCommandHandlerTests.cs` (New)

## 3. Implementation Plan

1. **Domain Update:** Create the `CardImage` entity inheriting from `BaseEntity`. Add a `ImageUrl` property and a `CardId` foreign key property.
2. **Domain Update:** Add a private list `_images` to `WeddingCard` and a method `AddImage(string url)` to encapsulate the logic.
3. **Application Command:** Create `AddGalleryImageCommand` carrying `Guid CardId` and `string ImageUrl`.
4. **Application Handler:** Implement `IRequestHandler`. The handler fetches the `WeddingCard` from the DbContext, calls `.AddImage(url)`, and calls `SaveChangesAsync()`.
5. **Unit Tests:** Write AAA pattern tests mocking the infrastructure layer.

## 4. Code Structure Specifications

**`src/WeddingApp.Domain/Entities/CardImage.cs`**
```csharp
namespace WeddingApp.Domain.Entities;

public class CardImage : BaseEntity
{
    public Guid WeddingCardId { get; private set; }
    public string ImageUrl { get; private set; }

    // Required by EF Core
    private CardImage() { ImageUrl = string.Empty; }

    internal CardImage(Guid weddingCardId, string imageUrl)
    {
        WeddingCardId = weddingCardId;
        ImageUrl = imageUrl;
    }
}
```

**`src/WeddingApp.Domain/Entities/WeddingCard.cs` (Additions)**
```csharp
namespace WeddingApp.Domain.Entities;

public class WeddingCard : BaseEntity
{
    // ... Existing properties

    private readonly List<CardImage> _images = new();
    public IReadOnlyCollection<CardImage> Images => _images.AsReadOnly();

    public void AddImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty.", nameof(imageUrl));
            
        _images.Add(new CardImage(Id, imageUrl));
    }
}
```

**`src/WeddingApp.Application/Cards/Commands/AddGalleryImageCommand.cs`**
```csharp
using MediatR;

namespace WeddingApp.Application.Cards.Commands;

public record AddGalleryImageCommand(Guid CardId, string ImageUrl) : IRequest<Guid>;
```

**`src/WeddingApp.Application/Cards/Commands/AddGalleryImageCommandHandler.cs`**
```csharp
using MediatR;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Application.Cards.Commands;

public class AddGalleryImageCommandHandler(IAppDbContext context) 
    : IRequestHandler<AddGalleryImageCommand, Guid>
{
    public async Task<Guid> Handle(AddGalleryImageCommand request, CancellationToken cancellationToken)
    {
        var card = await context.WeddingCards.FindAsync([request.CardId], cancellationToken);
        
        if (card is null)
        {
            throw new Exception($"Card with ID {request.CardId} not found."); 
        }

        card.AddImage(request.ImageUrl);
        await context.SaveChangesAsync(cancellationToken);

        return card.Id;
    }
}
```

## 5. Unit Test Specifications

**`tests/WeddingApp.Domain.UnitTests/Entities/WeddingCardTests.cs`**
```csharp
[Fact]
public void AddImage_ValidUrl_AddsImageToCollection()
{
    // Arrange
    var card = new WeddingCard("My Wedding", "my-slug", DateTime.UtcNow, Guid.NewGuid());
    var imageUrl = "https://cdn.example.com/image1.png";

    // Act
    card.AddImage(imageUrl);

    // Assert
    Assert.Single(card.Images);
    Assert.Equal(imageUrl, card.Images.First().ImageUrl);
}
```

**`tests/WeddingApp.Application.UnitTests/Cards/AddGalleryImageCommandHandlerTests.cs`**
```csharp
[Fact]
public async Task Handle_ValidCommand_SavesImageToDatabase()
{
    // Arrange
    var mockContext = Substitute.For<IAppDbContext>();
    var cardId = Guid.NewGuid();
    var card = new WeddingCard("Test", "slug", DateTime.UtcNow, Guid.NewGuid());
    
    mockContext.WeddingCards.FindAsync([cardId], Arg.Any<CancellationToken>())
        .Returns(card);

    var handler = new AddGalleryImageCommandHandler(mockContext);
    var command = new AddGalleryImageCommand(cardId, "url");

    // Act
    await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.Single(card.Images);
    await mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
}
```
