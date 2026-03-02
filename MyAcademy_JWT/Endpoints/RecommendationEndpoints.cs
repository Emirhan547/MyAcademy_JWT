using Microsoft.AspNetCore.Authorization;
using MyAcademy_JWT.Services;
using MyAcademy_JWT.Services.RecommendationServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Endpoints;

public static class RecommendationEndpoints
{
    public static void MapRecommendationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/recommendations", [Authorize] async (
            ClaimsPrincipal user,
            IRecommendationService recService,
            int take = 10) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Results.Unauthorized();

            var data = await recService.GetRecommendationsAsync(userId, take);
            return Results.Ok(data);
        });
    }
}