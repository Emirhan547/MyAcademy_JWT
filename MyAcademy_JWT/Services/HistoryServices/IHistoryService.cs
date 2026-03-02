namespace MyAcademy_JWT.Services.HistoryServices
{
    public interface IHistoryService
    {
        Task<object> GetMyHistoryAsync(string userId, int take = 50);
    }
}
