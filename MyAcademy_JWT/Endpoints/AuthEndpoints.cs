using Microsoft.AspNetCore.Identity;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Services;

namespace MyAcademy_JWT.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/register", async (RegisterRequest req, UserManager<AppUser> userManager) =>
            {
                var exists = await userManager.FindByEmailAsync(req.Email);
                if (exists != null) return Results.BadRequest("User already exists.");

                var user = new AppUser
                {
                    UserName = req.Email,
                    Email = req.Email,
                    DisplayName = req.DisplayName,
                    PackageId = req.PackageId
                };

                var result = await userManager.CreateAsync(user, req.Password);
                if (!result.Succeeded) return Results.BadRequest(result.Errors);

                return Results.Ok("Registered.");
            });

            app.MapPost("/api/login", async (LoginRequest req,
                UserManager<AppUser> userManager,
                SignInManager<AppUser> signInManager,
                TokenService tokenService,
                HttpContext http) =>
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user == null) return Results.Unauthorized();

                var check = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
                if (!check.Succeeded) return Results.Unauthorized();

                var (token, expires) = tokenService.CreateToken(user);

                http.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = expires
                });

                return Results.Ok(new { expiresUtc = expires });
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
}
