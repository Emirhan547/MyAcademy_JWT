using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Models;

namespace MyAcademy_JWT.ViewComponents.UILayout
{
    public class _UILayoutNavbarViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public _UILayoutNavbarViewComponent(
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity!.IsAuthenticated)
                return View(null);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return View(null);

            var package = await _context.Packages
                .FirstOrDefaultAsync(x => x.Id == user.PackageId);

            var model = new NavbarViewModel
            {
                DisplayName = user.DisplayName, // yoksa UserName kullan
                PackageName = package?.Name ?? "Free"
            };

            return View(model);
        }
    }
}