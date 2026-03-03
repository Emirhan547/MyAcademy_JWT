using Microsoft.ML;
using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Models.MLNetViewModels;
using MyAcademy_JWT.Services.PackageAccessServices;
using MyAcademy_JWT.Services.RecommendationServices;

namespace MyAcademy_JWT.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IUserSongHistoryRepository _historyRepo;
    private readonly ISongRepository _songRepo;
    private readonly IPackageAccessService _accessService;
    private readonly MLModelState _modelState;

    public RecommendationService(
        IUserSongHistoryRepository historyRepo,
        ISongRepository songRepo,
        IPackageAccessService accessService,
        MLModelState modelState)
    {
        _historyRepo = historyRepo;
        _songRepo = songRepo;
        _accessService = accessService;
        _modelState = modelState;
    }

    public async Task<object> GetRecommendationsAsync(string userId, int take = 10)
    {
        var minLevel = await _accessService.GetUserMinLevelAsync(userId);
        if (minLevel == null)
            return Array.Empty<object>();

        var availableSongs = await _songRepo.GetAvailableSongsAsync(minLevel.Value);
        if (availableSongs.Count == 0)
            return Array.Empty<object>();

        var availableSongIds = availableSongs.Select(s => s.Id).ToList();

        var playedIds = await _historyRepo.GetPlayedSongIdsAsync(userId);
        var candidateIds = availableSongIds
            .Where(id => !playedIds.Contains(id))
            .ToList();

        if (candidateIds.Count == 0)
        {
            return availableSongs
                .OrderByDescending(s => s.PlayCount)
                .Take(take)
                .Select(s => new { s.Id, s.Title, s.PlayCount })
                .ToList();
        }

        var model = _modelState.Model;

        if (model == null)
        {
            // Model henüz eğitilmediyse fallback
            return availableSongs
                .OrderByDescending(s => s.PlayCount)
                .Take(take)
                .Select(s => new { s.Id, s.Title, s.PlayCount })
                .ToList();
        }

        var ml = new MLContext();
        var engine = ml.Model
            .CreatePredictionEngine<SongInteraction, SongScorePrediction>(model);

        var scored = new List<(int SongId, float Score)>();

        foreach (var sid in candidateIds)
        {
            var pred = engine.Predict(new SongInteraction
            {
                UserId = userId,
                SongId = sid.ToString(),
                Label = 0f
            });

            scored.Add((sid, pred.Score));
        }

        var top = scored
            .OrderByDescending(x => x.Score)
            .Take(take)
            .ToList();

        var dict = availableSongs.ToDictionary(s => s.Id);

        return top.Select(x =>
        {
            var s = dict[x.SongId];
            return new
            {
                s.Id,
                s.Title,
                s.PlayCount,
                score = x.Score
            };
        }).ToList();
    }
}