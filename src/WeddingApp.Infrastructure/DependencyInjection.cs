using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeddingApp.Application.Common.Interfaces;
using WeddingApp.Infrastructure.Authentication;
using WeddingApp.Infrastructure.Data;
using WeddingApp.Infrastructure.Services;

namespace WeddingApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.Configure<StorageSettings>(configuration.GetSection("Storage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        return services;
    }
}
