using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PWC.Challenge.Application.Interfaces;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Domain;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Domain.Interfaces;
using PWC.Challenge.Domain.Services;
using PWC.Challenge.Infrastructure.Configurations;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Common;
using PWC.Challenge.Application.Interfaces;
using PWC.Challenge.Infrastructure.Services;
using StackExchange.Redis;
using System.Reflection;
using PWC.Challenge.Infrastructure.Data.Interceptors;

namespace PWC.Challenge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var domainAssembly = typeof(DomainAssembly).Assembly;
        var infrastructureAssembly = typeof(InfrastructureAssembly).Assembly;

        services.AddMemoryCache();

        // Agregar Redis Cache
        services.AddRedisCache(configuration);

        services.AddAuth();

        services.AddDbContexts(configuration);

        services.AddRepositories(domainAssembly, infrastructureAssembly);

        services.AddServices(configuration, environment);

        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            // Configurar Redis Settings
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));

            // Configurar Redis Distributed Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = configuration["RedisSettings:InstanceName"] ?? "PWChallenge:";
            });

            // Configurar Connection Multiplexer para operaciones avanzadas
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnectionString);
                config.AbortOnConnectFail = false;
                config.ConnectRetry = 3;
                config.ConnectTimeout = 5000;
                return ConnectionMultiplexer.Connect(config);
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            // Fallback a memoria si Redis no está configurado
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        }

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>(); // Added

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.EnableSensitiveDataLogging(); // Added for debugging
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseNetTopologySuite();
            });
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, Assembly domainAssembly, Assembly infrastructureAssembly)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        var repositoryInterfaces = domainAssembly.GetExportedTypes()
            .Where(t => t.IsInterface && !t.IsGenericTypeDefinition &&
                        (t.GetInterfaces().Any(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IBaseRepository<>))))).ToList();

        var repositoryImplementations = infrastructureAssembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsClass).ToList();

        foreach (var repositoryInterface in repositoryInterfaces)
        {
            var repositoryImplementation = repositoryImplementations
                .FirstOrDefault(impl => repositoryInterface.IsAssignableFrom(impl));

            if (repositoryImplementation != null)
            {
                services.AddScoped(repositoryInterface, repositoryImplementation);
            }
        }

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Registrar servicios de dominio que necesiten cache
        services.AddScoped<IAvailabilityService>(sp =>
        {
            var originalService = new AvailabilityService(
                sp.GetRequiredService<ICarRepository>(),
                sp.GetRequiredService<IRentalRepository>());

            var cacheService = sp.GetRequiredService<ICacheService>();
            var logger = sp.GetRequiredService<ILogger<CachedAvailabilityService>>();

            return new CachedAvailabilityService(originalService, cacheService, logger);
        });

        // Register RentalAvailabilityService
        services.AddScoped<IRentalAvailabilityService, RentalAvailabilityService>();

        // Register Clock Service
        services.AddSingleton<IClock, SystemClock>();

        // Register Current User Service
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IStatisticService, StatisticService>();

        return services;
    }
}