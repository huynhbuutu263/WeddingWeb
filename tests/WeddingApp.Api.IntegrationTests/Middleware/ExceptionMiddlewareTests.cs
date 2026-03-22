using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeddingApp.Api.Middleware;
using WeddingApp.Application.Common.Exceptions;

namespace WeddingApp.Api.IntegrationTests.Middleware;

public class ExceptionMiddlewareTests
{
    private static WebApplicationFactory<Program> CreateFactory(RequestDelegate handler)
    {
        return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseTestServer();
            builder.ConfigureServices(services =>
            {
                services.AddRouting();
            });
            builder.Configure(app =>
            {
                app.UseMiddleware<ExceptionMiddleware>();
                app.Run(handler);
            });
        });
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_Returns400WithErrors()
    {
        // Arrange
        var failures = new[]
        {
            new FluentValidation.Results.ValidationFailure("Title", "Title is required.")
        };
        var factory = CreateFactory(_ => throw new ValidationException(failures));
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Title", content);
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_Returns404()
    {
        // Arrange
        var factory = CreateFactory(_ => throw new NotFoundException("WeddingCard", Guid.NewGuid()));
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task InvokeAsync_UnhandledException_Returns500()
    {
        // Arrange
        var factory = CreateFactory(_ => throw new InvalidOperationException("Unexpected"));
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Unexpected", content);
    }

    [Fact]
    public async Task InvokeAsync_NoException_PassesThrough()
    {
        // Arrange
        var factory = CreateFactory(async ctx =>
        {
            ctx.Response.StatusCode = 200;
            await ctx.Response.WriteAsync("ok");
        });
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
