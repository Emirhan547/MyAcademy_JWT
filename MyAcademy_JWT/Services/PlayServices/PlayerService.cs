using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Services.PackageAccessServices; 

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

        public async Task<(bool ok, string? error, string? filePath)>
 PrepareStreamAsync(string userId, int songId, string webRootPath)
        {
            var song = await _songRepository.GetByIdAsync(songId);
            if (song == null)
                return (false, "Song not found", null);

            var canAccess =
                await _accessService.CanAccessSongAsync(userId, song.ContentLevel);

            if (!canAccess)
                return (false, "Forbidden", null);

            // 🔥 PlayCount artır
            song.PlayCount++;
            await _songRepository.UpdateAsync(song);

            // 🔥 History
            await _historyRepository.AddAsync(new UserSongHistory
            {
                UserId = userId,
                SongId = song.Id,
                PlayedAt = DateTime.UtcNow
            });

            var filePath = Path.Combine(webRootPath, "songs", song.Mp3FileName);
            if (!File.Exists(filePath))
                return (false, "Audio file not found", null);

            return (true, null, filePath);
        }
    }
}
