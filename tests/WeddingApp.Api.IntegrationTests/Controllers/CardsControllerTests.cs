using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using WeddingApp.Domain.Entities;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Api.IntegrationTests.Controllers;

public class CardsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public CardsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<Guid> SeedTemplateAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var template = new Template("Classic", "<html></html>");
        context.Templates.Add(template);
        await context.SaveChangesAsync();
        return template.Id;
    }

    [Fact]
    public async Task Post_ValidCard_Returns201Created()
    {
        // Arrange
        var templateId = await SeedTemplateAsync();
        var client = _factory.CreateClient();
        var payload = new
        {
            title = "John & Jane Wedding",
            slugUrl = "john-jane-wedding",
            eventDate = DateTime.UtcNow.AddMonths(6),
            templateId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/cards", payload);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("id", content);
    }

    [Fact]
    public async Task Post_InvalidSlug_Returns400BadRequest()
    {
        // Arrange
        var templateId = await SeedTemplateAsync();
        var client = _factory.CreateClient();
        var payload = new
        {
            title = "Wedding",
            slugUrl = "Invalid Slug With Spaces!", // invalid
            eventDate = DateTime.UtcNow.AddMonths(1),
            templateId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/cards", payload);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_ExistingSlug_Returns200WithCard()
    {
        // Arrange
        var templateId = await SeedTemplateAsync();
        var client = _factory.CreateClient();

        // Create a card first
        var slug = "test-wedding-slug";
        var createPayload = new
        {
            title = "Test Wedding",
            slugUrl = slug,
            eventDate = DateTime.UtcNow.AddMonths(6),
            templateId
        };
        var createResponse = await client.PostAsJsonAsync("/api/cards", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        // Act
        var response = await client.GetAsync($"/api/cards/{slug}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Wedding", content);
        Assert.Contains(slug, content);
    }

    [Fact]
    public async Task Get_NonExistentSlug_Returns404()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/cards/non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
