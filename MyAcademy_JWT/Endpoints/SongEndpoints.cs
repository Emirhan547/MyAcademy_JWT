using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Services.PackageAccessServices;
using MyAcademy_JWT.Services.SongQueryServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Endpoints;

public static class SongEndpoints
{
    public static void MapSongEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/songs/available",
            [Authorize] async (
                ClaimsPrincipal user,
                [FromServices] ISongQueryService songQueryService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Results.Unauthorized();

                var (ok, error, data) =
                    await songQueryService.GetAvailableSongsAsync(userId);

                return ok ? Results.Ok(data) : Results.BadRequest(error);
            });

        app.MapGet("/api/songs/{id}/stream",
            async (
                int id,
                HttpContext http,
                [FromServices] ISongRepository songRepo,
                [FromServices] IUserSongHistoryRepository historyRepo,
                [FromServices] IPackageAccessService accessService) =>
            {
                var userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return Results.Unauthorized();

                var song = await songRepo.GetWithDetailsAsync(id);
                if (song == null)
                    return Results.NotFound();

                var canAccess =
                    await accessService.CanAccessSongAsync(userId, song.ContentLevel);

                if (!canAccess)
                    return Results.Forbid();

                // 🔥 PlayCount artır
                song.PlayCount++;
                await songRepo.UpdateAsync(song);

                // 🔥 History ekle
                await historyRepo.AddAsync(new UserSongHistory
                {
                    UserId = userId,
                    SongId = song.Id,
                    PlayedAt = DateTime.UtcNow
                });

                var path = Path.Combine("wwwroot/mp3", song.Mp3FileName);

                if (!File.Exists(path))
                    return Results.NotFound();

                return Results.File(path, "audio/mpeg");
            })
        .RequireAuthorization();
    }
}