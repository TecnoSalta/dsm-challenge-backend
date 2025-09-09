
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PWC.Challenge.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        //TODO
        /*
        var domainAssembly = typeof(DomainAssembly).Assembly;
        var infrastructureAssembly = typeof(InfrastructureAssembly).Assembly;

        services.AddMemoryCache();

        services.AddAuth();

        services.AddDbContexts(configuration);

        services.AddDistributedCaches(configuration);

        services.AddRepositories(domainAssembly, infrastructureAssembly);

        services.AddServices(configuration, environment);

        services.AddSerilog(configuration, environment);*/

        return services;
    }

}    
