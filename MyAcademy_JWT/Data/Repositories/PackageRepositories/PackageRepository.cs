using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.PackageRepositories
{
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        public PackageRepository(AppDbContext context) : base(context) { }

        public Task<Package?> GetByNameAsync(string name)
            => _context.Packages.FirstOrDefaultAsync(p => p.Name == name);
    }
}
