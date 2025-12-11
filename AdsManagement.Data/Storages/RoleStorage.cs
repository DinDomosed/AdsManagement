using AdsManagement.App.Common;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AdsManagement.Data.Storages
{
    public class RoleStorage : IRoleStorage
    {
        private readonly AdsDbContext _dbContext;

        public RoleStorage(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Role?> GetAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                return null;

            return await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }
        public async Task<PagedResult<Role>> GetAllAsync(int page, int pageSize, CancellationToken token= default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            int totalCount = await _dbContext.Roles.CountAsync(token);

            var roles = await _dbContext.Roles
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
                

             return new PagedResult<Role>()
             {
                 Items = roles,
                 TotalCount = totalCount,
                 Page = page,
                 PageSize = pageSize
             };
        }
        public async Task<bool> AddAsync(Role role, CancellationToken token = default)
        {
            if (role == null)
                return false;

            if (await _dbContext.Roles.AnyAsync(c => c.Name == role.Name, token))
                throw new RoleExistsException("The role already exists");

            _dbContext.Roles.Add(role);

            await _dbContext.SaveChangesAsync(token);
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                return false;

            var dbRole = await _dbContext.Roles.FindAsync(id, token);

            if (dbRole == null)
                throw new RoleNotFoundException($"Role {id} not found");

            _dbContext.Roles.Remove(dbRole);
            await _dbContext.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> UpdateAsync(Role role, CancellationToken token = default)
        {
            if (role == null)
                return false;

            var dbRole = await _dbContext.Roles.FindAsync(role.Id, token);

            if (dbRole == null)
                throw new RoleNotFoundException($"Role {role.Id} not found");

            _dbContext.Entry(dbRole).CurrentValues.SetValues(role);
            await _dbContext.SaveChangesAsync(token);
            return true;
        }

    }
}
