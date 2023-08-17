using Mapster;
using Ohtic.Test.Products.Extensions;
using Ohtic.Test.Products.Factories;

var builder = WebApplication.CreateBuilder(args);

var jsonOptions = JsonSerializerOptionsFactory.Create();

// Add services to the container.

builder.Services.ConfigureData(builder.Configuration);
builder.Services.ConfigureServices(builder.Configuration, jsonOptions);
builder.Services.ConfigureApi(jsonOptions);
builder.Services.ConfigureAuthentication(builder.Configuration);

TypeAdapterConfig.GlobalSettings.ConfigureMapping();

var app = builder.Build();
app.Configure();
app.Run();
