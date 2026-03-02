namespace MyAcademy_JWT.Services.RecommendationServices
{
    public interface IRecommendationService
    {
        Task<object> GetRecommendationsAsync(string userId, int take = 10);
    }
}
