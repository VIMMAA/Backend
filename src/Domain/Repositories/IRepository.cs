using Domain.Abstractions;

namespace Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    public Task<T> CreateAsync(T entity);
    public Task<T> UpdateAsync(T entity);
    public Task<T> DeleteAsync(T entity);
    public Task<T> GetAsync(Guid id);
}