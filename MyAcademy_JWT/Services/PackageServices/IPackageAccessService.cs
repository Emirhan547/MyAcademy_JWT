namespace MyAcademy_JWT.Services.PackageServices
{
    public interface IPackageAccessService
    {
        Task<int?> GetUserMinLevelAsync(string userId);
        Task<bool> CanAccessSongAsync(string userId, int songId);
    }
}
