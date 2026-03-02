using Microsoft.AspNetCore.Identity;

namespace MyAcademy_JWT.Entities
{
    public class AppUser:IdentityUser
    {
        public string? DisplayName { get; set; }
        public int PackageId { get; set; } = 6;
        public Package? Package { get; set; }
    }
}
