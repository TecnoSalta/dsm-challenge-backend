

using FluentValidation;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PWC.Challenge.Application.Common;
using PWC.Challenge.Application.Mappings.Mapster;
using PWC.Challenge.Domain.Entities;
using System.Reflection;


namespace PWC.Challenge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
       var applicationAssembly = typeof(ApplicationAssembly).Assembly;
        var domainAssembly = typeof(Rental).Assembly; 


        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(applicationAssembly, domainAssembly);
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMapster(applicationAssembly);


        services.AddServices(applicationAssembly);

        return services;
    }
    private static IServiceCollection AddMapster(this IServiceCollection services, Assembly assembly)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Default.PreserveReference(true);
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<MapsterMapper.IMapper, MapsterMapper.Mapper>();
        MapsterMappingConfig.RegisterMappings(assembly);

        return services;
    }

     private static IServiceCollection AddServices(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));

        // Registro todos los servicios que implementan IEntityService y IAggregateService

        var serviceInterfaces = applicationAssembly.GetExportedTypes()
                .Where(t => t.IsInterface && !t.IsGenericTypeDefinition &&
                            (t.GetInterfaces().Any(i => i.IsGenericType &&
                                (i.GetGenericTypeDefinition() == typeof(IBaseService<,>)))))
                .ToList();

        var serviceImplementations = applicationAssembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsClass)
            .ToList();

        foreach (var serviceInterface in serviceInterfaces)
        {
            var serviceImplementation = serviceImplementations
                .FirstOrDefault(impl => serviceInterface.IsAssignableFrom(impl));

            if (serviceImplementation != null)
            {
                services.AddScoped(serviceInterface, serviceImplementation);
            }
        }

        return services;
    }
}
