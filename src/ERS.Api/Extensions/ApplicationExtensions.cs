using ERS.Api.Endpoints;
using Scalar.AspNetCore;

namespace ERS.Api.Extensions;

public static class ApplicationExtensions
{
    public static async Task UseApplicationMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "ERS API";
            });
            app.MapGet("/", () => Results.Redirect("/scalar"));

            await app.Services.SeedDatabaseAsync();
        }

        app.UseHttpsRedirection();

        app.MapEndpoints();

        await app.RunAsync();
    }
}