using AdsManagement.Domain.Models;
namespace AdsManagement.App.Interfaces.Storage
{
    public interface IStorage<T> 
        where T : BaseEntity
    {
        public Task<T> GetAsync(Guid id, CancellationToken token = default);
        public Task<Guid> AddAsync(T entity, CancellationToken token = default);
        public Task DeleteAsync(Guid id, CancellationToken token = default); 
        public Task UpdateAsync(T entity, CancellationToken token = default);
    }
}
