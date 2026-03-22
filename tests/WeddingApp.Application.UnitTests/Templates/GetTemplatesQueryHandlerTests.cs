using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Templates.Queries;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Templates;

public class GetTemplatesQueryHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_WithTemplates_ReturnsAllTemplateDtos()
    {
        // Arrange
        using var context = CreateContext();
        context.Templates.Add(new Template("Classic Blue", "<html>blue</html>"));
        context.Templates.Add(new Template("Modern Red", "<html>red</html>"));
        await context.SaveChangesAsync();

        var handler = new GetTemplatesQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetTemplatesQuery(), CancellationToken.None);

        // Assert
        var list = result.ToList();
        Assert.Equal(2, list.Count);
        Assert.Contains(list, t => t.Name == "Classic Blue");
        Assert.Contains(list, t => t.Name == "Modern Red");
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetTemplatesQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetTemplatesQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
