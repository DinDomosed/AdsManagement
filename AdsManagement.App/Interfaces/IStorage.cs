using AdsManagement.Domain.Models;
namespace AdsManagement.App.Interfaces
{
    public interface IStorage<T> 
        where T : BaseEntity
    {
        public Task<T?> GetAsync(Guid id, CancellationToken token = default);
        public Task<bool> AddAsync(T entity, CancellationToken token = default);
        public Task<bool> DeleteAsync(Guid id, CancellationToken token = default); 
        public Task<bool> UpdateAsync(T entity, CancellationToken token = default);
    }
}
