using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Services.PackageServices;

namespace MyAcademy_JWT.Services.SongQueryServices
{
    public class SongQueryService : ISongQueryService
    {
        private readonly IPackageAccessService _accessService;
        private readonly ISongRepository _songRepository;

        public SongQueryService(IPackageAccessService accessService, ISongRepository songRepository)
        {
            _accessService = accessService;
            _songRepository = songRepository;
        }

        public async Task<(bool ok, string? error, object? data)> GetAvailableSongsAsync(string userId)
        {
            var minLevel = await _accessService.GetUserMinLevelAsync(userId);
            if (minLevel == null) return (false, "User package not found.", null);

            var songs = await _songRepository.GetAvailableSongsAsync(minLevel.Value);

            var result = songs.Select(s => new { s.Id, s.Title, s.Duration, s.PlayCount, s.ContentLevel }).ToList();
            return (true, null, result);
        }
    }
}
