using AdsManagement.App.Common;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Storage;
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
        public async Task<Role> GetAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The role ID cannot be empty");

            var dbRole = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, token) ?? throw new RoleNotFoundException(id);

            return dbRole;
        }
        public async Task<Role> GetByNameAsync(string roleName, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException(nameof(roleName), "Role name cannot be empty");
            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == roleName, token) ?? throw new RoleNotFoundException(Guid.Empty);

            return role;
        }
        public async Task<PagedResult<Role>> GetAllAsync(int page, int pageSize, CancellationToken token = default)
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
        public async Task<Guid> AddAsync(Role role, CancellationToken token = default)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "The role cannot be null");

            if (await _dbContext.Roles.AnyAsync(c => c.Name == role.Name, token))
                throw new RoleExistsException("The role already exists");

            _dbContext.Roles.Add(role);

            await _dbContext.SaveChangesAsync(token);
            return role.Id;
        }
        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The ID cannot be empty");

            var dbRole = await _dbContext.Roles.FindAsync(id, token);

            if (dbRole == null)
                throw new RoleNotFoundException(id);

            _dbContext.Roles.Remove(dbRole);
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(Role role, CancellationToken token = default)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "The role cannot be null");

            var dbRole = await _dbContext.Roles.FindAsync(role.Id, token);

            if (dbRole == null)
                throw new RoleNotFoundException(role.Id);

            _dbContext.Entry(dbRole).CurrentValues.SetValues(role);
            await _dbContext.SaveChangesAsync(token);
        }

    }
}
