using Microsoft.EntityFrameworkCore;
using WeddingApp.Application.Admin.Queries;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Application.UnitTests.Admin;

public class GetDashboardStatsQueryHandlerTests
{
    private static AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_WithData_ReturnsCorrectCounts()
    {
        // Arrange
        using var context = CreateContext();
        var template1 = new Template("T1", "<html>1</html>");
        var template2 = new Template("T2", "<html>2</html>");
        context.Templates.AddRange(template1, template2);

        var card = new WeddingCard("Wedding", "wedding-slug", DateTime.UtcNow.AddMonths(1), template1.Id);
        context.WeddingCards.Add(card);
        await context.SaveChangesAsync();

        var handler = new GetDashboardStatsQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalTemplates);
        Assert.Equal(1, result.TotalCards);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsZeroCounts()
    {
        // Arrange
        using var context = CreateContext();
        var handler = new GetDashboardStatsQueryHandler(context);

        // Act
        var result = await handler.Handle(new GetDashboardStatsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(0, result.TotalTemplates);
        Assert.Equal(0, result.TotalCards);
    }
}
