using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Cards.Commands;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Cards;

public class AddGalleryImageCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ValidCardAndUrl_AddsImageToCard()
    {
        // Arrange
        using var context = CreateContext();
        var template = new Template("Classic", "<html></html>");
        context.Templates.Add(template);
        var card = new WeddingCard("John & Jane", "john-jane", DateTime.UtcNow.AddMonths(3), template.Id);
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new AddGalleryImageCommandHandler(context);
        var command = new AddGalleryImageCommand(card.Id, "/uploads/photo1.jpg");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var images = await context.CardImages.Where(i => i.WeddingCardId == card.Id).ToListAsync();
        Assert.Single(images);
        Assert.Equal("/uploads/photo1.jpg", images[0].Url);
    }

    [Fact]
    public async Task Handle_MultipleImages_AddsAllImagesToCard()
    {
        // Arrange
        using var context = CreateContext();
        var template = new Template("Modern", "<html></html>");
        context.Templates.Add(template);
        var card = new WeddingCard("Wedding", "wedding-slug", DateTime.UtcNow.AddMonths(6), template.Id);
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new AddGalleryImageCommandHandler(context);

        // Act
        await handler.Handle(new AddGalleryImageCommand(card.Id, "/uploads/photo1.jpg"), CancellationToken.None);
        await handler.Handle(new AddGalleryImageCommand(card.Id, "/uploads/photo2.jpg"), CancellationToken.None);

        // Assert
        var images = await context.CardImages.Where(i => i.WeddingCardId == card.Id).ToListAsync();
        Assert.Equal(2, images.Count);
        Assert.Contains(images, i => i.Url == "/uploads/photo1.jpg");
        Assert.Contains(images, i => i.Url == "/uploads/photo2.jpg");
    }

    [Fact]
    public async Task Handle_CardNotFound_ThrowsNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new AddGalleryImageCommandHandler(context);
        var command = new AddGalleryImageCommand(Guid.NewGuid(), "/uploads/photo.jpg");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
