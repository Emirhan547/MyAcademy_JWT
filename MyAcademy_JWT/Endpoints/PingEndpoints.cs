namespace MyAcademy_JWT.Endpoints
{
    public static class PingEndpoints
    {
        public static void MapPingEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/ping", () => Results.Ok(new { ok = true, atUtc = DateTime.UtcNow }));
        }
    }
}
