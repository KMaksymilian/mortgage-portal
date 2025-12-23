namespace MortgageComparer.Services.Interfaces {
    public interface IWriteService<T, TKey> where T : BasicEntity {
        void Add(T entity);
        void Update(T entity);
        void Delete(TKey id);
    }
}
