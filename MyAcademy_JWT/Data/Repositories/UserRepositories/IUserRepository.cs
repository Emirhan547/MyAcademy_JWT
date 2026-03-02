using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserWithPackageAsync(string userId);
    }
}
