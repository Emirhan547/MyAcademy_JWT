using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.PackageRepositories
{
    public interface IPackageRepository : IGenericRepository<Package>
    {
        Task<Package?> GetByNameAsync(string name);
    }
}
