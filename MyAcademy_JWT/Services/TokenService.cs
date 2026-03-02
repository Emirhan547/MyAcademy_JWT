using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyAcademy_JWT.Services
{
    public class TokenService
    {
        private readonly JwtOptions _opt;

        public TokenService(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
        }

        public (string token, DateTime expiresUtc) CreateToken(AppUser user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Name, user.UserName ?? "")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_opt.ExpiresMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
        }
    }
}
