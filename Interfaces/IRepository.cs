namespace Common;

public interface IRepository<T> where T : class
{
    Task<T> Create(T entity, CancellationToken cancellationToken);
    Task<T> ReadById(Guid id, CancellationToken cancellationToken);
    Task<List<T>> Read(int offset, int maxResults, CancellationToken cancellationToken);
    Task<T> Update(T entity);
    Task Delete(T entity);
}