using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.SongRepositories
{
    public class SongRepository : GenericRepository<Song>, ISongRepository
    {
        public SongRepository(AppDbContext context) : base(context) { }

        public async Task<Song?> GetWithDetailsAsync(int id)
        {
            return await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Song>> GetAvailableSongsAsync(int minLevel)
        {
            return await _context.Songs
                .AsNoTracking()
                .Where(s => s.ContentLevel >= minLevel)
                .OrderBy(s => s.Title)
                .ToListAsync();
        }
    }
}
