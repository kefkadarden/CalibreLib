namespace CalibreLib.Data
{
    public interface IRepositoryBase<T> : IDisposable where T:class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIDAsync(int id);
        void Insert(T item);
        void Delete(T item);
        void Update(T item);
        void SaveAsync();
    }
}
