using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserRepositories;

namespace MyAcademy_JWT.Services.PackageServices
{
    public class PackageAccessService : IPackageAccessService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISongRepository _songRepository;

        public PackageAccessService(IUserRepository userRepository, ISongRepository songRepository)
        {
            _userRepository = userRepository;
            _songRepository = songRepository;
        }

        public async Task<int?> GetUserMinLevelAsync(string userId)
        {
            var user = await _userRepository.GetUserWithPackageAsync(userId);
            return user?.Package?.MinLevel;
        }

        public async Task<bool> CanAccessSongAsync(string userId, int songId)
        {
            var minLevel = await GetUserMinLevelAsync(userId);
            if (minLevel == null) return false;

            var song = await _songRepository.GetByIdAsync(songId);
            if (song == null) return false;

            return song.ContentLevel >= minLevel.Value;
        }
    }
}
