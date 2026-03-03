using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Services.TokenServices
{
    public interface ITokenService
    {
        (string token, DateTime expiresUtc) CreateToken(AppUser user);
    }
}
