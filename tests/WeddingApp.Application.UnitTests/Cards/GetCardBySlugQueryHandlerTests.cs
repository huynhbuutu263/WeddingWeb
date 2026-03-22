using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Cards.Queries;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Cards;

public class GetCardBySlugQueryHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ExistingSlug_ReturnsCardDto()
    {
        // Arrange
        using var context = CreateContext();
        var template = new Template("Classic", "<html></html>");
        context.Templates.Add(template);
        var card = new WeddingCard("John & Jane", "john-jane", DateTime.UtcNow.AddMonths(3), template.Id);
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new GetCardBySlugQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetCardBySlugQuery("john-jane"), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John & Jane", result.Title);
        Assert.Equal("john-jane", result.SlugUrl);
        Assert.Equal("Classic", result.Template.Name);
        Assert.Empty(result.ImageUrls);
    }

    [Fact]
    public async Task Handle_NonExistentSlug_ThrowsNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetCardBySlugQueryHandler(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetCardBySlugQuery("non-existent"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CardWithImages_ReturnsImageUrls()
    {
        // Arrange
        using var context = CreateContext();
        var template = new Template("Classic", "<html></html>");
        context.Templates.Add(template);
        var card = new WeddingCard("Wedding", "wedding-slug", DateTime.UtcNow.AddMonths(3), template.Id);
        card.AddImage("/uploads/photo1.jpg");
        card.AddImage("/uploads/photo2.jpg");
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new GetCardBySlugQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetCardBySlugQuery("wedding-slug"), CancellationToken.None);

        // Assert
        var urls = result.ImageUrls.ToList();
        Assert.Equal(2, urls.Count);
        Assert.Contains("/uploads/photo1.jpg", urls);
        Assert.Contains("/uploads/photo2.jpg", urls);
    }
}
