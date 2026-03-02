using Microsoft.ML;

using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;

using MyAcademy_JWT.Models.MLNetViewModels;

using MyAcademy_JWT.Services.PackageServices;
using MyAcademy_JWT.Services.RecommendationServices;

namespace MyAcademy_JWT.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IUserSongHistoryRepository _historyRepo;
    private readonly ISongRepository _songRepo;
    private readonly IPackageAccessService _accessService;

    private static readonly object _lock = new();
    private static ITransformer? _model;
    private static DataViewSchema? _modelSchema;
    private static DateTime _lastTrainUtc = DateTime.MinValue;

    public RecommendationService(
        IUserSongHistoryRepository historyRepo,
        ISongRepository songRepo,
        IPackageAccessService accessService)
    {
        _historyRepo = historyRepo;
        _songRepo = songRepo;
        _accessService = accessService;
    }

    public async Task<object> GetRecommendationsAsync(string userId, int take = 10)
    {
        // 1) Kullanıcının paketine göre erişebileceği şarkılar
        var minLevel = await _accessService.GetUserMinLevelAsync(userId);
        if (minLevel == null) return new { success = false, message = "User package not found." };

        var availableSongs = await _songRepo.GetAvailableSongsAsync(minLevel.Value);
        if (availableSongs.Count == 0) return Array.Empty<object>();

        var availableSongIds = availableSongs.Select(s => s.Id).ToList();

        // 2) Kullanıcının daha önce dinlediklerini çıkar
        var playedIds = await _historyRepo.GetPlayedSongIdsAsync(userId);
        var candidateIds = availableSongIds.Where(id => !playedIds.Contains(id)).ToList();

        // Candidate yoksa: boş döndür veya top/popular döndür
        if (candidateIds.Count == 0)
        {
            return availableSongs
                .OrderByDescending(s => s.PlayCount)
                .Take(take)
                .Select(s => new { s.Id, s.Title, s.PlayCount })
                .ToList();
        }

        // 3) Model hazır değilse / eskiyse eğit
        await EnsureModelAsync();

        // Model yine yoksa fallback
        if (_model == null)
        {
            return availableSongs
                .OrderByDescending(s => s.PlayCount)
                .Take(take)
                .Select(s => new { s.Id, s.Title, s.PlayCount })
                .ToList();
        }

        // 4) Skorla ve sırala
        var ml = new MLContext();

        var engine = ml.Model.CreatePredictionEngine<SongInteraction, SongScorePrediction>(_model);

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

        // 5) Response (id + title)
        var dict = availableSongs.ToDictionary(s => s.Id, s => s);

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

    private async Task EnsureModelAsync()
    {
        // 5 dakikada bir retrain (istersen değiştir)
        if (_model != null && (DateTime.UtcNow - _lastTrainUtc) < TimeSpan.FromMinutes(5))
            return;

        var interactions = await _historyRepo.GetAllInteractionsAsync(50000);

        // Çok az veri varsa model eğitmenin anlamı yok
        if (interactions.Count < 50)
        {
            lock (_lock)
            {
                _model = null;
                _modelSchema = null;
                _lastTrainUtc = DateTime.UtcNow;
            }
            return;
        }

        lock (_lock)
        {
            // başka thread train ediyorsa tekrar train etme (basit kilit)
            if (_model != null && (DateTime.UtcNow - _lastTrainUtc) < TimeSpan.FromMinutes(5))
                return;

            var ml = new MLContext();

            var data = interactions.Select(x => new SongInteraction
            {
                UserId = x.UserId,
                SongId = x.SongId.ToString(),
                Label = 1f
            });

            var trainData = ml.Data.LoadFromEnumerable(data);

            // string -> key
            var pipeline =
                ml.Transforms.Conversion.MapValueToKey("UserIdEncoded", nameof(SongInteraction.UserId))
                .Append(ml.Transforms.Conversion.MapValueToKey("SongIdEncoded", nameof(SongInteraction.SongId)))
                .Append(ml.Recommendation().Trainers.MatrixFactorization(new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserIdEncoded",
                    MatrixRowIndexColumnName = "SongIdEncoded",
                    LabelColumnName = nameof(SongInteraction.Label),
                    NumberOfIterations = 20,
                    ApproximationRank = 64
                }));

            _model = pipeline.Fit(trainData);
            _modelSchema = trainData.Schema;
            _lastTrainUtc = DateTime.UtcNow;
        }
    }
}