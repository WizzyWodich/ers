using ERS.Infrastructure.Persistence;

namespace ERS.Api.Extensions;

public static class SeedExtensions
{
    public static async Task SeedDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ErsDbContext>();
        await DataSeeder.SeedAsync(db);
    }
}