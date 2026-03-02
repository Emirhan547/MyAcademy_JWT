using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories
{
    public class UserSongHistoryRepository : GenericRepository<UserSongHistory>, IUserSongHistoryRepository
    {
        public UserSongHistoryRepository(AppDbContext context) : base(context) { }

        public async Task<List<UserSongHistory>> GetUserHistoryAsync(string userId, int take = 50)
        {
            return await _context.UserSongHistories
                .AsNoTracking()
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.PlayedAtUtc)
                .Take(take)
                .ToListAsync();
        }
        public async Task<List<(string UserId, int SongId)>> GetAllInteractionsAsync(int takeLast = 50000)
        {

            var rows = await _context.UserSongHistories
                .AsNoTracking()
                .OrderByDescending(x => x.PlayedAtUtc)
                .Take(takeLast)
                .Select(x => new { x.UserId, x.SongId })
                .ToListAsync();

            return rows.Select(r => (r.UserId, r.SongId)).ToList();
        }

        public async Task<HashSet<int>> GetPlayedSongIdsAsync(string userId)
        {
            var ids = await _context.UserSongHistories
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => x.SongId)
                .Distinct()
                .ToListAsync();

            return ids.ToHashSet();
        }
    }
}
