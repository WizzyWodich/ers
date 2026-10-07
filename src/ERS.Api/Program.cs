using ERS.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplicationServices()
    .AddDatabase()
    .AddDependencyInjection();

var app = builder.Build();

await app.UseApplicationMiddleware();

await app.RunAsync();