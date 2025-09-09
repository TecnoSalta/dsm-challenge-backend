using System.Text.Json.Serialization;
using System.Text.Json;
using PWC.Challenge.Infrastructure;
using PWC.Challenge.Application;
using PWC.Challenge.Api;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuraci�n global de JsonSerializerOptions
var jsonSerializerOptions = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};

builder.Services.AddSingleton(jsonSerializerOptions);
builder.Services
    .AddInfrastructureServices(builder.Configuration, builder.Environment)
    .AddApplicationServices(builder.Configuration)
    .AddApiServices(builder.Configuration, builder.Environment);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseApiServices(builder.Configuration, builder.Environment);

if (app.Environment.IsDevelopment())
{
    //TODO
    //await app.ApplyDatabaseMigrationsAsync();
}

app.Run();
