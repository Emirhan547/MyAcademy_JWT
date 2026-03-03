using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Models;
using MyAcademy_JWT.Services.PackageAccessServices;
using MyAcademy_JWT.Services.RecommendationServices;
using System.Security.Claims;

namespace MyAcademy_JWT.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ISongRepository _songRepo;
        private readonly IPackageAccessService _accessService;
        IRecommendationService _recommendationService;
        public DefaultController(IPackageAccessService accessService, ISongRepository songRepo, IRecommendationService recommendationService)
        {
            _accessService = accessService;
            _songRepo = songRepo;
            _recommendationService = recommendationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Discover()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var songs = await _songRepo.GetAllWithArtistAsync();

            var recommendations = await _recommendationService
                .GetRecommendationsAsync(userId, 6);

            var list = new List<SongCardViewModel>();

            foreach (var song in songs)
            {
                var canAccess = await _accessService
                    .CanAccessSongAsync(userId, song.ContentLevel);

                list.Add(new SongCardViewModel
                {
                    Id = song.Id,
                    Title = song.Title,
                    ArtistName = song.Artist?.Name ?? "",
                    CoverImageUrl = song.Album?.CoverImageUrl,
                    PlayCount = song.PlayCount,
                    CanAccess = canAccess
                });
            }

            ViewBag.Recommendations = recommendations;

            return View(list);
        }
    }
}
