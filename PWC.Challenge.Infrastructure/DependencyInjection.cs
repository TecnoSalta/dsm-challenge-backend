using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Domain;
using PWC.Challenge.Domain.Common;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Infrastructure.Data.Respositories;
using System.Diagnostics;
using System.Reflection;
namespace PWC.Challenge.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
       
        var domainAssembly = typeof(DomainAssembly).Assembly;
        var infrastructureAssembly = typeof(InfrastructureAssembly).Assembly;

        services.AddMemoryCache();

        services.AddAuth();

        services.AddDbContexts(configuration);


        services.AddRepositories(domainAssembly, infrastructureAssembly);

        services.AddServices(configuration, environment);


        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IAuthService, AuthService>();

        return services;
    }


    private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext));
        //TODO
        //services.AddScoped<ISaveChangesInterceptor, AuditInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.UseNetTopologySuite();
            });
           
        });

        
        //TODO
        //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, Assembly domainAssembly, Assembly infrastructureAssembly)
    {
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));

        // Registro todos los servicios que implementan IEntityRepository y IAggregateRepository

        var repositoryInterfaces = domainAssembly.GetExportedTypes()
            .Where(t => t.IsInterface && !t.IsGenericTypeDefinition &&
                        (t.GetInterfaces().Any(i => i.IsGenericType &&
                            (i.GetGenericTypeDefinition() == typeof(IRepository<>) ))))
            .ToList();

        var repositoryImplementations = infrastructureAssembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsClass)
            .ToList();

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
       //TODO
        // services.AddSingleton<ITokenService, JwtService>();
        return services;
    }

}
