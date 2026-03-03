using Microsoft.ML;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Models.MLNetViewModels;

namespace MyAcademy_JWT.Services.RecommendationServices
{
    public class RecommendationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MLModelState _modelState;

        public RecommendationBackgroundService(
            IServiceScopeFactory scopeFactory,
            MLModelState modelState)
        {
            _scopeFactory = scopeFactory;
            _modelState = modelState;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var historyRepo = scope.ServiceProvider
                    .GetRequiredService<IUserSongHistoryRepository>();

                var interactions = await historyRepo
                    .GetAllInteractionsAsync(50000);

                if (interactions.Count > 50)
                {
                    var ml = new MLContext();

                    var data = interactions.Select(x => new SongInteraction
                    {
                        UserId = x.UserId,
                        SongId = x.SongId.ToString(),
                        Label = 1f
                    });

                    var trainData = ml.Data.LoadFromEnumerable(data);

                    var pipeline =
                        ml.Transforms.Conversion.MapValueToKey("UserIdEncoded", nameof(SongInteraction.UserId))
                        .Append(ml.Transforms.Conversion.MapValueToKey("SongIdEncoded", nameof(SongInteraction.SongId)))
                        .Append(ml.Recommendation().Trainers.MatrixFactorization(
                            new Microsoft.ML.Trainers.MatrixFactorizationTrainer.Options
                            {
                                MatrixColumnIndexColumnName = "UserIdEncoded",
                                MatrixRowIndexColumnName = "SongIdEncoded",
                                LabelColumnName = nameof(SongInteraction.Label),
                                NumberOfIterations = 20,
                                ApproximationRank = 64
                            }));

                    var model = pipeline.Fit(trainData);

                    _modelState.UpdateModel(model, trainData.Schema);
                }

                // 10 dakikada bir retrain
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
