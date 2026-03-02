namespace MyAcademy_JWT.Services.SongQueryServices
{
    public interface ISongQueryService
    {
        Task<(bool ok, string? error, object? data)> GetAvailableSongsAsync(string userId);
    }
}
