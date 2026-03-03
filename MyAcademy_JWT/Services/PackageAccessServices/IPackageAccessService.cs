namespace MyAcademy_JWT.Services.PackageAccessServices
{
    public interface IPackageAccessService
    {
        Task<int?> GetUserMinLevelAsync(string userId);
        Task<bool> CanAccessSongAsync(string userId, int songContentLevel);
    }
}
