using Microsoft.AspNetCore.Authorization;
using MyAcademy_JWT.Services;
using MyAcademy_JWT.Services.HistoryServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Endpoints;

public static class HistoryEndpoints
{
    public static void MapHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/history", [Authorize] async (
            ClaimsPrincipal user,
            IHistoryService historyService,
            int take = 50) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var data = await historyService.GetMyHistoryAsync(userId, take);
            return Results.Ok(data);
        });
    }
}