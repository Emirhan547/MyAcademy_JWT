using Microsoft.AspNetCore.Identity;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<(bool ok, string? error)> RegisterAsync(string email, string password, string displayName, int packageId)
        {
            var exists = await _userManager.FindByEmailAsync(email);
            if (exists != null) return (false, "User already exists.");

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                PackageId = packageId
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return (false, string.Join(" | ", result.Errors.Select(e => e.Description)));

            return (true, null);
        }

        public async Task<(bool ok, string? token, DateTime? expiresUtc)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return (false, null, null);

            var check = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!check.Succeeded) return (false, null, null);

            var (token, expires) = _tokenService.CreateToken(user);
            return (true, token, expires);
        }
    }
}
