using PWC.Challenge.Api;
using PWC.Challenge.Application;
using PWC.Challenge.Application.Features.Rentals.Commands.CompleteRental.Services;
using PWC.Challenge.Application.Features.Rentals.Commands.UpdateRental.Services;
using PWC.Challenge.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

var jsonSerializerOptions = new JsonSerializerOptions
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};

builder.Services.AddSingleton(jsonSerializerOptions);
builder.Services
    .AddInfrastructureServices(builder.Configuration, builder.Environment)
    .AddApplicationServices(builder.Configuration)
    .AddApiServices(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<ICompleteRentalService, CompleteRentalService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseApiServices(builder.Configuration, builder.Environment);

if (app.Environment.IsDevelopment())
{
    //TODO
    //await app.ApplyDatabaseMigrationsAsync();
}

app.Run();
