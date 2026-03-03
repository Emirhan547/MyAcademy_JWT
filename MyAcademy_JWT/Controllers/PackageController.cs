using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Entities;

namespace MyAcademy_JWT.Controllers
{
    [Authorize]
    public class PackageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PackageController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Upgrade()
        {
            var user = await _userManager.GetUserAsync(User);

            var packages = await _context.Packages
                .OrderBy(p => p.MinLevel)
                .ToListAsync();

            ViewBag.CurrentPackageId = user?.PackageId;

            return View(packages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upgrade(int packageId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var selectedPackage = await _context.Packages
                .FirstOrDefaultAsync(x => x.Id == packageId);

            if (selectedPackage == null)
                return RedirectToAction("Upgrade");

            user.PackageId = selectedPackage.Id;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Discover", "Default");
        }
    }
}
