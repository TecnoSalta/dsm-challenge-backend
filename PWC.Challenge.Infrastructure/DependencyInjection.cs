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
using PWC.Challenge.Infrastructure.Services;
using StackExchange.Redis;
using System.Reflection;
using PWC.Challenge.Infrastructure.Data.Interceptors;
using PWC.Challenge.Infrastructure.Data.Repositories;

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

        services.AddAuth(configuration); // Pasar configuración

        services.AddDbContexts(configuration, environment); // Pasar environment

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

            services.AddSingleton<ICacheService, RedisCacheService>(); // Cambiado a RedisCacheService
        }
        else
        {
            // Fallback a memoria si Redis no está configurado
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
        }

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        // Agregar autenticación JWT si es necesario
        // services.AddJwtAuthentication(configuration);

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{nameof(ApplicationDbContext)}' not found.");
        }

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            // Solo habilitar en desarrollo
            options.EnableSensitiveDataLogging(environment.IsDevelopment());
            options.EnableDetailedErrors(environment.IsDevelopment());

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseNetTopologySuite();
                npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                // Configuraciones adicionales para mejor rendimiento
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, Assembly domainAssembly, Assembly infrastructureAssembly)
    {
        // Registrar repositorio base primero
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // Registrar repositorios específicos explícitamente
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();

        // Registro automático para otros repositorios
        var repositoryInterfaces = domainAssembly.GetExportedTypes()
            .Where(t => t.IsInterface &&
                       !t.IsGenericTypeDefinition &&
                       t.Name.EndsWith("Repository") &&
                       t != typeof(IBaseRepository<>))
            .ToList();

        var repositoryImplementations = infrastructureAssembly.GetExportedTypes()
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       t.Name.EndsWith("Repository"))
            .ToList();

        foreach (var repositoryInterface in repositoryInterfaces)
        {
            // Saltar interfaces ya registradas explícitamente
            if (repositoryInterface == typeof(ICarRepository) ||
                repositoryInterface == typeof(ICustomerRepository) ||
                repositoryInterface == typeof(IRentalRepository))
            {
                continue;
            }

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
        // Registrar servicios con caching
        services.AddScoped<IAvailabilityService>(sp =>
        {
            var originalService = new AvailabilityService(
                sp.GetRequiredService<ICarRepository>(),
                sp.GetRequiredService<IRentalRepository>());

            var cacheService = sp.GetRequiredService<ICacheService>();
            var logger = sp.GetRequiredService<ILogger<CachedAvailabilityService>>();

            return new CachedAvailabilityService(originalService, cacheService, logger);
        });

        // Registrar otros servicios
        services.AddScoped<IRentalAvailabilityService, RentalAvailabilityService>();
        services.AddScoped<IStatisticService, StatisticService>();

        // Registrar servicios singleton
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}