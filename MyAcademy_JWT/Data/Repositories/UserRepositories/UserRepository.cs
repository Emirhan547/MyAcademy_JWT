using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Data.Repositories.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetUserWithPackageAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Package)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
