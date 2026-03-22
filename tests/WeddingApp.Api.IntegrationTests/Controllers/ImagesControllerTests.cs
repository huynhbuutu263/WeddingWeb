using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WeddingApp.Application.Common.Interfaces;

namespace WeddingApp.Api.IntegrationTests.Controllers;

public class ImagesControllerTests
{
    private sealed class FakeFileStorageService : IFileStorageService
    {
        public Task<string> UploadImageAsync(Stream data, string filename, CancellationToken cancellationToken = default)
            => Task.FromResult($"/uploads/fake_{filename}");
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        return new TestWebApplicationFactory().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IFileStorageService));
                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddScoped<IFileStorageService, FakeFileStorageService>();
            });
        });
    }

    [Fact]
    public async Task Post_ValidFile_Returns200WithUrl()
    {
        // Arrange
        using var factory = CreateFactory();
        var client = factory.CreateClient();

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("fake image bytes"));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "photo.jpg");

        // Act
        var response = await client.PostAsync("/api/images", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("url", body);
        Assert.Contains("photo.jpg", body);
    }

    [Fact]
    public async Task Post_ValidFile_ReturnsUploadUrl()
    {
        // Arrange
        using var factory = CreateFactory();
        var client = factory.CreateClient();

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 0xFF, 0xD8, 0xFF });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "file", "wedding.jpg");

        // Act
        var response = await client.PostAsync("/api/images", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("/uploads/", body);
    }
}
