using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdsManagement.Domain.Models;
namespace AdsManagement.App.Interfaces
{
    public interface IStorage<T> 
        where T : BaseEntity
    {
        public Task<T?> GetAsync(Guid id);
        public Task<bool> AddAsync(T entity);
        public Task<bool> DeleteAsync(Guid id); 
        public Task<bool> UpdateAsync(T entity);
    }
}
