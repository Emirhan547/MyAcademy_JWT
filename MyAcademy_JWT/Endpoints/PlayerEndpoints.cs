using Microsoft.AspNetCore.Authorization;
using MyAcademy_JWT.Services;
using MyAcademy_JWT.Services.PlayServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Endpoints;

public static class PlayerEndpoints
{
    public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/player/stream/{songId:int}", [Authorize] async (
            int songId,
            ClaimsPrincipal user,
            IPlayerService playerService,
            IWebHostEnvironment env) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var (ok, error, filePath) = await playerService.PrepareStreamAsync(userId, songId, env.WebRootPath);

            if (!ok)
            {
                // error string’lerini servis verdiği gibi ele alıyoruz
                return error switch
                {
                    "Forbidden" => Results.Forbid(),
                    "Song not found" => Results.NotFound(error),
                    "Audio file not found" => Results.NotFound(error),
                    _ => Results.BadRequest(error ?? "Error")
                };
            }

            return Results.File(filePath!, "audio/mpeg", enableRangeProcessing: true);
        });
    }
}