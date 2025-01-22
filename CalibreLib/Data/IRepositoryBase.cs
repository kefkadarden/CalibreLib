namespace CalibreLib.Data
{
    public interface IRepositoryBase<T> : IDisposable where T:class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIDAsync(int id);
        public void Insert(T item);
        public void Delete(T item);
        public void Update(T item);
        public void SaveAsync();
    }
}
