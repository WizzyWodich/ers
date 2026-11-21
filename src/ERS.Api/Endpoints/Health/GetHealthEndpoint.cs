using ERS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERS.Api.Endpoints.Health;

public sealed class GetHealthEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("api/health", async (ErsDbContext db, CancellationToken ct) =>
        {
            try
            {
                var hasGroups = await db.Groups.AnyAsync(ct);
                var groupCount = await db.Groups.CountAsync(ct);

                return Results.Ok(new
                {
                    status = "healthy",
                    database = "connected",
                    isEmpty = !hasGroups,
                    groupCount = groupCount
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new
                {
                    status = "unhealthy",
                    database = "disconnected",
                    error = ex.Message
                }, statusCode: 503);
            }
        })
        .WithName("GetHealth")
        .WithTags("System");
    }
}