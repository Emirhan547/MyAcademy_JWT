using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Services.PackageServices;

namespace MyAcademy_JWT.Services.PlayServices
{
    public class PlayerService : IPlayerService
    {
        private readonly IPackageAccessService _accessService;
        private readonly ISongRepository _songRepository;
        private readonly IUserSongHistoryRepository _historyRepository;

        public PlayerService(
            IPackageAccessService accessService,
            ISongRepository songRepository,
            IUserSongHistoryRepository historyRepository)
        {
            _accessService = accessService;
            _songRepository = songRepository;
            _historyRepository = historyRepository;
        }

        public async Task<(bool ok, string? error, string? filePath)> PrepareStreamAsync(string userId, int songId, string webRootPath)
        {
            var canAccess = await _accessService.CanAccessSongAsync(userId, songId);
            if (!canAccess) return (false, "Forbidden", null);

            var song = await _songRepository.GetByIdAsync(songId);
            if (song == null) return (false, "Song not found", null);

            // playcount
            song.PlayCount++;
            _songRepository.Update(song);
            await _songRepository.SaveChangesAsync();

            // history
            await _historyRepository.AddAsync(new UserSongHistory
            {
                UserId = userId,
                SongId = song.Id,
                PlayedAtUtc = DateTime.UtcNow
            });
            await _historyRepository.SaveChangesAsync();

            var filePath = Path.Combine(webRootPath, "songs", song.Mp3FileName);
            if (!File.Exists(filePath)) return (false, "Audio file not found", null);

            return (true, null, filePath);
        }
    }
}
