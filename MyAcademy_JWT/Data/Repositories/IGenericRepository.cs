namespace MyAcademy_JWT.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);   // EKLENDİ
        Task DeleteAsync(T entity);
    }
}
