using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;

namespace MyAcademy_JWT.Services.HistoryServices
{
    public class HistoryService : IHistoryService
    {
        private readonly IUserSongHistoryRepository _historyRepo;
        private readonly ISongRepository _songRepo;

        public HistoryService(IUserSongHistoryRepository historyRepo, ISongRepository songRepo)
        {
            _historyRepo = historyRepo;
            _songRepo = songRepo;
        }

        public async Task<object> GetMyHistoryAsync(string userId, int take = 50)
        {
            var history = await _historyRepo.GetUserHistoryAsync(userId, take);

            return history.Select(h => new { h.SongId, h.PlayedAt }).ToList();
        }
    }
}
