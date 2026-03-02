namespace MyAcademy_JWT.Services.PlayServices
{
    public interface IPlayerService
    {
        Task<(bool ok, string? error, string? filePath)> PrepareStreamAsync(string userId, int songId, string webRootPath);
    }
}
