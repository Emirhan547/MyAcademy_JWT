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
        public async Task<IActionResult> Index()
        {
            var songs = await _songRepo.GetAllWithArtistAsync();

            var model = new HomeViewModel
            {
                TotalSongCount = songs.Count,
                TotalArtistCount = songs.Select(x => x.ArtistId).Distinct().Count(),
                TotalPlayCount = songs.Sum(x => x.PlayCount),
                TrendingSongs = songs
                    .OrderByDescending(x => x.PlayCount)
                    .Take(4)
                    .Select(x => new HomeSongViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        ArtistName = x.Artist?.Name ?? "Unknown",
                        CoverImageUrl = x.Album?.CoverImageUrl,
                        PlayCount = x.PlayCount
                    })
                    .ToList(),
                PopularArtists = songs
                    .GroupBy(x => x.Artist?.Name ?? "Unknown")
                    .Select(g => new HomeArtistViewModel
                    {
                        Name = g.Key,
                        SongCount = g.Count(),
                        TotalPlayCount = g.Sum(s => s.PlayCount)
                    })
                    .OrderByDescending(x => x.TotalPlayCount)
                    .Take(5)
                    .ToList()
            };

            return View(model);
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
