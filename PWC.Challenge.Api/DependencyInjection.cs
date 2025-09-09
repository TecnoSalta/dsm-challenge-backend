using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

namespace PWC.Challenge.Api;

public static class DependencyInjection
{
    public const string CorsPolicyName = "ChallengeCorsPolicy";

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
      
        var dbConnectionString = configuration.GetConnectionString("CADENATODO");
        var redisConnectionString = configuration.GetConnectionString("Redis");

        //TODO
        //services.AddExceptionHandler<CustomExceptionHandler>();
        //TODO
     
        //services.AddCors(configuration);

       // services.AddInternalAuthentication(configuration, environment);

       // services.AddScoped<PermissionAuthorizationFilter>();

        services.AddControllers(config =>
        {
            //config.Filters.AddService<PermissionAuthorizationFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
           // options.SchemaFilter<SwaggerIgnoreFilter>();
        });

       

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsEnvironment("Development") || environment.IsEnvironment("QA"))
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(options => { });

       

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        var storagePath = configuration["Storage:Resources:Path"];
        if (string.IsNullOrWhiteSpace(storagePath))
        {
            storagePath = Path.Combine(environment.ContentRootPath, "resources");
        }

        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(storagePath),
            RequestPath = "/resources"
        });

        app.UseCors(CorsPolicyName);


        app.UseAuthentication();

        //TODO
        //app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}