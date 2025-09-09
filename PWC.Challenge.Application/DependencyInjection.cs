

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PWC.Challenge.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
       //TODO
        /* var applicationAssembly = typeof(ApplicationAssembly).Assembly;

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(applicationAssembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMapster(applicationAssembly);

        //services.AddAutoMapper(assembly);

        services.AddServices(applicationAssembly);*/

        return services;
    }
}
