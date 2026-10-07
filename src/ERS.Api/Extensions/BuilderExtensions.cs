using ERS.Application.Schedule.GetDay;
using ERS.Domain.Services;
using ERS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERS.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "ERS API";
                document.Info.Version = "v1";
                document.Info.Description = "Electronic Schedule & Replacements";
                return Task.CompletedTask;
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration
            .GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' not configured.");

        builder.Services.AddDbContext<ErsDbContext>(options =>
            options.UseNpgsql(connectionString));

        return builder;
    }

    public static WebApplicationBuilder AddDependencyInjection(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ScheduleResolver>();

        builder.Services.AddScoped<GetDayHandler>();

        return builder;
    }
}