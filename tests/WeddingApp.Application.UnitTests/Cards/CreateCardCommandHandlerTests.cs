using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Cards.Commands;
using WeddingApp.Application.Common.Exceptions;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Cards;

public class CreateCardCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ValidCommand_SavesCardAndReturnsId()
    {
        // Arrange
        using var context = CreateContext();
        var template = new Template("Classic", "<html></html>");
        context.Templates.Add(template);
        await context.SaveChangesAsync();

        var handler = new CreateCardCommandHandler(context);
        var command = new CreateCardCommand(
            "John & Jane Wedding",
            "john-jane-wedding",
            DateTime.UtcNow.AddMonths(6),
            template.Id);

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, id);
        var saved = await context.WeddingCards.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("john-jane-wedding", saved.SlugUrl);
    }

    [Fact]
    public async Task Handle_TemplateNotFound_ThrowsNotFoundException()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new CreateCardCommandHandler(context);
        var command = new CreateCardCommand(
            "Wedding Title",
            "wedding-slug",
            DateTime.UtcNow.AddMonths(1),
            Guid.NewGuid()); // non-existent template

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
