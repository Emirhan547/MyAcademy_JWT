using Microsoft.AspNetCore.Authorization;
using MyAcademy_JWT.Services;
using MyAcademy_JWT.Services.SongQueryServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Endpoints;

public static class SongEndpoints
{
    public static void MapSongEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/songs/available", [Authorize] async (
            ClaimsPrincipal user,
            ISongQueryService songQueryService) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var (ok, error, data) = await songQueryService.GetAvailableSongsAsync(userId);
            return ok ? Results.Ok(data) : Results.BadRequest(error);
        });
    }
}