using ERS.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddApplicationServices()
    .AddDatabase()
    .AddDependencyInjection()
    .AddCors();

var app = builder.Build();

app.UseCors();
await app.UseApplicationMiddleware(); 
await app.RunAsync(); 