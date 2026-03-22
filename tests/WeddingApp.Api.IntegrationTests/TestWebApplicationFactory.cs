using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Infrastructure.Data;

namespace WeddingApp.Api.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Fixed database name per factory instance – evaluated once so all scopes share the same store
    private readonly string _dbName = "WeddingApp_Test_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL AppDbContext related registrations, including EF Core
            // IDbContextOptionsConfiguration<T> which accumulates provider registrations
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    d.ServiceType == typeof(IAppDbContext) ||
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            // Use the fixed _dbName so all test scopes within this factory share the same store
            var dbName = _dbName;
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddScoped<IAppDbContext>(p => p.GetRequiredService<AppDbContext>());
        });
    }
}
