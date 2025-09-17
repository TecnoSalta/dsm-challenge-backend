using PWC.Challenge.Api;
using PWC.Challenge.Application;
using PWC.Challenge.Application.Services;
using PWC.Challenge.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PWC.Challenge.Domain.Entities;
using PWC.Challenge.Infrastructure.Data;
using PWC.Challenge.Api.Middleware;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        builder.Services.AddSingleton(jsonSerializerOptions);
        builder.Services.AddScoped<PWC.Challenge.Domain.Common.IUnitOfWork, PWC.Challenge.Infrastructure.Data.UnitOfWork>(); // Explicitly added IUnitOfWork registration
        builder.Services
            .AddInfrastructureServices(builder.Configuration, builder.Environment)
            .AddApplicationServices(builder.Configuration)
            .AddApiServices(builder.Configuration, builder.Environment);

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
            // Add other policies as needed
        });

        builder.Services.AddScoped<ICustomerService, CustomerService>();

        builder.Services.AddHttpClient("DurableFunction", client =>
        {
            client.BaseAddress = new Uri("http://pwc.challenge.email.processor:7071");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowSpecificOrigin",
                              builder =>
                              {
                                  builder.WithOrigins("http://localhost:4200")
                                         .AllowAnyHeader()
                                         .AllowAnyMethod();
                              });
        });



        var app = builder.Build();

        app.UseMiddleware<DomainExceptionMiddleware>();

        // Configure the HTTP request pipeline.

                app.UseApiServices(builder.Configuration, builder.Environment);

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            //TODO
            //await app.ApplyDatabaseMigrationsAsync();
        }

        app.Run();
    }
}