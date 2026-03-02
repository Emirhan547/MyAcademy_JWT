
using MyAcademy_JWT.Services.AuthServices;

namespace MyAcademy_JWT.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/register", async (RegisterRequest req, IAuthService authService) =>
        {
            var (ok, error) = await authService.RegisterAsync(req.Email, req.Password, req.DisplayName, req.PackageId);
            return ok ? Results.Ok("Registered.") : Results.BadRequest(error);
        });

        app.MapPost("/api/login", async (LoginRequest req, IAuthService authService, HttpContext http) =>
        {
            var (ok, token, expiresUtc) = await authService.LoginAsync(req.Email, req.Password);
            if (!ok || token == null || expiresUtc == null) return Results.Unauthorized();

            http.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,               // localde https ile çalıştır
                SameSite = SameSiteMode.Lax,
                Expires = expiresUtc.Value
            });

            return Results.Ok(new { expiresUtc });
        });

        app.MapPost("/api/logout", (HttpContext http) =>
        {
            http.Response.Cookies.Delete("jwt");
            return Results.Ok();
        });
    }

    public record RegisterRequest(string Email, string Password, string DisplayName, int PackageId);
    public record LoginRequest(string Email, string Password);
}