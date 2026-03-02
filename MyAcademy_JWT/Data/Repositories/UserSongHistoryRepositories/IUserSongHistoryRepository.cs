using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories
{
    public interface IUserSongHistoryRepository : IGenericRepository<UserSongHistory>
    {
        Task<List<UserSongHistory>> GetUserHistoryAsync(string userId, int take = 50);
        Task<List<(string UserId, int SongId)>> GetAllInteractionsAsync(int takeLast = 50000);
        Task<HashSet<int>> GetPlayedSongIdsAsync(string userId);
    }
}
