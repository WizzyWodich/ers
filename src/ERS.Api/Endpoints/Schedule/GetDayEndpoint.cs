using ERS.Application.Schedule.GetDay;

namespace ERS.Api.Endpoints.Schedule;

public sealed class GetDayEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet("/schedule/{groupId:guid}/day/{date}",
            async (
                Guid groupId,
                DateOnly date,
                GetDayHandler handler,
                CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(groupId, date, ct);

                return result is null
                    ? Results.NotFound(new { error = "Группа не найдена" })
                    : Results.Ok(result);
            })
            .WithName("GetDaySchedule")
            .WithTags("Schedule");
    }
}