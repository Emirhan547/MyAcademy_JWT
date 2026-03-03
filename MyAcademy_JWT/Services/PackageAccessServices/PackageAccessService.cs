using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;

namespace MyAcademy_JWT.Services.PackageAccessServices
{
    public class PackageAccessService : IPackageAccessService
    {
        private readonly AppDbContext _context;

        public PackageAccessService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetUserMinLevelAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Package)
                .FirstOrDefaultAsync(x => x.Id == userId);

            return user?.Package?.MinLevel;
        }

        public async Task<bool> CanAccessSongAsync(string userId, int songContentLevel)
        {
            var minLevel = await GetUserMinLevelAsync(userId);
            if (minLevel == null) return false;

            return minLevel.Value <= songContentLevel;
        }
    }
}
