using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WeddingApp.Api.IntegrationTests.Controllers;

public class AdminControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;

    public AdminControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidCommand_Returns200WithId()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new
        {
            email = $"admin_{Guid.NewGuid():N}@example.com",
            password = "StrongPass123!",
            firstName = "John",
            lastName = "Admin"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/admin/register", payload);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("id", content);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"logintest_{Guid.NewGuid():N}@example.com";
        const string password = "MySecret123!";

        // Register first
        await client.PostAsJsonAsync("/api/admin/register", new
        {
            email,
            password,
            firstName = "Login",
            lastName = "Test"
        });

        // Act
        var response = await client.PostAsJsonAsync("/api/admin/login", new
        {
            email,
            password
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", content);
    }

    [Fact]
    public async Task Dashboard_WithoutToken_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/admin/dashboard");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Dashboard_WithValidToken_Returns200()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"dashboard_{Guid.NewGuid():N}@example.com";
        const string password = "DashPass123!";

        // Register and login
        await client.PostAsJsonAsync("/api/admin/register", new
        {
            email,
            password,
            firstName = "Dash",
            lastName = "Board"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/admin/login", new
        {
            email,
            password
        });
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginContent).RootElement.GetProperty("token").GetString();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/admin/dashboard");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("totalTemplates", content);
        Assert.Contains("totalCards", content);
    }
}
