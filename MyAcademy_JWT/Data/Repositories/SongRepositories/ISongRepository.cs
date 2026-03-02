using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.SongRepositories
{
    public interface ISongRepository : IGenericRepository<Song>
    {
        Task<Song?> GetWithDetailsAsync(int id);
        Task<List<Song>> GetAvailableSongsAsync(int minLevel);
    }
}
