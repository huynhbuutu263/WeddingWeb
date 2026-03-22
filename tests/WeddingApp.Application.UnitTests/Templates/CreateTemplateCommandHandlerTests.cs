using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Templates.Commands;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Templates;

public class CreateTemplateCommandHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_ValidCommand_SavesTemplateAndReturnsId()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new CreateTemplateCommandHandler(context);
        var command = new CreateTemplateCommand("Classic Blue", "<html>classic</html>");

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, id);
        var saved = await context.Templates.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("Classic Blue", saved.Name);
        Assert.Equal("<html>classic</html>", saved.HtmlStructure);
    }

    [Fact]
    public async Task Handle_MultipleCommands_SavesAllTemplates()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new CreateTemplateCommandHandler(context);

        // Act
        await handler.Handle(new CreateTemplateCommand("Template A", "<html>a</html>"), CancellationToken.None);
        await handler.Handle(new CreateTemplateCommand("Template B", "<html>b</html>"), CancellationToken.None);

        // Assert
        var count = await context.Templates.CountAsync();
        Assert.Equal(2, count);
    }
}
